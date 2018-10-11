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
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using GlnApi.DTOs;
using GlnApi.Helpers;
using GlnApi.Models;
using GlnApi.Repository;
using GlnApi.Services;

namespace GlnApi.Controllers
{
    [DualAuthorize]
    public class GlnController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private ILoggerHelper _logger;

        public GlnController(IUnitOfWork unitOfWork, ILoggerHelper logger)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;

        }

        // GET api/<controller>
        [HttpGet]
        [Route("api/get-gln-primary")]
        public IHttpActionResult GetPrimary()
        {
            var primary = _unitOfWork.Glns.FindSingle(gln => gln.Primary);

            if (Equals(primary, null))
                return NotFound();

            return Ok(DtoHelper.CreateGlnIncludeChildrenDto(primary));
        }

        // GET api/<controller>
        [HttpPost]
        [Route("api/get-gln-query")]
        public IHttpActionResult GetGlnByQuery([FromBody] GlnQuery filterResource)
        {
            var queryResult = _unitOfWork.Glns.GetGlnsByQuery(filterResource);

            return Ok(queryResult);
        }

        // GET api/<controller>
        [HttpGet]
        [Route("api/get-glns")]
        public IHttpActionResult GetGlns()
        {
            IEnumerable<GlnDto> glns = _unitOfWork.Glns.Find(bc => bc.Assigned)
                .OrderBy(bc => bc.FriendlyDescriptionPurpose)
                .ToList()
                .Select(DtoHelper.CreateGlnIncludeChildrenDto);

            return Ok(glns);
        }

        // GET api/<controller>
        [HttpGet]
        [Route("api/next-unassigned-gln")]
        public IHttpActionResult GetNextUnassignedGln()
        {
            GlnDto nextUnassignedGln = _unitOfWork.Glns.Find(bc => !bc.Assigned && !bc.AlternativeSystemIsTruth).Select(DtoHelper.CreateGlnIncludeChildrenDto).FirstOrDefault();

            if (nextUnassignedGln.Id <= 0)
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "No unassigned GLN can be located."));

            return Ok(nextUnassignedGln);
        }
        // GET api/<controller>
        [HttpGet]
        [Route("api/change-address-get-next-unassigned-gln/{addressId:int?}")]
        public IHttpActionResult AssignAddressGetNextUnassignedGln(int addressId = 0)
        {
            Address addressToAssign = _unitOfWork.Addresses.FindSingle(a => a.Id == addressId);

            if (addressToAssign == null)
            {
                var nextUnassignedGln = _unitOfWork.Glns.Find(gln => !gln.Assigned && !gln.AlternativeSystemIsTruth).First();

                if (nextUnassignedGln.Id <= 0)
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                        "No unassigned GLN can be located."));

                _unitOfWork.Glns.RemoveAllGlnAssociation(nextUnassignedGln.Id);

                _unitOfWork.Complete();

                return Ok(DtoHelper.CreateGlnIncludeChildrenDto(nextUnassignedGln));
            }
            else
            {
                // If address is found assign the address to next GLN to be assigned
                var nextUnassignedGln = _unitOfWork.Glns.Find(gln => !gln.Assigned && !gln.AlternativeSystemIsTruth).First();

                if (nextUnassignedGln.Id <= 0)
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                        "No unassigned GLN can be located."));

                nextUnassignedGln.AddressId = addressId;
                
                _unitOfWork.Glns.RemoveAllGlnAssociation(nextUnassignedGln.Id);

                _unitOfWork.Complete();

                return Ok(DtoHelper.CreateGlnIncludeChildrenDto(nextUnassignedGln));
            }
        }

        // GET api/<controller>/id
        [HttpGet]
        [Route("api/gln-id/{id:int?}")]
        public IHttpActionResult GetGlnById(int id)
        {

            var gln = _unitOfWork.Glns.Get(id);

            if (Equals(gln, null))
            {
                return BadRequest();
            }
            else
            {
                GlnDto mappedGln = DtoHelper.CreateGlnIncludeChildrenDto(gln);
                return Ok(mappedGln);
            }
        }

        // GET api/<controller>/id
        [HttpGet]
        [Route("api/gln-by-gln/{gln?}")]
        public IHttpActionResult GetGlnByGln(string gln)
        {

            var foundGln = _unitOfWork.Glns.GetGlnsByGln(gln);

            if (Equals(foundGln, null))
            {
                return NotFound();
            }
            else
            {
                GlnDto mappedGln = DtoHelper.CreateGlnIncludeChildrenDto(foundGln);
                return Ok(mappedGln);
            }
        }

        // GET api/<controller>/search
        [HttpGet]
        [Route("api/gln-search/{search?}")]
        public IHttpActionResult GetGlns(string search = "")
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                List<GlnDto> emptyList = new List<GlnDto>();
                return Ok(emptyList);
            }

            IEnumerable<GlnDto> glns = _unitOfWork.Glns.Find(bc => bc.Assigned
                                                                           && bc.FriendlyDescriptionPurpose.Contains(search)
                                                                           || bc.OwnGln.StartsWith(search))
                .OrderBy(bc => bc.FriendlyDescriptionPurpose)
                .ToList()
                .Select(DtoHelper.CreateGlnIncludeChildrenDto);

            return Ok(glns);
        }

        // GET api/<controller>/search
        //[AllowAnonymous]
        [HttpGet]
        [Route("api/gln-search/take/{take:int?}/search/{search?}")]
        public IHttpActionResult GetGlns(string search = "empty", int take = 5)
        {
            if (string.IsNullOrWhiteSpace(search) || Equals(search, "empty"))
            {
                List<GlnDto> emptyList = new List<GlnDto>();
                return Ok(emptyList);
            }


            IEnumerable<GlnDto> glns = _unitOfWork.Glns.Find(bc => bc.Assigned
                                                                           && bc.FriendlyDescriptionPurpose.Contains(search)
                                                                           || bc.OwnGln.StartsWith(search))
                .OrderBy(bc => bc.FriendlyDescriptionPurpose)
                .Take(take)
                .ToList()
                .Select(DtoHelper.CreateGlnIncludeChildrenDto);

            return Ok(glns);
        }

        // GET api/<controller>/search
        [HttpGet]
        [Route("api/child-glns/{gln?}")]
        public IHttpActionResult GetChildrenOfGln(string gln = "")
        {
            if (string.IsNullOrWhiteSpace(gln))
            {
                List<GlnSummaryDto> emptyList = new List<GlnSummaryDto>();
                return Ok(emptyList);
            }

            IEnumerable<GlnSummaryDto> childGlns = _unitOfWork.Glns.Find(bc => bc.ParentGln == gln)
                .Where(cb => cb.Assigned)
                .ToList()
                .OrderBy(bc => bc.FriendlyDescriptionPurpose)
                .Select(DtoHelper.CreateGlnSummaryDto);

            return Ok(childGlns);

        }

        // GET api/<controller>/search
        [HttpGet]
        [Route("api/child-glns/{id:int}")]
        public IHttpActionResult GetChildrenOfGln(int id)
        {
            if (id <= 0)
            {
                List<GlnSummaryDto> emptyList = new List<GlnSummaryDto>();
                return Ok(emptyList);
            }

            var parent = _unitOfWork.Glns.Get(id);

            IEnumerable<GlnSummaryDto> children = _unitOfWork.Glns.Find(bc => bc.ParentGln == parent.OwnGln)
                .Where(cb => cb.Assigned)
                .ToList()
                .OrderBy(bc => bc.FriendlyDescriptionPurpose)
                .Select(DtoHelper.CreateGlnSummaryDto);

            if (children.Any())
            {
                parent.NumberOfChildren = children.Count();
                _unitOfWork.Complete();
            }

            return Ok(children);

        }

        // GET api/<controller>/int
        [HttpGet]
        [Route("api/gln-associations/{id:int}")]
        public IHttpActionResult GetGlnAssociations(int id)
        {
            if (id <= 0)
                return BadRequest();

            var associations = _unitOfWork.Glns.GetGlnAssociations(id);

            if (Equals(!associations.Any()))
                return NotFound();

            var associationsDtos = ConvertAssociationsToDtoSummary(associations);

            return Ok(associationsDtos);

        }

        // GET: api/GlnTagType/Glns/5
        [HttpGet]
        [Route("api/glns/gln-tag-type-id/{tagTypeId:int?}")]
        [ResponseType(typeof(IEnumerable<GlnDto>))]
        public IHttpActionResult GetGlnsByGlnTagType(int tagTypeId)
        {
            var tagType = _unitOfWork.GlnTagType.FindSingle(tt => tt.GlnTagTypeId == tagTypeId);
            var glnIds = _unitOfWork.GlnTag.Find(t => t.GlnTagTypeId == tagTypeId).Select(g => g.GlnId);

            if (!glnIds.Any())
                return NotFound();

            var glns = _unitOfWork.Glns.GetAll().Where(g => glnIds.Contains(g.Id) && g.TrustActive).ToList().Select(DtoHelper.CreateGlnDto);

            return Ok(glns);
        }

        // PUT api/<controller>
        [Authorize(Roles = "GLNadmin")]
        [HttpPut]
        [Route("api/update-gln")]
        public IHttpActionResult UpdateGln(Gln gln)
        {
            if (Equals(gln, null))
                return BadRequest();

            var glnToUpdate = _unitOfWork.Glns.FindSingle(g => g.Id == gln.Id);
            var glnBeforeUpdate = DtoHelper.CreateGlnIncludeChildrenDto(glnToUpdate);

            if (Equals(glnToUpdate, null))
                return BadRequest();

            var currentDbVersion = glnToUpdate.Version;

            if (!ConcurrencyChecker.canSaveChanges(gln.Version, currentDbVersion))
            {
                _logger.ConcurrenyServerLog(HttpContext.Current.User, gln.Version, currentDbVersion);
                return Conflict();
            }

            var updatedGln = _unitOfWork.Glns.UpdateGln(gln);

            try
            {
                _unitOfWork.Complete();

                var completed = _unitOfWork.Glns.FindSingle(g => g.Id == gln.Id);

                _logger.SuccessfulUpdateServerLog(HttpContext.Current.User, glnBeforeUpdate, DtoHelper.CreateGlnDto(completed));

                if (glnBeforeUpdate.ParentGln != glnToUpdate.ParentGln)
                {
                    if (!string.IsNullOrEmpty(glnBeforeUpdate.ParentGln))
                    {
                        // Update number of children on previous parent
                        var oldParent = _unitOfWork.Glns.FindSingle(g => g.OwnGln == glnBeforeUpdate.ParentGln);
                        oldParent.NumberOfChildren = _unitOfWork.Glns.Find(g => g.ParentGln == oldParent.OwnGln).Count();
                        _unitOfWork.Complete();
                    }

                    if (!string.IsNullOrEmpty(glnToUpdate.ParentGln))
                    {
                        // Update number of children on new parent that has aquired an additional child
                        var newParent = _unitOfWork.Glns.FindSingle(g => g.OwnGln == glnToUpdate.ParentGln);
                        newParent.NumberOfChildren = _unitOfWork.Glns.Find(g => g.ParentGln == newParent.OwnGln).Count();
                        _unitOfWork.Complete();
                    }
                }

                return Ok(DtoHelper.CreateGlnIncludeChildrenDto(completed));
            }
            catch (Exception ex)
            {
                _logger.FailedUpdateServerLog(HttpContext.Current.User, ex.Message, ex.InnerException, DtoHelper.CreateGlnDto(glnToUpdate), DtoHelper.CreateGlnIncludeChildrenDto(updatedGln));
                return InternalServerError();
            }
        }

        // PUT api/<controller>/5
        [Authorize(Roles = "GLNadmin")]
        [HttpPut]
        [Route("api/assign-primary-contact-to-gln/{glnId:int?}")]
        public IHttpActionResult AssignPrimaryContactToGln(int glnId, [FromBody] PrimaryContact selectedContact)
        {
            var glnToUpdate = _unitOfWork.Glns.FindSingle(b => b.Id == glnId);
            var glnBeforeUpdate = DtoHelper.CreateGlnDto(glnToUpdate);

            try
            {
                var currentDbVersion = _unitOfWork.Glns.FindSingle(g => g.Id == glnId).Version;

                if (ConcurrencyChecker.canSaveChanges(glnToUpdate.Version, currentDbVersion) && _unitOfWork.Glns.PrimaryContactExists(selectedContact.Id))
                {
                    glnToUpdate.ContactId = selectedContact.Id;
                    glnToUpdate.Version = glnToUpdate.Version + 1;
                    _unitOfWork.Complete();

                    var assignedDetails =
                        $"Contact {selectedContact.Name}, Id: {selectedContact.Id} " +
                        $"was assigned to GLN: {glnToUpdate.OwnGln}";

                    _logger.SuccessfulUpdateServerLog(HttpContext.Current.User, glnBeforeUpdate, assignedDetails);

                    return Ok(DtoHelper.CreateGlnIncludeChildrenDto(glnToUpdate));
                }
                else
                {
                    return Conflict();
                }
            }
            catch (Exception ex)
            {
                var failedMsg =
                    $"Failed to assign contact {selectedContact.Name}, Id: {selectedContact.Id} to GLN: {glnToUpdate.OwnGln}. Exception generated: {ex}";

                _logger.FailedUpdateServerLog<Exception, string, object>(HttpContext.Current.User, ex.Message, ex.InnerException, failedMsg);

                return InternalServerError();
            }
        }

        // PUT api/<controller>/OrignalParentGln/{origninalParentGln}/NewParentGln/{newParentGln}
        [Authorize(Roles = "GLNadmin")]
        [HttpPut]
        [Route("api/change-parent-on-children/orignal-parent-gln/{origninalParentGln}/new-parent-gln/{newParentGln}")]
        public IHttpActionResult ChangeParentOnChildren(string origninalParentGln, string newParentGln)
        {
            if (string.IsNullOrWhiteSpace(origninalParentGln) || string.IsNullOrWhiteSpace(newParentGln))
                return BadRequest();

            var childrenToUpdateParent = _unitOfWork.Glns.Find(gln => gln.ParentGln == origninalParentGln).ToList();
            var originalParent = _unitOfWork.Glns.FindSingle(gln => gln.OwnGln == origninalParentGln);
            var newParent = _unitOfWork.Glns.FindSingle(gln => gln.OwnGln == newParentGln);

            if (Equals(childrenToUpdateParent, null) || Equals(originalParent, null) || Equals(newParent, null))
                return BadRequest();

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
                _unitOfWork.Complete();

                originalParent.NumberOfChildren = _unitOfWork.Glns.Find(gln => gln.ParentGln == origninalParentGln).Count();
                originalParent.Version = originalParent.Version + 1;

                newParent.NumberOfChildren = _unitOfWork.Glns.Find(gln => gln.ParentGln == newParentGln).Count();
                newParent.Version = newParent.Version + 1;

                _unitOfWork.Complete();

                var successMsg =
                    $"The children of parent GLN: {origninalParentGln} were all assigned to new parent GLN: {newParentGln}";
                foreach (var child in childrenToUpdateParent)
                {
                    var i = 1;
                    successMsg = successMsg + $" Child No. {i} {child.OwnGln}, ";
                    i = i + 1;
                }

                _logger.SuccessfulUpdateServerLog(HttpContext.Current.User, DtoHelper.CreateGlnDto(originalParent), successMsg);

                GlnDto returnParentGln = DtoHelper.CreateGlnIncludeChildrenDto(_unitOfWork.Glns.FindSingle(gln => gln.OwnGln == origninalParentGln));

                return Ok(returnParentGln);
            }
            catch (Exception ex)
            {
                var failedMsg =
                    $"Failed to update children of parent {origninalParentGln} to new parent {newParentGln}. Exception generated: {ex}";

                _logger.FailedUpdateServerLog<Exception, string, object>(HttpContext.Current.User, ex.Message, ex.InnerException, failedMsg);

                return InternalServerError();
            }
        }

        // PUT api/<controller>/OrignalParentGln/{origninalParentGln}/NewParentGln/{newParentGln}
        [Authorize(Roles = "GLNadmin")]
        [HttpPut]
        [Route("api/change-parent/gln-id/{glnId}/original-parent-gln/{originalParentGln}/new-parent-gln/{newParentGln}")]
        public IHttpActionResult ChangeParent(int glnId, string originalParentGln, string newParentGln)
        {
            if (glnId <= 0 || string.IsNullOrWhiteSpace(originalParentGln) || string.IsNullOrWhiteSpace(newParentGln))
                return BadRequest();

            var updateParentGln = _unitOfWork.Glns.FindSingle(gln => gln.Id == glnId);
            var newParent = _unitOfWork.Glns.FindSingle(gln => gln.OwnGln == newParentGln);
            var oldParent = _unitOfWork.Glns.FindSingle(gln => gln.OwnGln == originalParentGln);

            if (Equals(updateParentGln, null) || Equals(newParent, null) || Equals(oldParent, null))
                return BadRequest();

            // Don't update if alternative system is responsible for holding truth state
            if (updateParentGln.AlternativeSystemIsTruth)
                return BadRequest();

            try
            {
                //Check to ensure not assigning itself as its own parent and
                //Check that new parents parent is not the same as childs own number to avoid creating a loop
                if (updateParentGln.OwnGln != newParent.OwnGln &&
                    newParent.ParentGln != updateParentGln.OwnGln)
                {
                    var updateGln = _unitOfWork.Glns.FindSingle(gln => gln.Id == glnId);
                    updateGln.ParentDescriptionPurpose = newParent.FriendlyDescriptionPurpose;
                    updateGln.ParentGln = newParent.OwnGln;
                    updateGln.ParentId = newParent.Id;

                    if (newParent.TierLevel >= 5)
                    {
                        updateGln.TierLevel = newParent.TierLevel;
                    }
                    else
                    {
                        updateGln.TierLevel = newParent.TierLevel + 1;
                    }

                    updateGln.Version = updateGln.Version + 1;

                    if (updateGln.PhysicalType)
                    {
                        var primaryContact = _unitOfWork.PrimaryContacts
                            .FindSingle(pc => pc.ForPhysicals == true);

                        if (primaryContact != null)
                            updateGln.ContactId = primaryContact.Id;
                    }
                    else
                    {
                        var primaryContact = _unitOfWork.PrimaryContacts
                            .FindSingle(pc => pc.ForFunctionals == true);

                        if (primaryContact != null)
                            updateGln.ContactId = primaryContact.Id;
                    }

                    _unitOfWork.Complete();

                    UpdateTierLevel(updateGln);

                    var successMsg =
                        $"The parent of GLN: {updateGln.OwnGln} was updated from {originalParentGln} to new parent GLN: {newParentGln}";

                    _logger.SuccessfulUpdateServerLog<string, object>(HttpContext.Current.User, successMsg);

                    GlnDto returnUpdatedGln = DtoHelper.CreateGlnIncludeChildrenDto(_unitOfWork.Glns.FindSingle(gln => gln.Id == glnId));
                    return Ok(returnUpdatedGln);
                }

                return Ok(updateParentGln);
            }
            catch (Exception ex)
            {
                var failedMsg = $"Failed to update parent on {updateParentGln.OwnGln} " +
                                $"from to old parent {originalParentGln} to new parent {newParentGln}. Exception generated: {ex}";

                _logger.FailedUpdateServerLog<Exception, string, object>(HttpContext.Current.User, ex.Message, ex.InnerException, failedMsg);

                return InternalServerError();
            }
        }

        private void UpdateTierLevel(Gln updateGln)
        {
            var children = _unitOfWork.Glns.Find(glns => glns.ParentGln == updateGln.OwnGln).ToList();

            if (children.Any())
            {
                foreach (var child in children)
                {
                    if (updateGln.TierLevel >= 5)
                    {
                        child.TierLevel = updateGln.TierLevel;
                        // Parent cannot be set above tier 5 so use parent of updated GLN
                        child.ParentGln = updateGln.ParentGln;
                        UpdateTierLevel(child);

                    }
                    else
                    {
                        child.TierLevel = updateGln.TierLevel + 1;
                        UpdateTierLevel(child);
                    }
                }

                _unitOfWork.Complete();
            }
        }

        // PUT api/<controller>/id
        [Authorize(Roles = "GLNadmin")]
        [HttpPut]
        [Route("api/new-assigned-gln")]
        public IHttpActionResult NewAssignedGln([FromBody] Gln assignedGln)
        {
            if (Equals(assignedGln, null) || Equals(assignedGln.Address, null))
                return BadRequest();

            GlnDto updateGln = DtoHelper.CreateGlnIncludeChildrenDto(_unitOfWork.Glns.FindSingle(gln => gln.Id == assignedGln.Id));

            if (Equals(updateGln, null))
                return BadRequest();

            _unitOfWork.Glns.UpdateGln(assignedGln);

            return Ok(DtoHelper.CreateGlnIncludeChildrenDto(assignedGln));
        }

        // POST api/<controller>/id1/id2
        [Authorize(Roles = "GLNadmin")]
        [HttpPost]
        [Route("api/create-gln-association/{id1:int?}/{id2:int?}")]
        public IHttpActionResult CreateGlnAssociation(int id1, int id2)
        {
            if (Equals(id1 <= 0) || Equals(id2 <= 0))
                return BadRequest();

            _unitOfWork.Glns.AssignNewGlnAssociation(id1, id2);

            _unitOfWork.Complete();

            var updatedAssociations = _unitOfWork.Glns.GetGlnAssociations(id1);

            if (Equals(!updatedAssociations.Any()))
                return Ok();

            var updatedAssociationsDto = ConvertAssociationsToDtoSummary(updatedAssociations);

            return Ok(updatedAssociationsDto);

        }
        // POST api/<controller>/id1/id2
        [Authorize(Roles = "GLNadmin")]
        [HttpPost]
        [Route("api/remove-gln-association/{id1:int?}/{id2:int?}")]
        public IHttpActionResult RemoveGlnAssociation(int id1, int id2)
        {
            if (Equals(id1 <= 0) || Equals(id2 <= 0))
                return BadRequest();

            _unitOfWork.Glns.RemoveGlnAssociation(id1, id2);

            _unitOfWork.Complete();

            var updatedAssociations = _unitOfWork.Glns.GetGlnAssociations(id1);

            if (Equals(!updatedAssociations.Any()))
                return Ok();

            var updatedAssociationsDto = ConvertAssociationsToDtoSummary(updatedAssociations);

            return Ok(updatedAssociationsDto);

        }

        private IEnumerable<GlnSummaryDto> ConvertAssociationsToDtoSummary(IEnumerable<Gln> associations)
        {
            var associationsDtos = new List<GlnSummaryDto>();

            foreach (var a in associations)
            {
                associationsDtos.Add(DtoHelper.CreateGlnSummaryDto(a));
            }

            return associationsDtos;
        }
    }
}
