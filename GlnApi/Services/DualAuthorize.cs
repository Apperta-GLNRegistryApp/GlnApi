//Global Location Number (GLN) Registry API
//Copyright (C) 2018  University Hospitals Plymouth NHS Trust
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using IdentityModel.Client;

namespace GlnApi.Services
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class DualAuthorize : AuthorizeAttribute
    {
        protected override bool IsAuthorized(HttpActionContext httpActionContext)
        {
            if (httpActionContext == null) return false;

            //If using oAuth2
            if (httpActionContext.Request.Headers.Authorization != null && httpActionContext.Request.Headers.Authorization.Scheme.ToUpper() == "BEARER") return CheckToken(httpActionContext.Request.Headers.Authorization.Parameter).Result;

            //Revert back to windows auth
            return base.IsAuthorized(httpActionContext);
        }

        private async Task<bool> CheckToken(string token)
        {
            try
            {
                var introspectUrl = ConfigurationManager.AppSettings["identityServerIntrospectUrl"];
                var apiName = ConfigurationManager.AppSettings["identityServerAPIName"];
                var apiSecret = ConfigurationManager.AppSettings["identityServerAPISecret"];
                var introspectionClient = new IntrospectionClient(introspectUrl, apiName, apiSecret);
                var response = introspectionClient.SendAsync(new IntrospectionRequest { Token = token }).Result;
                return response.IsActive;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
