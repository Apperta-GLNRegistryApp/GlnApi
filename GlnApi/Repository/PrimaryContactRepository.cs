//Global Location Number (GLN) Registry API
//Copyright (C) 2018  University Hospitals Plymouth NHS Trust
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using GlnApi.Extensions;
using GlnApi.Helpers;
using GlnApi.Models;

namespace GlnApi.Repository
{
    public class PrimaryContactRepository : Repository<PrimaryContact>, IPrimaryContactRepository
    {
        public PrimaryContactRepository(DbContext context) : base(context)
        {
            
        }

        public GlnRegistryDb GlNdbDiagramContainer
        {
            get { return _context as GlnRegistryDb; }
        }

        public QueryResult<PrimaryContactDto> GetPrimaryContactByQuery(PrimaryContactQuery queryObj)
        {
            var result = new QueryResult<PrimaryContactDto>();

            var query = GlNdbDiagramContainer.PrimaryContacts.AsQueryable();

            query = query.ApplyPrimaryContactFiltering(queryObj);

            var columnsMap = new Dictionary<string, Expression<Func<PrimaryContact, object>>>()
            {
                ["name"] = pc => pc.Name,
                ["email"] = pc => pc.Email,
                ["telephone"] = pc => pc.Telephone,
                ["telephone"] = pc => pc.Telephone,
            };

            if (string.IsNullOrWhiteSpace(queryObj.SortBy))
                queryObj.SortBy = "name";

            query = query.ApplyingOrdering(queryObj, columnsMap);

            result.TotalItems = query.Count();
            result.TotalPages = Math.Ceiling((double)result.TotalItems / queryObj.PageSize);

            query = query.ApplyPaging(queryObj);

            result.Items = query.Select(DtoHelper.CreatePrimaryContactDto);
            result.CurrentPage = queryObj.Page;

            return result;

        }
    }
}