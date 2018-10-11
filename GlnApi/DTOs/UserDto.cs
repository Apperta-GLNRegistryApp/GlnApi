//Global Location Number (GLN) Registry API
//Copyright (C) 2018  University Hospitals Plymouth NHS Trust
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using GlnApi.ViewModels;

namespace GlnApi.Models
{
    public class UserDto
    {
        public string DisplayName { get; set; }
        public string GivenName { get; set; }
        public string Forename { get; set; }
        public string Surname { get; set; }
        public string SamAccountName { get; set; }
        public string JobTitle { get; set; }
        public string EmailAddress { get; set; }
        public bool AdminGroupUser { get; set; }
        public Enums.Server CurrentServer { get; set; }
    }
}