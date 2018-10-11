//Global Location Number (GLN) Registry API
//Copyright (C) 2018  University Hospitals Plymouth NHS Trust
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace gln_registry_aspNet
{
    public class MultiComponentTelemetryInitializer : ITelemetryInitializer
    {
        private HttpContext _httpContextAccessor;

        public MultiComponentTelemetryInitializer(HttpContext httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void Initialize(ITelemetry telemetry)
        {
            var requestTelemetry = telemetry as RequestTelemetry;
            if (requestTelemetry?.Context?.Cloud == null) return;

            requestTelemetry.Context.Cloud.RoleName = "GlnApi";
            if (_httpContextAccessor.User.Identity.IsAuthenticated)
            {
                requestTelemetry.Context.User.Id = _httpContextAccessor.User.Identity.Name;
            }
        }
        
    }
}