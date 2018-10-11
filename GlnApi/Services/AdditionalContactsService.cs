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
    public class AdditionalContactsService : IAdditionalContactsService
    {
        private readonly GlnRegistryDb _db;
        private readonly MongoLoggerHelper _mongoMongoLogger;

        public AdditionalContactsService(GlnRegistryDb db, MongoLoggerHelper mongoLoggerHelper)
        {
            _db = db;
            _mongoMongoLogger = mongoLoggerHelper;
        }

        public HttpStatusCode AddNewAdditionalContact(AdditionalContact newAdditionalContact)
        {
            var smAlreadyExists = _db.AdditionalContacts.Any(c => c.Email == newAdditionalContact.Email);

            if (smAlreadyExists)
                return HttpStatusCode.Conflict;

            try
            {
                _db.AdditionalContacts.Add(newAdditionalContact);

                _db.SaveChanges();

                _mongoMongoLogger.SuccessfullyAddedServerLog(HttpContext.Current.User, newAdditionalContact);

                return HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _mongoMongoLogger.FailedToCreateServerLog(HttpContext.Current.User, ex.Message, ex.InnerException, newAdditionalContact);

                return HttpStatusCode.InternalServerError;
            }
        }
        public HttpStatusCode UpdateAdditionalContact(AdditionalContact additionalContact)
        {
            var systemManagerToUpdate = _db.AdditionalContacts.SingleOrDefault(sm => sm.Id == additionalContact.Id);
            var systemManagerBeforeUpdate = DtoHelper.CreateAdditionalContactDto(systemManagerToUpdate);

            if (Equals(systemManagerToUpdate, null))
                return HttpStatusCode.BadRequest;

            try
            {
                if (ConcurrencyChecker.canSaveChanges(additionalContact.Version, systemManagerToUpdate.Version))
                {
                    systemManagerToUpdate.Name = additionalContact.Name;
                    systemManagerToUpdate.Email = additionalContact.Email;
                    systemManagerToUpdate.Telephone = additionalContact.Telephone;
                    systemManagerToUpdate.System = additionalContact.System;
                    systemManagerToUpdate.Fax = additionalContact.Fax;
                    systemManagerToUpdate.Salutation = additionalContact.Salutation;
                    systemManagerToUpdate.Version = additionalContact.Version + 1;
                    systemManagerToUpdate.Role = additionalContact.Role;
                    systemManagerToUpdate.NotificationSubscriber = additionalContact.NotificationSubscriber;
                    systemManagerToUpdate.Active = additionalContact.Active;

                    _db.SaveChanges();

                    _mongoMongoLogger.SuccessfulUpdateServerLog(HttpContext.Current.User, systemManagerBeforeUpdate, DtoHelper.CreateAdditionalContactDto(systemManagerToUpdate));

                    return HttpStatusCode.OK;
                }
                else
                {
                    _mongoMongoLogger.ConcurrenyServerLog<object, object>(HttpContext.Current.User, additionalContact);

                    return HttpStatusCode.Conflict;
                }
            }
            catch (Exception ex)
            {
                _mongoMongoLogger.FailedUpdateServerLog<Exception, object, object>(HttpContext.Current.User, ex.Message, ex.InnerException, additionalContact);

                return HttpStatusCode.InternalServerError;

            }
        }
        public IEnumerable<AdditionalContactDto> GetAdditionalContacts()
        {
            var additionalContacts = _db.AdditionalContacts.ToList()
                    .Select(DtoHelper.CreateAdditionalContactDto);

            return additionalContacts;
        }
        public IEnumerable<AdditionalContact> GetAdditionalContacts(string search, int take = 5)
        {
            var additionalContacts = _db.AdditionalContacts
                                     .Where(sm => sm.Name.StartsWith(search)
                                    || sm.System.StartsWith(search)
                                    || sm.Email.StartsWith(search))
                                    .Take(take)
                                    .ToList();

            return additionalContacts;
        }
        public AdditionalContactDto GetAdditionalContactById(int id)
        {
            var additionalContact = _db.AdditionalContacts.SingleOrDefault(sm => sm.Id == id);

            return DtoHelper.CreateAdditionalContactDto(additionalContact);
        }
        public AdditionalContactDto GetAdditionalContactByEmail(string email)
        {
            var additionalContact = _db.AdditionalContacts.SingleOrDefault(sm => sm.Email == email);

            return DtoHelper.CreateAdditionalContactDto(additionalContact);
        }

        public IEnumerable<AdditionalContactDto> GetAdditionalContacts(string search)
        {
            var additionalContacts = _db.AdditionalContacts
                    .Where(sm => sm.Name.StartsWith(search)
                                || sm.System.StartsWith(search)
                                || sm.Email.StartsWith(search))
                    .ToList()
                    .Select(DtoHelper.CreateAdditionalContactDto);

            return additionalContacts;
        }

        public HttpStatusCode DeactivateAdditionalContact(int deactivateId, int? replacementId)
        {
            var additionalContactToDeactivate = _db.AdditionalContacts.SingleOrDefault(pc => pc.Id == deactivateId);
            var replacementAdditionalContact = _db.AdditionalContacts.SingleOrDefault(pc => pc.Id == replacementId);

            if (Equals(additionalContactToDeactivate, null))
                return HttpStatusCode.BadRequest;

            try
            {

                additionalContactToDeactivate.Active = false;

                _db.SaveChanges();

                _mongoMongoLogger.SuccessfulUpdateServerLog(HttpContext.Current.User, DtoHelper.CreateAdditionalContactDto(replacementAdditionalContact), 
                                                                                DtoHelper.CreateAdditionalContactDto(replacementAdditionalContact));

                return HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _mongoMongoLogger.FailedUpdateServerLog(HttpContext.Current.User, ex.Message, ex.InnerException, $"Attempt to deactivate Additional Contact Id: {deactivateId}", $"Supplied replacement Additional Contact Id: {replacementId}");

                return HttpStatusCode.InternalServerError;
            }
        }
    }
}