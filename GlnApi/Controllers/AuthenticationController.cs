//Global Location Number (GLN) Registry API
//Copyright (C) 2018  University Hospitals Plymouth NHS Trust
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System.Configuration;
using System.Web;
using System.Web.Http;
using GlnApi.Models;

namespace GlnApi.Controllers
{
    [Authorize]
    public class AuthenticationController : ApiController
    {
        private readonly string ADadminGroupName = ConfigurationManager.AppSettings["customise:ActiveDirectoryAdminGroupName"];
        [HttpGet]
        public IHttpActionResult AuthenticateAdminUser()
        {
            var user = HttpContext.Current.User;

            var userDto = new UserDto()
            {
                SamAccountName = user.Identity.Name,
                AdminGroupUser = user.IsInRole(ADadminGroupName),
            };

            return Ok(userDto);
        }
    }
}
