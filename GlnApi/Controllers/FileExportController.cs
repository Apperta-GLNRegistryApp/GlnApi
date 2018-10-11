//Global Location Number (GLN) Registry API
//Copyright (C) 2018  University Hospitals Plymouth NHS Trust
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Hosting;
using System.Web.Http;
using GlnApi.Helpers;
using GlnApi.Repository;
using GlnApi.Services;

namespace GlnApi.Controllers
{
    public class FileExportController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private ILoggerHelper _logger;
        private Export _export;

        public FileExportController(IUnitOfWork unitOfWork, ILoggerHelper logger, Export export)
        {
            _export = export;
            _logger = logger;
            _unitOfWork = unitOfWork;

        }

        // GET api/<controller>
        [HttpGet]
        [Route("api/create-national-gln-csv")]
        public IHttpActionResult GetCreateGsnRegistryCsv()
        {

            var csvExport = _export.CreateCsv();

            return Ok(csvExport);
        }


        [HttpGet]
        [Route("api/download-national-gln-csv")]
        public HttpResponseMessage DownloadFile()
        {
            try
            {
                byte[] bytes = null;
                var fileName = "GLNcsvRegistryExport.csv";

                var filePath = Path.GetFullPath(Path.Combine(HostingEnvironment.MapPath("~/GLNexport/"), "GLNcsvRegistryExport.csv"));

                var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                var br = new BinaryReader(fs);
                bytes = br.ReadBytes((int)fs.Length);
                br.Close();
                fs.Close();

                var result = new HttpResponseMessage(HttpStatusCode.OK);
                var stream = new MemoryStream(bytes);
                result.Content = new StreamContent(stream);
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("text/csv");
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = fileName
                };

                return (result);
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }
    }
}
