//Global Location Number (GLN) Registry API
//Copyright (C) 2018  University Hospitals Plymouth NHS Trust
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System.Configuration;
using System.Web;

namespace GlnApi.Helpers
{
    public class OptionsModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.BeginRequest += (sender, args) =>
            {
                var app = (HttpApplication)sender;

                if (app.Request.HttpMethod == "OPTIONS")
                {
                    app.Response.StatusCode = 200;
                    app.Response.AddHeader("Access-Control-Allow-Headers", "content-type");
                    app.Response.AddHeader("Access-Control-Allow-Origin", ConfigurationManager.AppSettings["angularEndpoint"]);
                    //app.Response.AddHeader("Access-Control-Allow-Origin", "http://localhost:4200");
                    app.Response.AddHeader("Access-Control-Allow-Credentials", "true");
                    app.Response.AddHeader("Access-Control-Allow-Methods", "PUT, DELETE, POST, GET, OPTIONS");
                    app.Response.AddHeader("Content-Type", "application/json");
                    app.Response.End();
                }
            };
        }

        public void Dispose()
        {
        }
    }
}