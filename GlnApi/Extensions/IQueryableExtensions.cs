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
using GlnApi.Models;
using GlnApi.Repository;

namespace GlnApi.Extensions
{
    public static class IQueryableExtensions
    {
        private static List<Gln> GetAllGlnsStartingFromSpecifiedParent(List<Gln> query, GlnQuery queryObj)
        {
            var newListGln = query.ToList();

            foreach (var child in newListGln)
            {
                var children = query.Where(gln => gln.ParentGln == queryObj.ParentGln).ToList();

                if (children.Any())
                {
                    newListGln.AddRange(children);
                }
            }

            return newListGln;
        }
        public static IQueryable<Address> ApplyAddressFiltering(this IQueryable<Address> query, AddressQuery queryObj)
        {
            if (queryObj.Active)
            {
                query = query.Where(a => a.Active);
            }
            else
            {
                query = query.Where(a => a.Active == false);
            }


            if (!string.IsNullOrWhiteSpace(queryObj.SearchTerm))
            {
                query.Where(a => a.AddressLineOne.Contains(queryObj.SearchTerm) ||
                                         a.AddressLineTwo.Contains(queryObj.SearchTerm) ||
                                         a.AddressLineThree.Contains(queryObj.SearchTerm) ||
                                         a.AddressLineFour.Contains(queryObj.SearchTerm) ||
                                         a.Postcode.Contains(queryObj.SearchTerm));
            }

            if (queryObj.Level > 0)
            {
                query = query.Where(a => a.Level == queryObj.Level);
            }

            return query;
        }
        public static IQueryable<Gln> ApplyGlnFiltering(this IQueryable<Gln> query, GlnQuery queryObj)
        {

            // If parent GLN supplied only return children of parent
            if (!string.IsNullOrWhiteSpace(queryObj.ParentGln) && string.IsNullOrWhiteSpace(queryObj.SearchTerm))
                query = query.Where(gln => gln.ParentGln == queryObj.ParentGln);

            if (queryObj.MatchAllTypes)
            {
                query = MatchAllTypesFilter(query, queryObj);
            }
            else
            {
                if (queryObj.Physical || queryObj.Functional || queryObj.Legal || queryObj.Digital)
                {
                    var physicalIds = PhysicalIdsFilter(query, queryObj);
                    var functionalIds = FunctionalIdsFilter(query, queryObj);
                    var legalIds = LegalIdsFilter(query, queryObj);
                    var digitalIds = DigitalIdsFilter(query, queryObj);
                    // TODO: union all id and then query

                    var combinedIds = physicalIds.Union(functionalIds);
                    combinedIds = combinedIds.Union(legalIds);
                    combinedIds = combinedIds.Union(digitalIds);

                    query = query.Where(g => combinedIds.Contains(g.Id));
                }
            }

            query = ApplyStatusFilter(query, queryObj);
            query = PublicPrivateFilter(query, queryObj);

            return query;
        }
        public static IQueryable<Gln> ApplyStatusFilter(this IQueryable<Gln> query, GlnQuery queryObj)
        {
            if (!queryObj.AllStatus)
            {
                query = query.Where(gln => gln.TrustActive == queryObj.TrustActive);
            }

            return query;
        }
        // Get gln physical type ids
        private static List<int> PhysicalIdsFilter(this IQueryable<Gln> query, GlnQuery queryObj)
        {
            if (!queryObj.Physical)
                return new List<int>();

            var glnIds = query.Where(g => g.PhysicalType).Select(g => g.Id).ToList();

            return glnIds;
        }
        // Get gln functional type ids
        private static List<int> FunctionalIdsFilter(this IQueryable<Gln> query, GlnQuery queryObj)
        {
            if (!queryObj.Functional)
                return new List<int>();

            var glnIds = query.Where(g => g.FunctionalType).Select(g => g.Id).ToList();

            return glnIds;
        }
        // Get gln legal type ids
        private static List<int> LegalIdsFilter(this IQueryable<Gln> query, GlnQuery queryObj)
        {
            if (!queryObj.Legal)
                return new List<int>();

            var glnIds = query.Where(g => g.LegalType).Select(g => g.Id).ToList();

            return glnIds;
        }
        // Get gln digital type ids
        private static List<int> DigitalIdsFilter(this IQueryable<Gln> query, GlnQuery queryObj)
        {
            if (!queryObj.Digital)
                return new List<int>();

            var glnIds = query.Where(g => g.DigitalType).Select(g => g.Id).ToList();

            return glnIds;
        }

        public static IQueryable<Gln> PublicPrivateFilter(this IQueryable<Gln> query, GlnQuery queryObj)
        {
            // If public and private are the same no filter applied
            if ((queryObj.Public && !queryObj.Private) || (!queryObj.Public && queryObj.Private))
            {
                query = query.Where(gln => gln.Public == queryObj.Public);
            }

            return query;
        }

        public static IQueryable<Gln> MatchAllTypesFilter(this IQueryable<Gln> query, GlnQuery queryObj)
        {
            if (queryObj.MatchAllTypes)
            {
                query = query.Where(gln => gln.PhysicalType == queryObj.Physical &&
                                    gln.FunctionalType == queryObj.Functional &&
                                    gln.DigitalType == queryObj.Digital &&
                                    gln.LegalType == queryObj.Legal);
            }

            return query;
        }

        public static IQueryable<PrimaryContact> ApplyPrimaryContactFiltering(this IQueryable<PrimaryContact> query, PrimaryContactQuery queryObj)
        {
            if (queryObj.Active)
            {
                query = query.Where(pc => pc.Active);
            }
            else
            {
                query = query.Where(pc => pc.Active == false);
            }


            if (!string.IsNullOrWhiteSpace(queryObj.SearchTerm))
            {
                query = query.Where(pc => pc.Name.Contains(queryObj.SearchTerm) ||
                                          pc.Email.Contains(queryObj.SearchTerm) ||
                                          pc.Function.Contains(queryObj.SearchTerm) ||
                                          pc.Telephone.Contains(queryObj.SearchTerm));
            }

            return query;
        }
        public static IQueryable<AdditionalContact> ApplyAdditionalContactFiltering(this IQueryable<AdditionalContact> query, AdditionalContactQuery queryObj)
        {
            if (queryObj.Active)
            {
                query = query.Where(ac => ac.Active);
            }
            else
            {
                query = query.Where(ac => ac.Active == false);
            }


            if (!string.IsNullOrWhiteSpace(queryObj.SearchTerm))
            {
                query = query.Where(ac => ac.Name.Contains(queryObj.SearchTerm) ||
                                          ac.TrustUsername.Contains(queryObj.SearchTerm) ||
                                          ac.System.Contains(queryObj.SearchTerm) ||
                                          ac.Email.Contains(queryObj.SearchTerm) ||
                                          ac.Role.Contains(queryObj.SearchTerm) ||
                                          ac.Telephone.Contains(queryObj.SearchTerm));
            }

            return query;
        }
        public static IQueryable<GlnTagType> ApplyTagTypeFiltering(this IQueryable<GlnTagType> query, TagTypeQuery queryObj)
        {
            if (queryObj.Active)
            {
                query = query.Where(ac => ac.Active);
            }
            else
            {
                query = query.Where(ac => ac.Active == false);
            }

            if (!string.IsNullOrWhiteSpace(queryObj.SearchTerm))
            {
                query = query.Where(t => t.Description.Contains(queryObj.SearchTerm) || 
                                        t.Code.Contains(queryObj.SearchTerm));
            }

            return query;
        }

        public static IQueryable<T> ApplyingOrdering<T>(this IQueryable<T> query, IQueryObject queryObj,
            Dictionary<string, Expression<Func<T, object>>> columnsMap)
        {
            if (string.IsNullOrWhiteSpace(queryObj.SortBy) || !columnsMap.ContainsKey(queryObj.SortBy))
                return query;

            if (queryObj.IsSortAscending)
            {
                return query.OrderBy(columnsMap[queryObj.SortBy]);
            }
            else
            {
                return query.OrderByDescending(columnsMap[queryObj.SortBy]);
            }

        }

        public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, IQueryObject queryObj)
        {
            if (queryObj.Page <= 0)
                queryObj.Page = 1;

            if (queryObj.PageSize <= 0)
                queryObj.PageSize = 10;

            return query.Skip((queryObj.Page - 1) * queryObj.PageSize).Take(queryObj.PageSize);
        }
    }
}