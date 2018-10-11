//Global Location Number (GLN) Registry API
//Copyright (C) 2018  University Hospitals Plymouth NHS Trust
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System.Web.Http;
using System.Web.Http.Cors;

namespace GlnApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            //var cors = new EnableCorsAttribute(ConfigurationManager.AppSettings["angularEndpoint"], "*", "*") { SupportsCredentials = true };
            var cors = new EnableCorsAttribute("*", "*", "*") { SupportsCredentials = true };
            config.EnableCors(cors);
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
