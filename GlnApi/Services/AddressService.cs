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
using GlnApi.Helpers;
using GlnApi.Repository;

namespace GlnApi.Services
{
    public class AddressService : IAddressService
    {
        private readonly GlnRegistryDb _db;
        private readonly MongoLoggerHelper _mongoMongoLogger;
        private readonly IUnitOfWork _unitOfWork;

        public AddressService(GlnRegistryDb db, MongoLoggerHelper mongoLoggerHelper, IUnitOfWork unitOfWork)
        {
            _db = db;
            _mongoMongoLogger = mongoLoggerHelper;
            _unitOfWork = unitOfWork;
        }

        public Address GetAddress(int id)
        {
            //var address = _db.Addresses.Find(id);
            var address = _unitOfWork.Addresses.FindSingle(a => a.Id == id);

            return address;
        }
        public Address GetAddressByGln(string gln)
        {
            //var barcode = _db.Barcodes.FirstOrDefault(g => g.OwnGln == gln);
            var glnToFind = _unitOfWork.Glns.FindSingle(g => g.OwnGln == gln);

            //var address = _db.Addresses.Find(barcode.AddressId);
            var address = _unitOfWork.Addresses.FindSingle(a => a.Id == glnToFind.AddressId);

            return address;
        }

        public void UpdateAddress(Address address)
        {
            //var addressToUpdate = _db.Addresses.FirstOrDefault(a => a.Id == address.Id);

            var addressToUpdate = _db.Addresses.Find(address.Id);
            var addressBeforeUpdate = DtoHelper.CreateAddressDto(addressToUpdate);

            try
            {
                addressToUpdate.Active = address.Active;
                addressToUpdate.AddressLineOne = address.AddressLineOne;
                addressToUpdate.AddressLineTwo = address.AddressLineTwo;
                addressToUpdate.AddressLineThree = address.AddressLineThree;
                addressToUpdate.AddressLineFour = address.AddressLineFour;
                addressToUpdate.City = address.City;
                addressToUpdate.Country = address.Country;
                addressToUpdate.RegionCounty = address.RegionCounty;
                addressToUpdate.Postcode = address.Postcode;
                addressToUpdate.Version = addressToUpdate.Version + 1;
                addressToUpdate.Level = address.Level;
                addressToUpdate.DeliveryNote = address.DeliveryNote;

                _db.SaveChanges();
                //_unitOfWork.Complete();

                _mongoMongoLogger.SuccessfulUpdateServerLog(HttpContext.Current.User, addressBeforeUpdate, addressToUpdate);

            }
            catch (Exception ex)
            {
                _mongoMongoLogger.FailedUpdateServerLog<Exception, object, object>(HttpContext.Current.User, ex.Message, ex.InnerException, address);
            }

        }

        public IEnumerable<Address> GetAddressPageBySearchTerm(string searchTerm, int pageNumber, int pageSize)
        {
            if (pageNumber > 1)
            {
                return _db.Addresses
                    .Where(a => a.Active && a.AddressLineOne.Contains(searchTerm) || a.AddressLineTwo.Contains(searchTerm) || a.AddressLineThree.Contains(searchTerm) || a.AddressLineFour.Contains(searchTerm) || a.Postcode.Contains(searchTerm))
                    .OrderBy(a => a.AddressLineOne)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
            }

            return _db.Addresses
                .Where(a => a.Active && a.AddressLineOne.Contains(searchTerm) || a.AddressLineTwo.Contains(searchTerm) || a.AddressLineThree.Contains(searchTerm) || a.AddressLineFour.Contains(searchTerm) || a.Postcode.Contains(searchTerm))
                .OrderBy(a => a.AddressLineOne)
                .Take(pageSize)
                .ToList();
        }

        public Address AddAddress(Address newAddress)
        {
            try
            {
                newAddress.Version = 1;
                newAddress.Country = "GBR";
                newAddress.Active = true;
                _db.Addresses.Add(newAddress);
                _db.SaveChanges();

                _mongoMongoLogger.SuccessfullyAddedServerLog(HttpContext.Current.User, DtoHelper.CreateAddressDto(newAddress));

                return newAddress;
            }
            catch (Exception ex)
            {
                _mongoMongoLogger.FailedToCreateServerLog<Exception, object>(HttpContext.Current.User, ex.Message, ex.InnerException);

                return newAddress;
            }
        }

        public HttpStatusCode DeactivateAddress(int deactivateId, int replacementId)
        {
            var toDeactivate = _db.Addresses.SingleOrDefault(pc => pc.Id == deactivateId);
            var toReplace = _db.Addresses.SingleOrDefault(pc => pc.Id == replacementId);

            if (Equals(toDeactivate, null) || Equals(toReplace, null))
                return HttpStatusCode.BadRequest;

            toDeactivate.Active = false;
            toReplace.Active = true;

            var barcodesToUpdate = _db.Glns.Where(bc => bc.AddressId == deactivateId).ToList();

            if (!Equals(barcodesToUpdate, null))
                barcodesToUpdate.ForEach(bc => bc.AddressId = replacementId);

            toDeactivate.Active = false;

            try
            {
                _db.SaveChanges();

                _mongoMongoLogger.SuccessfulUpdateServerLog(HttpContext.Current.User, DtoHelper.CreateAddressDto(toDeactivate), DtoHelper.CreateAddressDto(toReplace));

                return HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _mongoMongoLogger.FailedUpdateServerLog(HttpContext.Current.User, ex.Message, ex.InnerException, $"Attempt to deactivate Address with Id: {deactivateId}", $"Supplied replacement Address Id: {replacementId}");

                return HttpStatusCode.InternalServerError;
            }
        }

    }
}