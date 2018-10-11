//Global Location Number (GLN) Registry API
//Copyright (C) 2018  University Hospitals Plymouth NHS Trust
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
using Microsoft.Practices.Unity;
using System.Data.Entity;
using System.Web.Http;
using GlnApi.Helpers;
using GlnApi.Interfaces;
using GlnApi.Repository;
using GlnApi.Services;
using Unity.WebApi;
using Microsoft.ApplicationInsights.Extensibility;
using gln_registry_aspNet;
using System.Configuration;

namespace GlnApi
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();
            var useMongo = ConfigurationManager.AppSettings["customise:useMongoLogging"].ToUpper();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();

            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
            container.RegisterType<IPrimaryContactsService, PrimaryContactsService>();
            container.RegisterType<IAdditionalContactsService, AdditionalContactsService>();
            container.RegisterType<IAddressService, AddressService>();
            container.RegisterType<IMongoHelper, MongoHelper>();
            container.RegisterType<IMongoLogger, MongoLogger>();

            if (useMongo == "TRUE")
            {
                container.RegisterType<ILoggerHelper, MongoLoggerHelper>();
            }
            else
            {
                container.RegisterType<ILoggerHelper, SqlLogger>();
            }

            container.RegisterType<IGlnRepository, GlnRepository>();
            container.RegisterType<IAddressRepository, AddressRepository>();
            container.RegisterType<IPrimaryContactRepository, PrimaryContactRepository>();
            container.RegisterType<IAdditionalContactRepository, AdditionalContactRepository>();
            container.RegisterType<IIprRepository, IprRepository>();
            container.RegisterType<IGlnTagRepository, GlnTagRepository>();
            container.RegisterType<IGlnTagTypeRepository, GlnTagTypeRepository>();
            container.RegisterType<IUnitOfWork, UnitOfWork>();
            container.RegisterType<DbContext, GlnRegistryDb>(new PerResolveLifetimeManager());

            //Application insights listener
            container.RegisterType<ITelemetryInitializer, MultiComponentTelemetryInitializer>();

            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}