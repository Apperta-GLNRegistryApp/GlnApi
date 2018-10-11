//Global Location Number (GLN) Registry API
//Copyright (C) 2018  University Hospitals Plymouth NHS Trust
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using GlnApi.DTOs;
using GlnApi.Helpers;
using GlnApi.Repository;
using GlnApi.Services;

namespace GlnApi.Controllers
{
    [AllowAnonymous]
    public class GlnMobileController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private ILoggerHelper _logger;

        public GlnMobileController(IUnitOfWork unitOfWork, ILoggerHelper logger)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        // GET api/<controller>/id
        [HttpGet]
        [Route("api/gln-mobile-id/{id:int?}")]
        public IHttpActionResult GetGlnById(int id)
        {

            var gln = _unitOfWork.Glns.Get(id);

            if (Equals(gln, null))
            {
                return BadRequest();
            }
            else
            {
                GlnDto mappedGln = DtoHelper.CreateGlnIncludeChildrenDto(gln);
                return Ok(mappedGln);
            }
        }

        // GET api/<controller>/id
        [HttpGet]
        [Route("api/gln-mobile-by-gln/{gln?}")]
        public IHttpActionResult GetGlnByGln(string gln)
        {

            var foundGln = _unitOfWork.Glns.GetGlnsByGln(gln);

            if (Equals(foundGln, null))
            {
                return NotFound();
            }
            else
            {
                GlnDto mappedGln = DtoHelper.CreateGlnIncludeChildrenDto(foundGln);
                return Ok(mappedGln);
            }
        }

        // GET api/<controller>/search
        [HttpGet]
        [Route("api/gln-mobile-search/{search?}")]
        public IHttpActionResult GetGlns(string search = "")
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                List<GlnDto> emptyList = new List<GlnDto>();
                return Ok(emptyList);
            }

            IEnumerable<GlnDto> glns = _unitOfWork.Glns.Find(bc => bc.Assigned
                                                                   && bc.FriendlyDescriptionPurpose.Contains(search)
                                                                   || bc.OwnGln.StartsWith(search))
                .OrderBy(bc => bc.FriendlyDescriptionPurpose)
                .ToList()
                .Select(DtoHelper.CreateGlnIncludeChildrenDto);

            return Ok(glns);
        }

        // GET api/<controller>/search
        [HttpGet]
        [Route("api/gln-mobile-search/take/{take:int?}/search/{search?}")]
        public IHttpActionResult GetGlns(string search = "empty", int take = 5)
        {
            if (string.IsNullOrWhiteSpace(search) || Equals(search, "empty"))
            {
                List<GlnDto> emptyList = new List<GlnDto>();
                return Ok(emptyList);
            }


            IEnumerable<GlnDto> glns = _unitOfWork.Glns.Find(bc => bc.Assigned
                                                                   && bc.FriendlyDescriptionPurpose.Contains(search)
                                                                   || bc.OwnGln.StartsWith(search))
                .OrderBy(bc => bc.FriendlyDescriptionPurpose)
                .Take(take)
                .ToList()
                .Select(DtoHelper.CreateGlnIncludeChildrenDto);

            return Ok(glns);
        }

        // PUT api/<controller>
        [HttpPut]
        [Route("api/gln-mobile-update")]
        public IHttpActionResult MobileUpdateGln(Gln gln)
        {
            if (Equals(gln, null))
                return BadRequest();

            var glnToUpdate = _unitOfWork.Glns.FindSingle(g => g.Id == gln.Id);
            var glnBeforeUpdate = DtoHelper.CreateGlnIncludeChildrenDto(glnToUpdate);

            if (Equals(glnToUpdate, null))
                return BadRequest();

            var currentDbVersion = glnToUpdate.Version;

            glnToUpdate.FriendlyDescriptionPurpose = gln.FriendlyDescriptionPurpose;

            if (!ConcurrencyChecker.canSaveChanges(gln.Version, currentDbVersion))
            {
                _logger.ConcurrenyServerLog(HttpContext.Current.User, gln.Version, currentDbVersion);
                return Conflict();
            }

            glnToUpdate.Version = currentDbVersion + 1;

            try
            {
                _unitOfWork.Complete();

                var completed = _unitOfWork.Glns.FindSingle(g => g.Id == gln.Id);

                _logger.SuccessfulUpdateServerLog(HttpContext.Current.User, glnBeforeUpdate, DtoHelper.CreateGlnIncludeChildrenDto(completed));

                return Ok(DtoHelper.CreateGlnIncludeChildrenDto(completed));
            }
            catch (Exception ex)
            {
                _logger.FailedUpdateServerLog(HttpContext.Current.User, ex.Message, ex.InnerException, DtoHelper.CreateGlnIncludeChildrenDto(glnToUpdate), DtoHelper.CreateGlnIncludeChildrenDto(glnToUpdate));
                return InternalServerError();
            }
        }
        // PUT api/<controller>
        [HttpPut]
        [Route("api/gln-mobile-site-visited")]
        public IHttpActionResult LogPharmacyVisit(Gln gln)
        {
            if (Equals(gln, null))
                return BadRequest();

            var glnToUpdate = _unitOfWork.Glns.FindSingle(g => g.Id == gln.Id);
            var glnBeforeUpdate = DtoHelper.CreateGlnIncludeChildrenDto(glnToUpdate);

            if (Equals(glnToUpdate, null))
                return BadRequest();

            try
            {
                _logger.SiteVisitedLog(HttpContext.Current.User, glnBeforeUpdate);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.FailedUpdateServerLog(HttpContext.Current.User, ex.Message, ex.InnerException, DtoHelper.CreateGlnIncludeChildrenDto(glnToUpdate), DtoHelper.CreateGlnIncludeChildrenDto(glnToUpdate));
                return InternalServerError();
            }
        }

    }
}
