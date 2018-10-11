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
    public class AdditionalContactRepository : Repository<AdditionalContact>, IAdditionalContactRepository
    {

        public AdditionalContactRepository(DbContext context) : base(context)
        {

        }

        public GlnRegistryDb GLNdbDiagramContainer
        {
            get { return _context as GlnRegistryDb; }
        }
        public QueryResult<AdditionalContactDto> GetAdditionalContactByQuery(AdditionalContactQuery queryObj)
        {
            var result = new QueryResult<AdditionalContactDto>();

            var query = GLNdbDiagramContainer.AdditionalContacts.AsQueryable();

            query = query.ApplyAdditionalContactFiltering(queryObj);

            var columnsMap = new Dictionary<string, Expression<Func<AdditionalContact, object>>>()
            {
                ["name"] = pc => pc.Name,
                ["trustUsername"] = pc => pc.TrustUsername,
                ["system"] = pc => pc.System,
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

            result.Items = query.Select(DtoHelper.CreateAdditionalContactDto);
            result.CurrentPage = queryObj.Page;

            return result;
        }
    }
}