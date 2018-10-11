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
    public class GlnTagsController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILoggerHelper _logger;

        public GlnTagsController(IUnitOfWork unitOfWork, ILoggerHelper logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        // GET: api/get/gln-tags
        [ResponseType(typeof(GlnTagDto))]
        [Route("api/get/gln-tags")]
        public IHttpActionResult GetTags()
        {
            return Ok(_unitOfWork.GlnTag.GetAll().Select(DtoHelper.CreateGlnTagDto));
        }

        // GET: api/get/gln-tags/id/5
        [ResponseType(typeof(GlnTagDto))]
        [Route("api/get/gln-tags/id/{id:int?}")]
        public IHttpActionResult GetGlnTag(int id)
        {
            GlnTag glnTag = _unitOfWork.GlnTag.FindSingle(tt => tt.GlnTagId == id);

            if (glnTag == null)
                return NotFound();

            return Ok(DtoHelper.CreateGlnTagDto(glnTag));
        }

        // GET: api/get/gln-tags/gln-id/5
        [ResponseType(typeof(GlnTagDto))]
        [Route("api/get/gln-tags/gln-id/{glnId:int?}")]
        public IHttpActionResult GetGlnTagsByGlnId(int glnId)
        {
            var glnTags = _unitOfWork.GlnTag.Find(tt => tt.GlnId == glnId);

            if (!glnTags.Any())
                return Ok(new List<GlnTagDto>());

            return Ok(glnTags.Select(DtoHelper.CreateGlnTagDto));
        }

        // GET: api/get/gln-tags/gln/"555555555555555"
        [ResponseType(typeof(GlnTagDto))]
        [Route("api/get/gln-tags/gln/{gln?}")]
        public IHttpActionResult GetGlnTagsByGln(string gln)
        {
            if (Equals(gln, null))
                return BadRequest();

            var glnTags = _unitOfWork.GlnTag.Find(tt => tt.TypeKey == gln);

            if (!glnTags.Any())
                return NotFound();

            return Ok(glnTags.Select(DtoHelper.CreateGlnTagDto));
        }

        // PUT: api/put/gln-tags
        [ResponseType(typeof(GlnTagDto))]
        [Route("api/put/gln-tags")]
        public IHttpActionResult PutGlnTag(GlnTag glnTag)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tagToBeUpdated = _unitOfWork.GlnTag.FindSingle(t => t.GlnTagId == glnTag.GlnTagId);

            if (Equals(tagToBeUpdated, null))
                return NotFound();

            var beforeUpdate = DtoHelper.CreateGlnTagDto(tagToBeUpdated);

            if (TagAlreadyExistOnGln(DtoHelper.CreateGlnTagDto(glnTag)))
            {
                _logger.FailedToCreateServerLog(HttpContext.Current.User, $"GLN already has this tag associated with it.", "", DtoHelper.CreateGlnTagDto(glnTag));
                return BadRequest($"GLN already has this tag associated with it.");
            }

            tagToBeUpdated.Active = glnTag.Active;
            tagToBeUpdated.GlnId = glnTag.GlnId;
            tagToBeUpdated.GlnTagTypeId = glnTag.GlnTagTypeId;
            tagToBeUpdated.TypeKey = glnTag.TypeKey;
            tagToBeUpdated.ModifiedDateTime = DateTime.Now;
            tagToBeUpdated.UserModified = HttpContext.Current.User.ToString();

            _unitOfWork.GlnTag.Update(tagToBeUpdated);

            try
            {
                _unitOfWork.Complete();
                _logger.SuccessfulUpdateServerLog(HttpContext.Current.User, beforeUpdate, DtoHelper.CreateGlnTagDto(tagToBeUpdated));

            }
            catch (DbUpdateConcurrencyException)
            {
                _logger.FailedToCreateServerLog(HttpContext.Current.User, "Unable to create new tag type", "", DtoHelper.CreateGlnTagDto(tagToBeUpdated));
                return InternalServerError();
            }

            return Ok(DtoHelper.CreateGlnTagDto(tagToBeUpdated));
        }

        // POST: api/gln-tags
        [ResponseType(typeof(GlnTagDto))]
        [Route("api/post/gln-tags")]
        public IHttpActionResult PostGlnTag(GlnTagDto glnTagDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.FailedToCreateServerLog(HttpContext.Current.User, "Unable to create new tag, state invalid.", "", glnTagDto);
                return BadRequest(ModelState);
            }

            if (TagAlreadyExistOnGln(glnTagDto))
            {
                _logger.FailedToCreateServerLog(HttpContext.Current.User, $"GLN already has this tag associated with it.", "", glnTagDto);
                return BadRequest($"GLN already has this tag associated with it.");
            }

            var glnTag = new GlnTag()
            {
                Active = true,
                CreatedDateTime = DateTime.Now,
                GlnId = glnTagDto.GlnId,
                GlnTagTypeId = glnTagDto.GlnTagTypeId,
                TypeKey = glnTagDto.TypeKey,
                UserCreated = HttpContext.Current.User.ToString()
            };

            _unitOfWork.GlnTag.Add(glnTag);

            try
            {
                _unitOfWork.Complete();
                _logger.SuccessfullyAddedServerLog(HttpContext.Current.User, DtoHelper.CreateGlnTagDto(glnTag));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                _logger.FailedToCreateServerLog(HttpContext.Current.User, e.Message, e.InnerException, glnTagDto);
                return InternalServerError();
            }

            return Ok(glnTagDto);
        }

        // DELETE: api/GlnTags/5
        [ResponseType(typeof(GlnTagDto))]
        [Route("api/delete/gln-tags/id/{id:int?}")]
        public IHttpActionResult DeleteGlnTag(int id)
        {
            GlnTag glnTag = _unitOfWork.GlnTag.FindSingle(tt => tt.GlnTagId == id);

            if (glnTag == null)
            {
                return NotFound();
            }

            _unitOfWork.GlnTag.Remove(glnTag);
            _unitOfWork.Complete();
            _logger.SuccessfulServerLog("Tag Type deleted", HttpContext.Current.User, DtoHelper.CreateGlnTagDto(glnTag), new object());

            return Ok(DtoHelper.CreateGlnTagDto(glnTag));
        }

        private bool TagAlreadyExistOnGln(GlnTagDto glnTagDto)
        {
            var alreadyOnGln =
                _unitOfWork.GlnTag.Find(t => t.GlnId == glnTagDto.GlnId && t.GlnTagTypeId == glnTagDto.GlnTagTypeId);

            if (alreadyOnGln.Any())
                return true;

            return false;
        }

        private bool GlnTagExists(int id)
        {
            return _unitOfWork.GlnTag.GetAll().Count(e => e.GlnTagId == id) > 0;
        }
    }
}