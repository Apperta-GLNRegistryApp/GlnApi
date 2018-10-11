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
using System.Web.Http;
using GlnApi.Helpers;
using GlnApi.Models;
using GlnApi.Repository;
using GlnApi.Services;

namespace GlnApi.Controllers
{
    [DualAuthorize]
    public class AdditionalContactsController : ApiController
    {
        private IUnitOfWork _unitOfWork;
        private ILoggerHelper _logger;

        public AdditionalContactsController(IUnitOfWork unitOfWork, ILoggerHelper logger)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }
        // GET api/<controller>
        [HttpGet]
        [Route("api/gln-additional-contacts")]
        public IHttpActionResult GetAdditionalContacts()
        {
            var additionalContacts = _unitOfWork.AdditionalContacts.GetAll()
                .Select(DtoHelper.CreateAdditionalContactDto);

            return Ok(additionalContacts);
        }
        // GET api/<controller>
        [HttpPost]
        [Route("api/gln-additional-contacts-query")]
        public IHttpActionResult getAdditionalContactsByQuery([FromBody] AdditionalContactQuery filterResource)
        {
            var queryResult = _unitOfWork.AdditionalContacts.GetAdditionalContactByQuery(filterResource);

            return Ok(queryResult);
        }
        // GET api/<controller>/searchString
        [HttpGet]
        [Route("api/gln-additional-contacts/{search?}")]
        public IHttpActionResult GetAdditionalContacts(string search = "")
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                var emptyList = new List<AdditionalContactDto>();
                return Ok(emptyList);
            }

            var additionalContacts = _unitOfWork.AdditionalContacts.Find(ac => ac.Name.Contains(search) ||
                                                                               ac.TrustUsername.Contains(search) ||
                                                                               ac.System.Contains(search) ||
                                                                               ac.Email.Contains(search) ||
                                                                               ac.Role.Contains(search) ||
                                                                               ac.Telephone.Contains(search))
                .Select(DtoHelper.CreateAdditionalContactDto);

            return Ok(additionalContacts);
        }
        // GET api/<controller>/search
        [HttpGet]
        [Route("api/gln-additional-contacts/take/{take:int?}/search/{search?}")]
        public IHttpActionResult GetAdditionalContacts(string search = "", int take = 5)
        {
            if (string.IsNullOrEmpty(search))
            {
                var emptyList = new List<AdditionalContactDto>();
                return Ok(emptyList);
            }

            var additionalContacts = _unitOfWork.AdditionalContacts.Find(ac => ac.Name.Contains(search) ||
                                                                               ac.TrustUsername.Contains(search) ||
                                                                               ac.System.Contains(search) ||
                                                                               ac.Email.Contains(search) ||
                                                                               ac.Role.Contains(search) ||
                                                                               ac.Telephone.Contains(search)).Take(take)
                .Select(DtoHelper.CreateAdditionalContactDto);

            return Ok(additionalContacts);
        }
    
        // GET api/<controller>/id
        [HttpGet]
        [Route("api/gln-additional-contacts/{id:int?}")]
        public IHttpActionResult GetAdditionalContactsById(int id)
        {
            if (id <= 0)
                return BadRequest();

            var additionalContact = _unitOfWork.AdditionalContacts.FindSingle(ac => ac.Id == id);

            if(Equals(additionalContact, null))
                return BadRequest();

            return Ok(DtoHelper.CreateAdditionalContactDto(additionalContact));
        }

        // PUT api/<controller>
        [Authorize(Roles = "GLNadmin")]
        [HttpPut]
        [Route("api/gln-update-additional-contact")]
        public IHttpActionResult UpdateAdditionalContact(AdditionalContact additionalContact)
        {
            if(Equals(additionalContact, null))
                return BadRequest();

            var additionalContactToUpdate =
                _unitOfWork.AdditionalContacts.FindSingle(ac => ac.Id == additionalContact.Id);

            if (additionalContactToUpdate == null)
                return BadRequest();

            var additionalContactBeforeUpdate = DtoHelper.CreateAdditionalContactDto(additionalContact);

            try
            {
                if (ConcurrencyChecker.canSaveChanges(additionalContact.Version, additionalContactToUpdate.Version))
                {
                    additionalContactToUpdate.Name = additionalContact.Name;
                    additionalContactToUpdate.Email = additionalContact.Email;
                    additionalContactToUpdate.Telephone = additionalContact.Telephone;
                    additionalContactToUpdate.System = additionalContact.System;
                    additionalContactToUpdate.Fax = additionalContact.Fax;
                    additionalContactToUpdate.Salutation = additionalContact.Salutation;
                    additionalContactToUpdate.Version = additionalContact.Version + 1;
                    additionalContactToUpdate.Role = additionalContact.Role;
                    additionalContactToUpdate.NotificationSubscriber = additionalContact.NotificationSubscriber;
                    additionalContactToUpdate.Active = additionalContact.Active;

                    _unitOfWork.Complete();

                    _logger.SuccessfulUpdateServerLog(HttpContext.Current.User, DtoHelper.CreateAdditionalContactDto(additionalContact), DtoHelper.CreateAdditionalContactDto(additionalContactToUpdate));

                    return Ok(DtoHelper.CreateAdditionalContactDto(additionalContactToUpdate));
                }
                else
                {
                    _logger.ConcurrenyServerLog<object, object>(HttpContext.Current.User, additionalContact);

                    return Conflict();
                }
            }
            catch (Exception ex)
            {
                _logger.FailedUpdateServerLog<Exception, object, object>(HttpContext.Current.User, ex.Message, ex.InnerException, DtoHelper.CreateAdditionalContactDto(additionalContact));

                return InternalServerError();

            }

        }
        // POST api/<controller>/contact
        [Authorize(Roles = "GLNadmin")]
        [HttpPost]
        [Route("api/gln-additional-contacts/add-new-additional-contact/")]
        public IHttpActionResult AddNewAdditionalContact([FromBody] AdditionalContact newAdditionalContact)
        {
            if(Equals(newAdditionalContact, null))
                return BadRequest();

            var additionalContact = _unitOfWork.AdditionalContacts.Find(ac => ac.Email == newAdditionalContact.Email);

            if (additionalContact.Any())
                return Content(HttpStatusCode.Conflict, "Contact with same email already exists.");

            try
            {
                _unitOfWork.AdditionalContacts.Add(newAdditionalContact);
                _unitOfWork.Complete();

                _logger.SuccessfullyAddedServerLog(HttpContext.Current.User, DtoHelper.CreateAdditionalContactDto(newAdditionalContact));

                return Ok(DtoHelper.CreateAdditionalContactDto(newAdditionalContact));
            }
            catch (Exception ex)
            {
                _logger.FailedToCreateServerLog(HttpContext.Current.User, ex.Message, ex.InnerException, DtoHelper.CreateAdditionalContactDto(newAdditionalContact));

                return InternalServerError(ex);
            }
        }

        // PUT api/<controller>/contact
        [Authorize(Roles = "GLNadmin")]
        [HttpPut]
        [Route("api/gln-additional-contacts/to-deactivate-id/{deactivateId:int?}/to-replace-id/{replacementId:int?}")]
        public IHttpActionResult DeactivateAdditionalContact(int deactivateId, int replacementId)
        {
            if (deactivateId <= 0 || replacementId <= 0)
                return BadRequest();

            var additionalContactToDeactivate = _unitOfWork.AdditionalContacts.FindSingle(ac => ac.Id == deactivateId);
            var replacementAdditionalContact = _unitOfWork.AdditionalContacts.FindSingle(ac => ac.Id == replacementId);

            if (additionalContactToDeactivate == null)
                return BadRequest();

            try
            {

                additionalContactToDeactivate.Active = false;

                _unitOfWork.Complete();

                _logger.SuccessfulUpdateServerLog(HttpContext.Current.User, DtoHelper.CreateAdditionalContactDto(replacementAdditionalContact),
                    DtoHelper.CreateAdditionalContactDto(replacementAdditionalContact));

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.FailedUpdateServerLog(HttpContext.Current.User, ex.Message, ex.InnerException, $"Attempt to deactivate Additional Contact Id: {deactivateId}", $"Supplied replacement Additional Contact Id: {replacementId}");

                return InternalServerError(ex);
            }

        }

    }
}
