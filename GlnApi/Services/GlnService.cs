//Global Location Number (GLN) Registry API
//Copyright (C) 2018  University Hospitals Plymouth NHS Trust
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using GlnApi.DTOs;
using GlnApi.Helpers;
using GlnApi.Models;
using GlnApi.Repository;

namespace GlnApi.Services
{
    public class GlnService
    {
        private readonly GlnRegistryDb _db;
        private readonly MongoLoggerHelper _mongoMongoLogger;
        private readonly IUnitOfWork _unitOfWork;

        public GlnService(GlnRegistryDb db, MongoLoggerHelper mongoLoggerHelper, IUnitOfWork unitOfWork)
        {
            _db = db;
            _mongoMongoLogger = mongoLoggerHelper;
            _unitOfWork = unitOfWork;
        }

        public void CalculateChildrenAllGlns()
        {
            foreach (var gln in _db.Glns)
            {
                gln.NumberOfChildren = CalculateChildrenNumbers(gln.OwnGln);
                var parent = _unitOfWork.Glns.Find(p => gln.ParentGln == p.OwnGln);

                if (!Equals(parent, null) && !gln.Primary)
                {
                    gln.ParentDescriptionPurpose = parent.FirstOrDefault(p => p.OwnGln == gln.ParentGln).FriendlyDescriptionPurpose;
                }
            }
            _db.SaveChanges();
        }

        public IEnumerable<GlnDto> GetGln()
        {
            CalculateChildrenAllGlns();

            var glns = _db.Glns.Where(b => b.Assigned == true)
                .OrderBy(bc => bc.FriendlyDescriptionPurpose)
                .ToList()
                .Select(DtoHelper.CreateGlnIncludeChildrenDto);

            return glns;
        }

        public IEnumerable<GlnDto> GetGln(string search)
        {
            var glns = _db.Glns
                .Where(bc => bc.Assigned)
                .Where(bc => bc.OwnGln.StartsWith(search) || bc.FriendlyDescriptionPurpose.StartsWith(search))
                .OrderBy(bc => bc.FriendlyDescriptionPurpose)
                .ToList()
                .Select(DtoHelper.CreateGlnIncludeChildrenDto);

            return glns;
        }

        public IEnumerable<GlnDto> GetGln(string search, int take)
        {
            var glns = _db.Glns
                .Where(bc => bc.Assigned == true)
                .Where(bc => bc.OwnGln.StartsWith(search) || bc.FriendlyDescriptionPurpose.StartsWith(search))
                .OrderBy(bc => bc.FriendlyDescriptionPurpose)
                .Take(take)
                .ToList()
                .Select(DtoHelper.CreateGlnIncludeChildrenDto);

            return glns;
        }

        public PaginationDto<GlnSummaryDto> GetPagedGlns(int pageNumber, int pageSize = 10)
        {
            var glns = _db.Glns.Where(bc => bc.Assigned)
                .OrderBy(bc => bc.FriendlyDescriptionPurpose)
                .ThenBy(bc => bc.OwnGln)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize * 3)
                .Select(DtoHelper.CreateGlnSummaryDto);

            var totalCount = _db.Glns.Count(bc => bc.Assigned);

            var paginationDto = new PaginationDto<GlnSummaryDto>(pageNumber, pageSize, totalCount, glns);

            return paginationDto;
        }

        public PaginationDto<GlnSummaryDto> GetPagedGlnsBySearchTerm(int pageNumber, int pageSize = 10,
            string searchTerm = "")
        {
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var totalCount =
                    _db.Glns.Count(
                        bc =>
                            bc.Assigned && bc.OwnGln.StartsWith(searchTerm) ||
                            bc.FriendlyDescriptionPurpose.StartsWith(searchTerm));
                var totalPages = Math.Ceiling((double)totalCount / pageSize);

                if (totalPages < pageNumber || searchTerm == "")
                    pageNumber = 1;

                var glns = _db.Glns.Where(bc => bc.Assigned)
                    .Where(bc => bc.OwnGln.StartsWith(searchTerm) || bc.FriendlyDescriptionPurpose.Contains(searchTerm))
                    .OrderBy(bc => bc.FriendlyDescriptionPurpose)
                    .ThenBy(bc => bc.OwnGln)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize * 3)
                    .Select(DtoHelper.CreateGlnSummaryDto);

                var paginationDto = new PaginationDto<GlnSummaryDto>(pageNumber, pageSize, totalCount, glns);

                return paginationDto;
            }
            else
            {
                var paginationDto = GetPagedGlns(1, pageSize);
                return paginationDto;
            }
        }

        public GlnDto GetGlnById(int id)
        {
            var gln = _db.Glns.SingleOrDefault(b => b.Id == id);

            if (!Equals(gln, null))
            {
                //Everytime gln is fetched calculate its children to ensure it is up to date
                var currentDbVersion = _db.Glns.SingleOrDefault(bc => bc.Id == id).Version;
                if (ConcurrencyChecker.canSaveChanges(gln.Version, currentDbVersion))
                    gln.NumberOfChildren = CalculateChildrenNumbers(gln.OwnGln);

                // If primary then GLN will not have a parent
                try
                {
                    if (!gln.Primary)
                        gln.ParentDescriptionPurpose =
                            _db.Glns.SingleOrDefault(bc => bc.OwnGln == gln.ParentGln).FriendlyDescriptionPurpose;

                    _db.SaveChanges();
                }
                catch (Exception ex)
                {
                    var failedMsg = $"Failed to update Parent Description Purpose on {gln.OwnGln} " +
                                    $". Exception generated: {ex}";

                    _mongoMongoLogger.FailedUpdateServerLog<Exception, string, object>(HttpContext.Current.User, ex.Message, ex.InnerException, failedMsg);
                }
            }

            return DtoHelper.CreateGlnIncludeChildrenDto(gln);
        }
        public GlnDto GetGlnByGln(string gln)
        {
            var findGlns = _db.Glns.SingleOrDefault(b => b.OwnGln == gln);

            if (!Equals(findGlns, null))
            {
                //Everytime gln is fetched calculate its children to ensure it is up to date
                var currentDbVersion = _db.Glns.SingleOrDefault(bc => bc.OwnGln == gln).Version;
                if (ConcurrencyChecker.canSaveChanges(findGlns.Version, currentDbVersion))
                    findGlns.NumberOfChildren = CalculateChildrenNumbers(findGlns.OwnGln);

                // If primary then GLN will not have a parent
                if (!findGlns.Primary)
                    findGlns.ParentDescriptionPurpose =
                        _db.Glns.SingleOrDefault(bc => bc.OwnGln == findGlns.ParentGln).FriendlyDescriptionPurpose;

                _db.SaveChanges();
            }

            return DtoHelper.CreateGlnIncludeChildrenDto(findGlns);
        }

        public GlnDto GetNextUnassignedGln()
        {
            var gln = _db.Glns.FirstOrDefault(b => !b.Assigned && !b.AlternativeSystemIsTruth);

            if (Equals(gln, null))
                return new GlnDto();

            return DtoHelper.CreateGlnIncludeChildrenDto(gln);
        }

        public IEnumerable<GlnSummaryDto> GetChildrenOfGln(string gln)
        {
            var childrenOfGln = _db.Glns
                .Where(bc => bc.ParentGln == gln)
                .ToList()
                .OrderBy(bc => bc.FriendlyDescriptionPurpose)
                .Select(DtoHelper.CreateGlnSummaryDto);

            return childrenOfGln;
        }

        public HttpStatusCode AssignNewGln(int contactId, int addressId, Gln gln)
        {
            if (contactId <= 0)
                return HttpStatusCode.BadRequest;

            if (addressId <= 0)
                return HttpStatusCode.BadRequest;

            var glnToUpdate = _db.Glns.SingleOrDefault(b => b.Id == gln.Id);
            var glnBeforeUpdate = DtoHelper.CreateGlnIncludeChildrenDto(glnToUpdate);
            var currentDbVersion = glnToUpdate.Version;

            try
            {
                if (ConcurrencyChecker.canSaveChanges(gln.Version, currentDbVersion))
                {
                    glnToUpdate.Assigned = true;
                    glnToUpdate.Active = gln.Active;
                    glnToUpdate.FriendlyDescriptionPurpose = gln.FriendlyDescriptionPurpose;
                    glnToUpdate.FunctionalType = gln.FunctionalType;
                    glnToUpdate.LegalType = gln.LegalType;
                    glnToUpdate.PhysicalType = gln.PhysicalType;
                    glnToUpdate.DigitalType = gln.DigitalType;
                    glnToUpdate.Public = gln.Public;
                    glnToUpdate.ParentGln = gln.ParentGln;
                    glnToUpdate.AddressId = gln.AddressId;
                    glnToUpdate.ContactId = gln.ContactId;
                    glnToUpdate.UseParentAddress = gln.UseParentAddress;
                    glnToUpdate.Version = gln.Version + 1;
                    glnToUpdate.TrustActive = gln.TrustActive;
                    glnToUpdate.SuspensionReason = gln.SuspensionReason;
                    glnToUpdate.Function = gln.Function;
                    glnToUpdate.Department = gln.Department;
                    glnToUpdate.TierLevel = gln.TierLevel;
                    //Calculate how many children this gln has
                    glnToUpdate.NumberOfChildren = CalculateChildrenNumbers(glnToUpdate.OwnGln);
                    glnToUpdate.ParentGln = _db.Glns.SingleOrDefault(bc => bc.Primary).OwnGln;

                    _db.SaveChanges();

                    _mongoMongoLogger.SuccessfulUpdateServerLog(HttpContext.Current.User, glnBeforeUpdate, DtoHelper.CreateGlnIncludeChildrenDto(glnToUpdate));

                    return HttpStatusCode.OK;
                }
                else
                {
                    _mongoMongoLogger.ConcurrenyServerLog<object, object>(HttpContext.Current.User, glnBeforeUpdate);
                    return HttpStatusCode.Conflict;
                }

            }
            catch (Exception ex)
            {
                var failedMsg = $"Failed to assign new GLN {glnToUpdate.OwnGln}, {glnToUpdate.FriendlyDescriptionPurpose}.";

                _mongoMongoLogger.FailedUpdateServerLog<Exception, string, object>(HttpContext.Current.User, ex.Message, ex.InnerException, failedMsg);

                return HttpStatusCode.InternalServerError;
            }
        }

        public HttpStatusCode ChangeParent(int barcodeId, string origninalParentGln, string newParentGln)
        {
            var glnToUpdateParentOn = _db.Glns.SingleOrDefault(bc => bc.Id == barcodeId);
            var newParent = _db.Glns.SingleOrDefault(bc => bc.OwnGln == newParentGln);
            var oldParent = _db.Glns.SingleOrDefault(bc => bc.OwnGln == origninalParentGln);

            if (Equals(glnToUpdateParentOn, null) || Equals(newParent, null) || Equals(oldParent, null))
                return HttpStatusCode.BadRequest;

            // Don't update if alternative system is responsible for holding truth state
            if (glnToUpdateParentOn.AlternativeSystemIsTruth)
                return HttpStatusCode.BadRequest;

            try
            {
                //Check to ensure not assigning itself as its own parent and
                //Check that new parents parent is not the same as childs own number to avoid creating a loop
                if (glnToUpdateParentOn.OwnGln != newParent.OwnGln &&
                    newParent.ParentGln != glnToUpdateParentOn.OwnGln)
                {
                    glnToUpdateParentOn.ParentGln = newParentGln;
                    glnToUpdateParentOn.ParentDescriptionPurpose = _db.Glns.Any(p => p.OwnGln == newParentGln)
                        ? _unitOfWork.Glns.FindSingle(p => p.OwnGln == newParentGln).FriendlyDescriptionPurpose : "";
                    glnToUpdateParentOn.Version = glnToUpdateParentOn.Version + 1;

                    _db.SaveChanges();

                    //Update the number of children on the original parent who has now lost a child
                    oldParent.NumberOfChildren = CalculateChildrenNumbers(origninalParentGln);
                    oldParent.Version = oldParent.Version + 1;

                    //Update the number of children on the new parent who has now gained a child
                    newParent.NumberOfChildren = CalculateChildrenNumbers(newParentGln);
                    newParent.Version = newParent.Version + 1;

                    _db.SaveChanges();

                    var successMsg =
                        $"The parent of GLN: {glnToUpdateParentOn.OwnGln} was updated from {origninalParentGln} to new parent GLN: {newParentGln}";

                    _mongoMongoLogger.SuccessfulUpdateServerLog<string, object>(HttpContext.Current.User, successMsg);

                    return HttpStatusCode.OK;
                }

                return HttpStatusCode.Conflict;
            }
            catch (Exception ex)
            {
                var failedMsg = $"Failed to update parent on {glnToUpdateParentOn.OwnGln} " +
                                $"from to old parent {origninalParentGln} to new parent {newParentGln}. Exception generated: {ex}";

                _mongoMongoLogger.FailedUpdateServerLog<Exception, string, object>(HttpContext.Current.User, ex.Message, ex.InnerException, failedMsg);

                return HttpStatusCode.InternalServerError;
            }
        }

        public HttpStatusCode ChangeParentOnChildren(string origninalParentGln, string newParentGln)
        {
            var childrenToUpdateParent = _db.Glns.Where(bc => bc.ParentGln == origninalParentGln).ToList();
            var originalParent = _db.Glns.SingleOrDefault(bc => bc.OwnGln == origninalParentGln);
            var newParent = _db.Glns.SingleOrDefault(bc => bc.OwnGln == newParentGln);

            if (Equals(childrenToUpdateParent, null) || Equals(originalParent, null) || Equals(newParent, null))
                return HttpStatusCode.BadRequest;

            try
            {
                foreach (var child in childrenToUpdateParent)
                {
                    //Check to ensure not assigning itself as its own parent and
                    //Check that new parents parent is not the same as childs own number to avoid creating a loop
                    if (child.OwnGln != newParentGln && newParent.ParentGln != child.OwnGln)
                    {
                        // Don't update if alternative system is responsible for holding truth state
                        if (!child.AlternativeSystemIsTruth)
                        {
                            child.ParentGln = newParentGln;
                            child.Version = child.Version + 1;
                        }
                    }
                }
                //Required so that the calculations for children below will be correct
                _db.SaveChanges();

                originalParent.NumberOfChildren = CalculateChildrenNumbers(origninalParentGln);
                originalParent.Version = originalParent.Version + 1;

                newParent.NumberOfChildren = CalculateChildrenNumbers(newParentGln);
                newParent.Version = newParent.Version + 1;

                _db.SaveChanges();

                var successMsg =
                    $"The children of parent GLN: {origninalParentGln} were all assigned to new parent GLN: {newParentGln}";
                foreach (var child in childrenToUpdateParent)
                {
                    var i = 1;
                    successMsg = successMsg + $" Child No. {i} {child.OwnGln}, ";
                    i = i + 1;
                }

                _mongoMongoLogger.SuccessfulUpdateServerLog(HttpContext.Current.User, DtoHelper.CreateGlnIncludeChildrenDto(originalParent), successMsg);

                return HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                var failedMsg =
                    $"Failed to update children of parent {origninalParentGln} to new parent {newParentGln}. Exception generated: {ex}";

                _mongoMongoLogger.FailedUpdateServerLog<Exception, string, object>(HttpContext.Current.User, ex.Message, ex.InnerException, failedMsg);

                return HttpStatusCode.InternalServerError;
            }
        }

        public Address GetAddress(int id)
        {
            return _db.Addresses.Find(id);
        }

        public HttpStatusCode AddNewGln(Gln newGln)
        {
            var newAddress = newGln.Address;

            try
            {
                _db.Addresses.Add(newAddress);
                _db.SaveChanges();

                var glnToSave = new Gln()
                {
                    Active = newGln.Active,
                    AddressId = newAddress.Id,
                    Assigned = true,
                    ContactId = newGln.ContactId,
                    CreationDate = newGln.CreationDate,
                    FriendlyDescriptionPurpose = newGln.FriendlyDescriptionPurpose,
                    DigitalType = newGln.DigitalType,
                    FunctionalType = newGln.FunctionalType,
                    LegalType = newGln.LegalType,
                    PhysicalType = newGln.PhysicalType,
                    Public = newGln.Public,
                    OwnGln = newGln.OwnGln,
                    ParentGln = _db.Glns.SingleOrDefault(bc => bc.Primary).OwnGln,
                    UseParentAddress = newGln.UseParentAddress,
                    Verified = newGln.Verified,
                    Version = 1,
                    NumberOfChildren = 0,
                    SuspensionDate = newGln.CreationDate,
                    ParentDescriptionPurpose = "",
                    AlternativeSystemIsTruth = false,
                    TruthDescriptionPurpose = newGln.FriendlyDescriptionPurpose
                };

                _unitOfWork.Glns.Add(glnToSave);
                _unitOfWork.Complete();
                _unitOfWork.Dispose();
            }
            catch (Exception)
            {
                return HttpStatusCode.InternalServerError;
            }

            return HttpStatusCode.OK;
        }

        public int CalculateChildrenNumbers(string parentGln)
        {
            if (string.IsNullOrWhiteSpace(parentGln))
                throw new ArgumentException("No gln GLN supplied");

            return _db.Glns.Where(bc => bc.ParentGln == parentGln).Count();

        }

        public void CalculateChildrenNumbersForParentAndChild(string parentGln, string childGln)
        {
            if (!string.IsNullOrWhiteSpace(parentGln))
                _db.Glns.FirstOrDefault(bc => bc.OwnGln == parentGln).NumberOfChildren =
                    CalculateChildrenNumbers(parentGln);


            if (!string.IsNullOrWhiteSpace(childGln))
                _db.Glns.FirstOrDefault(bc => bc.OwnGln == childGln).NumberOfChildren =
                    CalculateChildrenNumbers(childGln);

        }
    }
}