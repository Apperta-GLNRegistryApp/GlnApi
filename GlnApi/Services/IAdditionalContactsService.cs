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
    public interface IAdditionalContactsService
    {
        HttpStatusCode AddNewAdditionalContact(AdditionalContact newAdditionalContact);
        HttpStatusCode UpdateAdditionalContact(AdditionalContact additionalContact);
        IEnumerable<AdditionalContactDto> GetAdditionalContacts();
        IEnumerable<AdditionalContact> GetAdditionalContacts(string search, int take = 5);
        AdditionalContactDto GetAdditionalContactById(int id);
        AdditionalContactDto GetAdditionalContactByEmail(string email);
        IEnumerable<AdditionalContactDto> GetAdditionalContacts(string search);
        HttpStatusCode DeactivateAdditionalContact(int deactivateId, int? replacementId);
    }
}
