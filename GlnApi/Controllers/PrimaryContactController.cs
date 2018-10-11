//Global Location Number (GLN) Registry API
//Copyright (C) 2018  University Hospitals Plymouth NHS Trust
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System;
using System.Linq;
using System.Web;
using System.Web.Http;
using GlnApi.Helpers;
using GlnApi.Repository;
using GlnApi.Services;
using Microsoft.Practices.ObjectBuilder2;

namespace GlnApi.Controllers
{
    [DualAuthorize]
    public class PrimaryContactController : ApiController
    {
        private IUnitOfWork _unitOfWork;
        private ILoggerHelper _logger;

        public PrimaryContactController(IUnitOfWork unitOfWork, ILoggerHelper logger)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }
        // GET api/<controller>
        [HttpGet]
        [Route("api/gln-primary-contacts")]
        public IHttpActionResult getPrimaryContacts()
        {
            var contacts = _unitOfWork.PrimaryContacts.GetAll().Select(DtoHelper.CreatePrimaryContactDto);

            if(!contacts.Any())
                return NotFound();

            return Ok(contacts);
        }        
        // GET api/<controller>
        [HttpPost]
        [Route("api/gln-primary-contacts-query")]
        public IHttpActionResult getPrimaryContactsByQuery([FromBody] PrimaryContactQuery filterResource)
        {
            var queryResult = _unitOfWork.PrimaryContacts.GetPrimaryContactByQuery(filterResource);

            return Ok(queryResult);
        }
        // GET api/<controller>/contactId{int?}
        [HttpGet]
        [Route("api/gln-primary-contacts/exclude-primary-contact-id/{int?}")]
        public IHttpActionResult getPrimaryContactsExcludeId(int contactId)
        {
            var contacts = _unitOfWork.PrimaryContacts.Find(pc => pc.Id != contactId)
                .Select(DtoHelper.CreatePrimaryContactDto);

            if (!contacts.Any())
                return NotFound();

            return Ok(contacts);
        }
        // GET api/<controller>/search
        [HttpGet]
        [Route("api/gln-primary-contacts-search/{search?}")]
        public IHttpActionResult getActivePrimaryContacts(string search = "")
        {
            var contacts = _unitOfWork.PrimaryContacts.Find(pc => pc.Name.Contains(search) || 
                                                            pc.Email.Contains(search) ||
                                                            pc.Telephone.Contains(search) ||
                                                            pc.Function.Contains(search)).Select(DtoHelper.CreatePrimaryContactDto);
            if (!contacts.Any())
                return NotFound();

            return Ok(contacts);
        }
        // GET api/<controller>/id
        [HttpGet]
        [Route("api/gln-primary-contact-id/{id:int?}")]
        public IHttpActionResult GetPrimaryContactById(int id)
        {
            var contact = _unitOfWork.PrimaryContacts.FindSingle(pc => pc.Id == id);

            if(Equals(contact, null))
                return BadRequest();

            return Ok(DtoHelper.CreatePrimaryContactDto(contact));
        }
        // PUT api/<controller>
        [Authorize(Roles = "GLNadmin")]
        [HttpPut]
        [Route("api/gln-update-primary-contact")]
        public IHttpActionResult UpdatePrimaryContact(PrimaryContact contact)
        {
            if(Equals(contact, null))
                return BadRequest();

            var contactToUpdate = _unitOfWork.PrimaryContacts.FindSingle(pc => pc.Id == contact.Id);

            if (Equals(contactToUpdate, null))
                return BadRequest();

            var contactBeforeUpdate = DtoHelper.CreatePrimaryContactDto(contactToUpdate);

            try
            {
                contactToUpdate.Name = contact.Name;
                contactToUpdate.Email = contact.Email;
                contactToUpdate.Telephone = contact.Telephone;
                contactToUpdate.Function = contact.Function;
                contactToUpdate.Salutation = contact.Salutation;
                contactToUpdate.Fax = contact.Fax;
                contactToUpdate.Active = contact.Active;
                contactToUpdate.Version = contactToUpdate.Version + 1;

                _unitOfWork.Complete();

                _logger.SuccessfulUpdateServerLog(HttpContext.Current.User, contactBeforeUpdate, DtoHelper.CreatePrimaryContactDto(_unitOfWork.PrimaryContacts.FindSingle(pc => pc.Id == contact.Id)));

                return Ok(DtoHelper.CreatePrimaryContactDto(contactToUpdate));
            }
            catch (Exception ex)
            {
                _logger.FailedUpdateServerLog<Exception, object, object>(HttpContext.Current.User, ex.Message,
                    ex.InnerException, contact);

                return InternalServerError();
            }

        }
        // POST api/<controller>/contact
        [Authorize(Roles = "GLNadmin")]
        [HttpPost]
        [Route("api/gln-primary-contacts/add-new-primary-contact/")]
        public IHttpActionResult AddNewPrimaryContact([FromBody] PrimaryContact newContact)
        {
            if(Equals(newContact, null))
                return BadRequest();

            var contactAlreadyExists = _unitOfWork.PrimaryContacts.Find(pc => pc.Email == newContact.Email);

            if (!contactAlreadyExists.Any())
            {
                try
                {
                    _unitOfWork.PrimaryContacts.Add(newContact);
                    _unitOfWork.Complete();

                    var createdContact = _unitOfWork.PrimaryContacts.FindSingle(pc => pc.Email == newContact.Email);

                    return Ok(DtoHelper.CreatePrimaryContactDto(createdContact));
                }
                catch (Exception ex)
                {
                    _logger.FailedUpdateServerLog<Exception, object, object>(HttpContext.Current.User, ex.Message,
                        ex.InnerException, DtoHelper.CreatePrimaryContactDto(newContact));

                    return InternalServerError(ex);
                }
            }
            else
            {
                return Conflict();
            }

        }
        // PUT api/<controller>/contact
        [Authorize(Roles = "GLNadmin")]
        [HttpPut]
        [Route("api/gln-primary-contacts/to-deactivate-id/{deactivateId:int?}/to-replace-id/{replacementId:int?}")]
        public IHttpActionResult DeactivatePrimaryContact(int deactivateId, int replacementId)
        {
            if (deactivateId <= 0 || replacementId <= 0)
                return BadRequest();

            var toDeactivate = _unitOfWork.PrimaryContacts.FindSingle(pc => pc.Id == deactivateId);
            var toReplace = _unitOfWork.PrimaryContacts.FindSingle(pc => pc.Id == replacementId);

            if (Equals(toDeactivate, null) || Equals(toReplace, null))
                return BadRequest();

            toDeactivate.Active = false;
            toReplace.Active = true;

            var barcodesToUpdate = _unitOfWork.Glns.Find(gln => gln.ContactId == deactivateId);

            if (barcodesToUpdate.Any())
                barcodesToUpdate.ForEach(bc => bc.ContactId = replacementId);

            toDeactivate.Active = false;

            try
            {
                _unitOfWork.Complete();

                _logger.SuccessfulReplacementServerLog(HttpContext.Current.User, DtoHelper.CreatePrimaryContactDto(toDeactivate), DtoHelper.CreatePrimaryContactDto(toReplace));

                return Ok(DtoHelper.CreatePrimaryContactDto(toDeactivate));
            }
            catch (Exception ex)
            {
                _logger.FailedUpdateServerLog(HttpContext.Current.User, ex.Message, ex.InnerException, $"Attempt to deactivate Primary Contact Id: {deactivateId}", $"Supplied replacement Primary Contact Id: {replacementId}");

                return InternalServerError(ex);
            }
        }
    }
}
