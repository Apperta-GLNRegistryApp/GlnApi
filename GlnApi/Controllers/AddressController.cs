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
using System.Web;
using System.Web.Http;
using GlnApi.Helpers;
using GlnApi.Models;
using GlnApi.Repository;
using GlnApi.Services;

namespace GlnApi.Controllers
{
    [DualAuthorize]
    public class AddressController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private ILoggerHelper _logger;

        public AddressController(IUnitOfWork unitOfWork,
            ILoggerHelper logger)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        // Get api/<controller>
        [HttpPost]
        [Route("api/get-address-query")]
        public IHttpActionResult GetAddressByQuery ([FromBody] AddressQuery filterResource)
        {
            var queryResult = _unitOfWork.Addresses.GetAddressByQuery(filterResource);

            return Ok(queryResult);
        }

        // GET api/<controller>
        [HttpGet]
        [Route("api/gln-address-page/page-number/{pageNumber:int}/page-size/{pageSize:int}/search-term/{searchTerm?}")]
        public IHttpActionResult GetAddressPageBySearchTerm(int pageNumber, int pageSize, string searchTerm = "")
        {
            var addresses = _unitOfWork.Addresses.GetAddressPageBySearchTerm(searchTerm, pageNumber, pageSize)
                .Select(DtoHelper.CreateAddressDto);

            if (!addresses.Any())
                addresses = new List<AddressDto>();

            return Ok(addresses);

        }
        // GET api/<controller>
        [HttpGet]
        [Route("api/gln-address/{id:int}")]
        public IHttpActionResult GetAddress(int id)
        {
            var address = _unitOfWork.Addresses.FindSingle(a => a.Id == id);

            if(Equals(address, null))
                return NotFound();

            return Ok(DtoHelper.CreateAddressDto(address));

        }        
        // GET api/<controller>
        [HttpGet]
        [Route("api/gln-address/{gln}")]
        public IHttpActionResult GetAddress(string gln)
        {
            var glnToFind = _unitOfWork.Glns.FindSingle(g => g.OwnGln == gln);

            var address = _unitOfWork.Addresses.FindSingle(a => a.Id == glnToFind.AddressId);

            if (Equals(address, null))
                return NotFound();

            return Ok(DtoHelper.CreateAddressDto(address));

        }

        // PUT: api/Address
        [Authorize(Roles = "GLNadmin")]
        [HttpPut]
        [Route("api/gln-update-address")]
        public IHttpActionResult UpdateAddress(Address address)
        {
            if (Equals(address, null))
                return BadRequest();

            var addressToUpdate = _unitOfWork.Addresses.FindSingle(a => a.Id == address.Id);

            if (Equals(addressToUpdate, null))
                return NotFound();

            if (!ConcurrencyChecker.canSaveChanges(address.Version, addressToUpdate.Version))
            {
                _logger.ConcurrenyServerLog<object, object>(HttpContext.Current.User, address);
                return Conflict();
            }

            var addressBeforeUpdate = DtoHelper.CreateAddressDto(address);

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

                _unitOfWork.Complete();

                _logger.SuccessfulUpdateServerLog(HttpContext.Current.User, addressBeforeUpdate, DtoHelper.CreateAddressDto(addressToUpdate));

            }
            catch (Exception ex)
            {
                _logger.FailedUpdateServerLog<Exception, object, object>(HttpContext.Current.User, ex.Message, ex.InnerException, DtoHelper.CreateAddressDto(address));
            }

            return Ok(DtoHelper.CreateAddressDto(addressToUpdate));
        }
        // POST: api/Address
        [Authorize(Roles = "GLNadmin")]
        [HttpPost]
        [Route("api/gln-add-address")]
        public IHttpActionResult AddAddress(Address newAddress)
        {
            if (Equals(newAddress, null))
                return BadRequest();

            try
            {
                newAddress.Version = 1;
                newAddress.Country = "GBR";
                newAddress.Active = true;
                _unitOfWork.Addresses.Add(newAddress);
                _unitOfWork.Complete();

                _logger.SuccessfullyAddedServerLog(HttpContext.Current.User, DtoHelper.CreateAddressDto(newAddress));

                return Ok(newAddress);
            }
            catch (Exception ex)
            {
                _logger.FailedToCreateServerLog<Exception, object>(HttpContext.Current.User, ex.Message, ex.InnerException);

                return InternalServerError();
            }
        }
        // PUT api/<controller>/contact
        [Authorize(Roles = "GLNadmin")]
        [HttpPut]
        [Route("api/gln-deactivate-address/to-deactivate-id/{deactivateId:int?}/to-replace-id/{replacementId:int?}")]
        public IHttpActionResult DeactivateAddress(int deactivateId, int replacementId)
        {
            if (deactivateId <= 0 || replacementId <= 0)
                return BadRequest();

            var toDeactivate = _unitOfWork.Addresses.FindSingle(a => a.Id == deactivateId);
            var toReplace = _unitOfWork.Addresses.FindSingle(a => a.Id == replacementId);

            if (Equals(toDeactivate, null) || Equals(toReplace, null))
                return BadRequest();

            toDeactivate.Active = false;
            toReplace.Active = true;

            var glnsToUpdate = _unitOfWork.Glns.Find(gln => gln.AddressId == deactivateId).ToList();

            if (!Equals(glnsToUpdate, null))
                glnsToUpdate.ForEach(bc => bc.AddressId = replacementId);

            try
            {
                _unitOfWork.Complete();

                _logger.SuccessfulUpdateServerLog(HttpContext.Current.User, DtoHelper.CreateAddressDto(toDeactivate), DtoHelper.CreateAddressDto(toReplace));

                var updatedDeactivatedAddress = _unitOfWork.Addresses.FindSingle(a => a.Id == deactivateId);

                return Ok(updatedDeactivatedAddress);
            }
            catch (Exception ex)
            {
                _logger.FailedUpdateServerLog(HttpContext.Current.User, ex.Message, ex.InnerException, $"Attempt to deactivate Address with Id: {deactivateId}", $"Supplied replacement Address Id: {replacementId}");

                return InternalServerError();
            }
        }

    }
}
