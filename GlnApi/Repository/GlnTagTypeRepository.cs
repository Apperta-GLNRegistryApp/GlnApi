//Global Location Number (GLN) Registry API
//Copyright (C) 2018  University Hospitals Plymouth NHS Trust
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using GlnApi.Extensions;
using GlnApi.DTOs;
using GlnApi.Helpers;
using GlnApi.Models;

namespace GlnApi.Repository
{
    public class GlnTagTypeRepository : Repository<GlnTagType>, IGlnTagTypeRepository
    {
        public GlnTagTypeRepository(DbContext context) : base(context)
        {
        }

        public GlnRegistryDb GLNdbDiagramContainer
        {
            get { return _context as GlnRegistryDb; }
        }

        public QueryResult<GlnTagTypeDto> GetTagTypesByQuery(TagTypeQuery queryObj)
        {
            var result = new QueryResult<GlnTagTypeDto>();

            var query = GLNdbDiagramContainer.TagTypes.AsQueryable();

            query = query.ApplyTagTypeFiltering(queryObj);

            var columnsMap = new Dictionary<string, Expression<Func<GlnTagType, object>>>()
            {
                ["code"] = t => t.Code,
                ["description"] = t => t.Description
            };

            if (string.IsNullOrWhiteSpace(queryObj.SortBy))
                queryObj.SortBy = "description";

            query = query.ApplyingOrdering(queryObj, columnsMap);

            result.TotalItems = query.Count();
            result.TotalPages = Math.Ceiling((double)result.TotalItems / queryObj.PageSize);

            query = query.ApplyPaging(queryObj);

            result.Items = query.Select(DtoHelper.CreateGlnTagTypeDto);
            result.CurrentPage = queryObj.Page;

            return result;
        }
    }
}