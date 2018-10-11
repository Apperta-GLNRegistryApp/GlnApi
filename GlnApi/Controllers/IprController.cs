//Global Location Number (GLN) Registry API
//Copyright (C) 2018  University Hospitals Plymouth NHS Trust
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System;
using System.Web.Http;
using GlnApi.Helpers;
using GlnApi.Repository;

namespace GlnApi.Controllers
{
    public class IprController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private ILoggerHelper _logger;

        public IprController(IUnitOfWork unitOfWork, ILoggerHelper logger)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;

        }
        [HttpGet]
        public IHttpActionResult IprDetails()
        {
            var ipr = _unitOfWork.Ipr.FindSingle(i => i.Active == true);

            if (ipr == null)
                return NotFound();

            return Ok(ipr);
        }

        [HttpPut]
        [Route("api/update-ipr")]
        public IHttpActionResult IprUpdate(Ipr ipr)
        {
            if (Equals(null, ipr))
                return BadRequest();

            var findIpr = _unitOfWork.Ipr.FindSingle(i => i.Id == ipr.Id);

            if (Equals(null, findIpr))
                return NotFound();

            _unitOfWork.Ipr.UpdateIpr(ipr);

            try
            {
                _unitOfWork.Complete();
                var updatedIpr = _unitOfWork.Ipr.FindSingle(i => i.Id == ipr.Id);
                return Ok(DtoHelper.CreateIprDto(updatedIpr));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
