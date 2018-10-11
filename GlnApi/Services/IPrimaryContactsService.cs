//Global Location Number (GLN) Registry API
//Copyright (C) 2018  University Hospitals Plymouth NHS Trust
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System.Collections.Generic;
using System.Net;
using GlnApi.Models;

namespace GlnApi.Services
{
    public interface IPrimaryContactsService
    {
        HttpStatusCode AddNewPrimaryContact(PrimaryContact newContact);
        HttpStatusCode DeactivatePrimaryContact(int deactivateId, int replacementId);
        IEnumerable<PrimaryContactDto> GetActivePrimaryContacts();
        PrimaryContact GetPrimaryContactById(int id);
        PrimaryContact GetPrimaryContactEmail(string email);
        IEnumerable<PrimaryContactDto> GetPrimaryContacts();
        IEnumerable<PrimaryContactDto> GetPrimaryContacts(string search);
        IEnumerable<PrimaryContactDto> GetPrimaryContactsExcludeId(int contactId);
        HttpStatusCode UpdatePrimaryContact(PrimaryContact contact);
    }
}