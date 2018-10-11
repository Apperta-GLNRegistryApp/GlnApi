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
    public class AddressRepository : Repository<Address>, IAddressRepository
    {
        public AddressRepository(DbContext context) : base(context)
        {
            
        }

        public GlnRegistryDb GLNdbDiagramContainer
        {
            get { return _context as GlnRegistryDb; }
        }

        public IEnumerable<Address> GetAddressPageBySearchTerm(string searchTerm, int pageNumber, int pageSize)
        {
            if (pageNumber > 1)
            {
                return GLNdbDiagramContainer.Addresses.Where(a => a.Active &&
                                                                 a.AddressLineOne.Contains(searchTerm) ||
                                                                 a.AddressLineTwo.Contains(searchTerm) ||
                                                                 a.AddressLineThree.Contains(searchTerm) ||
                                                                 a.AddressLineFour.Contains(searchTerm) ||
                                                                 a.Postcode.Contains(searchTerm))
                                                                .OrderBy(a => a.AddressLineOne)
                                                                .Skip((pageNumber - 1) * pageSize)
                                                                .Take(pageSize)
                                                                .ToList();
            }

            return GLNdbDiagramContainer.Addresses.Where(a => a.Active &&
                                                              a.AddressLineOne.Contains(searchTerm) ||
                                                              a.AddressLineTwo.Contains(searchTerm) ||
                                                              a.AddressLineThree.Contains(searchTerm) ||
                                                              a.AddressLineFour.Contains(searchTerm) ||
                                                              a.Postcode.Contains(searchTerm))
                                                            .OrderBy(a => a.AddressLineOne)
                                                            .Take(pageSize)
                                                            .ToList();
        }

        public QueryResult<AddressDto> GetAddressByQuery(AddressQuery queryObj)
        {
            var result = new QueryResult<AddressDto>();

            var query = GLNdbDiagramContainer.Addresses.AsQueryable();

            query = query.ApplyAddressFiltering(queryObj);

            var columnsMap = new Dictionary<string, Expression<Func<Address, object>>>()
            {
                ["addressLineOne"] = a => a.AddressLineOne,
                ["addressLineTwo"] = a => a.AddressLineTwo,
                ["addressLineThree"] = a => a.AddressLineThree,
                ["addressLineFour"] = a => a.AddressLineFour,
                ["level"] = a => a.Level
            };

            if (string.IsNullOrWhiteSpace(queryObj.SortBy))
                queryObj.SortBy = "addressLineOne";

            query = query.ApplyingOrdering(queryObj, columnsMap);

            result.TotalItems = query.Count();
            result.TotalPages = Math.Ceiling((double)result.TotalItems / queryObj.PageSize);

            query = query.ApplyPaging(queryObj);

            result.Items = query.Select(DtoHelper.CreateAddressDto);
            result.CurrentPage = queryObj.Page;

            return result;

        }
    }
}