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
using GlnApi.Models;

namespace GlnApi.Services
{
    public class PrimaryContactsService : IPrimaryContactsService
    {
        private readonly GlnRegistryDb _db;
        private readonly MongoLoggerHelper _mongoMongoLogger;

        public PrimaryContactsService(GlnRegistryDb db, MongoLoggerHelper mongoLoggerHelper)
        {
            _db = db;
            _mongoMongoLogger = mongoLoggerHelper;
        }
    
        public IEnumerable<PrimaryContactDto> GetPrimaryContacts()
        {
            var contacts = _db.PrimaryContacts.Select(DtoHelper.CreatePrimaryContactDto);

            return contacts;
        }
        public IEnumerable<PrimaryContactDto> GetActivePrimaryContacts()
        {
            var contacts = _db.PrimaryContacts.Where(pc => pc.Active).Select(DtoHelper.CreatePrimaryContactDto);

            return contacts;
        }

        public IEnumerable<PrimaryContactDto> GetPrimaryContacts(string search)
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                var emptyList = new List<PrimaryContactDto>();
                return emptyList;
            }

            var contacts = _db.PrimaryContacts
                .Where(c => c.Name.Contains(search)
                            || c.Email.Contains(search))
                .Select(DtoHelper.CreatePrimaryContactDto);

            return contacts;
        }

        public PrimaryContact GetPrimaryContactById(int id)
        {
            var contact = _db.PrimaryContacts.SingleOrDefault(c => c.Id == id);

            return contact;
        }

        public IEnumerable<PrimaryContactDto> GetPrimaryContactsExcludeId(int contactId)
        {

            var contacts = _db.PrimaryContacts.Where(c => c.Id != contactId).Select(DtoHelper.CreatePrimaryContactDto);

            return contacts;
        }

        public PrimaryContact GetPrimaryContactEmail(string email)
        {

            var contact = _db.PrimaryContacts.SingleOrDefault(c => c.Email == email);

            return contact;
        }

        public HttpStatusCode AddNewPrimaryContact(PrimaryContact newContact)
        {
            bool contactAlreadyExists = _db.PrimaryContacts.Any(c => c.Email == newContact.Email);

            if (!contactAlreadyExists)
            {
                try
                {
                    _db.PrimaryContacts.Add(newContact);
                    _db.SaveChanges();

                    return HttpStatusCode.OK;
                }
                catch (Exception ex)
                {
                    _mongoMongoLogger.FailedUpdateServerLog<Exception, object, object>(HttpContext.Current.User, ex.Message,
                        ex.InnerException, newContact);

                    return HttpStatusCode.InternalServerError;
                }
            }
            else
            {
                return HttpStatusCode.Conflict;
            }
        }

        public HttpStatusCode UpdatePrimaryContact(PrimaryContact contact)
        {
            try
            {
                var contactToUpdate = _db.PrimaryContacts.SingleOrDefault(c => c.Id == contact.Id);
                var contactBeforeUpdate = DtoHelper.CreatePrimaryContactDto(contactToUpdate);

                if (contactToUpdate != null)
                {
                    contactToUpdate.Name = contact.Name;
                    contactToUpdate.Email = contact.Email;
                    contactToUpdate.Telephone = contact.Telephone;
                    contactToUpdate.Function = contact.Function;
                    contactToUpdate.Salutation = contact.Salutation;
                    contactToUpdate.Fax = contact.Fax;
                    contactToUpdate.Active = contact.Active;
                }

                _db.SaveChanges();

                _mongoMongoLogger.SuccessfulUpdateServerLog(HttpContext.Current.User, contactBeforeUpdate, DtoHelper.CreatePrimaryContactDto(contactToUpdate));

                return HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _mongoMongoLogger.FailedUpdateServerLog<Exception, object, object>(HttpContext.Current.User, ex.Message,
                    ex.InnerException, contact);

                return HttpStatusCode.InternalServerError;
            }
        }

        public HttpStatusCode DeactivatePrimaryContact(int deactivateId, int replacementId)
        {
            var toDeactivate = _db.PrimaryContacts.SingleOrDefault(pc => pc.Id == deactivateId);
            var toReplace = _db.PrimaryContacts.SingleOrDefault(pc => pc.Id == replacementId);

            if (Equals(toDeactivate, null) || Equals(toReplace, null))
                return HttpStatusCode.BadRequest;

            toDeactivate.Active = false;
            toReplace.Active = true;

            var barcodesToUpdate = _db.Glns.Where(bc => bc.ContactId == deactivateId).ToList();

            if (!Equals(barcodesToUpdate, null))
                barcodesToUpdate.ForEach(bc => bc.ContactId = replacementId);

            toDeactivate.Active = false;

            try
            {
                _db.SaveChanges();

                _mongoMongoLogger.SuccessfulUpdateServerLog(HttpContext.Current.User, DtoHelper.CreatePrimaryContactDto(toDeactivate), DtoHelper.CreatePrimaryContactDto(toReplace));

                return HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _mongoMongoLogger.FailedUpdateServerLog(HttpContext.Current.User, ex.Message, ex.InnerException, $"Attempt to deactivate Primary Contact Id: {deactivateId}", $"Supplied replacement Primary Contact Id: {replacementId}");

                return HttpStatusCode.InternalServerError;
            }
        }
    }
}