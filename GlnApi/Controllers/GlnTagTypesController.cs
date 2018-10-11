//Global Location Number (GLN) Registry API
//Copyright (C) 2018  University Hospitals Plymouth NHS Trust
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using GlnApi.DTOs;
using GlnApi.Helpers;
using GlnApi.Models;
using GlnApi.Repository;

namespace GlnApi.Controllers
{
    public class GlnTagTypesController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILoggerHelper _logger;

        public GlnTagTypesController(IUnitOfWork unitOfWork, ILoggerHelper logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        // GET: api/get/gln-tag-types
        [ResponseType(typeof(GlnTagTypeDto))]
        [Route("api/get/gln-tag-types")]
        public IHttpActionResult GetTagTypes()
        {
            return Ok(_unitOfWork.GlnTagType.GetAll().Where(tt => tt.Active).Select(DtoHelper.CreateGlnTagTypeDto));
        }

        // GET api/<controller>
        [HttpPost]
        [Route("api/get-gln-tag-type-query")]
        public IHttpActionResult GetTagTypesByQuery([FromBody] TagTypeQuery filterResource)
        {
            var queryResults = _unitOfWork.GlnTagType.GetTagTypesByQuery(filterResource);

            return Ok(queryResults);
        }

        // GET: api/gln-tag-types/id/5
        [ResponseType(typeof(GlnTagTypeDto))]
        [Route("api/get/gln-tag-types/id/{tagTypeId:int?}")]
        public IHttpActionResult GetGlnTagType(int id)
        {
            var glnTagType = _unitOfWork.GlnTagType.FindSingle(tt => tt.GlnTagTypeId == id);

            if (glnTagType == null)
                return NotFound();

            return Ok(DtoHelper.CreateGlnTagTypeDto(glnTagType));
        }

        // PUT: api/put/gln-tag-types/id/5
        [ResponseType(typeof(GlnTagTypeDto))]
        [Route("api/put/gln-tag-types/id/{glnTagType:int?}")]
        public IHttpActionResult PutGlnTagType(GlnTagTypeDto glnTagType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tagTypeToBeUpdated = _unitOfWork.GlnTagType.FindSingle(tt => tt.GlnTagTypeId == glnTagType.GlnTagTypeId);

            if (Equals(tagTypeToBeUpdated, null))
                return NotFound();

            var beforeUpdate = DtoHelper.CreateGlnTagTypeDto(tagTypeToBeUpdated);

            tagTypeToBeUpdated.Code = glnTagType.Code;
            tagTypeToBeUpdated.Description = glnTagType.Description;
            tagTypeToBeUpdated.ModifiedDateTime = DateTime.Now;
            tagTypeToBeUpdated.UserModified = HttpContext.Current.User.ToString();
            tagTypeToBeUpdated.Active = glnTagType.Active;

            _unitOfWork.GlnTagType.Update(tagTypeToBeUpdated);

            try
            {
                _unitOfWork.Complete();
                _logger.SuccessfulUpdateServerLog(HttpContext.Current.User, beforeUpdate, DtoHelper.CreateGlnTagTypeDto(tagTypeToBeUpdated));
            }
            catch (DbUpdateConcurrencyException)
            {
                _logger.FailedToCreateServerLog(HttpContext.Current.User, "Unable to create new tag type", "", DtoHelper.CreateGlnTagTypeDto(tagTypeToBeUpdated));
                return InternalServerError();
            }

            return Ok(DtoHelper.CreateGlnTagTypeDto(tagTypeToBeUpdated));
            //return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/post/gln-tag-types
        [ResponseType(typeof(GlnTagTypeDto))]
        [Route("api/post/gln-tag-types")]
        public IHttpActionResult PostGlnTagType(GlnTagTypeDto glnTagTypeDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.FailedToCreateServerLog(HttpContext.Current.User, "Unable to create new tag type, state invalid.", "", glnTagTypeDto);
                return BadRequest(ModelState);
            }

            var sameDescription = _unitOfWork.GlnTagType.Find(tt => tt.Description == glnTagTypeDto.Description);
            var sameCode = _unitOfWork.GlnTagType.Find(tt => tt.Code == glnTagTypeDto.Code);

            if (sameDescription.Any())
            {
                _logger.FailedToCreateServerLog(HttpContext.Current.User, $"A GlnTagType already exists with this decription: {glnTagTypeDto.Description}", "", glnTagTypeDto);
                return BadRequest($"A GlnTagType already exists with this decription: {glnTagTypeDto.Description}");
            }

            if (sameCode.Any())
            {
                _logger.FailedToCreateServerLog(HttpContext.Current.User, $"A GlnTagType already exists with this code: {glnTagTypeDto.Code}", "", glnTagTypeDto);
                return BadRequest($"A GlnTagType already exists with this code: {glnTagTypeDto.Code}");
            }

            var glnTagType = new GlnTagType()
            {
                Active = glnTagTypeDto.Active,
                Code = glnTagTypeDto.Code,
                CreatedDateTime = DateTime.Now,
                Description = glnTagTypeDto.Description,
                ModifiedDateTime = DateTime.Now,
                UserCreated = HttpContext.Current.User.ToString()
            };

            _unitOfWork.GlnTagType.Add(glnTagType);

            try
            {
                _unitOfWork.Complete();
                _logger.SuccessfullyAddedServerLog(HttpContext.Current.User, DtoHelper.CreateGlnTagTypeDto(glnTagType));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                _logger.FailedToCreateServerLog(HttpContext.Current.User, e.Message, e.InnerException, glnTagTypeDto);
                return InternalServerError();
            }


            return Ok(DtoHelper.CreateGlnTagTypeDto(glnTagType));
        }

        // DELETE: api/delete/gln-tag-types/id/5
        [ResponseType(typeof(GlnTagTypeDto))]
        [Route("api/delete/gln-tag-types/id/{tagTypeId:int?}")]
        public IHttpActionResult DeleteGlnTagType(int id)
        {
            GlnTagType glnTagType = _unitOfWork.GlnTagType.FindSingle(tt => tt.GlnTagTypeId == id);

            if (glnTagType == null)
            {
                return NotFound();
            }

            _unitOfWork.GlnTagType.Remove(glnTagType);
            _unitOfWork.Complete();
            _logger.SuccessfulServerLog("Tag Type deleted", HttpContext.Current.User, DtoHelper.CreateGlnTagTypeDto(glnTagType), new object());

            return Ok(DtoHelper.CreateGlnTagTypeDto(glnTagType));
        }

        private bool GlnTagTypeExists(int id)
        {
            return _unitOfWork.GlnTagType.GetAll().Count(e => e.GlnTagTypeId == id) > 0;
        }
    }
}