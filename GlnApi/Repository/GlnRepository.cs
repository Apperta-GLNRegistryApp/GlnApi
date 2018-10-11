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
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;
using GlnApi.Extensions;
using System.Data.Entity;
using GlnApi.DTOs;
using GlnApi.Helpers;
using GlnApi.Models;

namespace GlnApi.Repository
{
    public class GlnRepository : Repository<Gln>, IGlnRepository
    {
        private List<int> ChildGlnIds;
        private IQueryable<Gln> _results;
        public GlnRepository(DbContext context) : base(context)
        {

        }
        public Gln UpdateGln(Gln sentGln)
        {
            var glnToUpdate = IGlnRegistryDb.Glns.SingleOrDefault(g => g.Id == sentGln.Id);

            glnToUpdate.Id = sentGln.Id;
            glnToUpdate.Assigned = sentGln.Assigned;
            glnToUpdate.Active = sentGln.Active;
            glnToUpdate.FriendlyDescriptionPurpose = sentGln.FriendlyDescriptionPurpose;
            glnToUpdate.FunctionalType = sentGln.FunctionalType;
            glnToUpdate.LegalType = sentGln.LegalType;
            glnToUpdate.PhysicalType = sentGln.PhysicalType;
            glnToUpdate.DigitalType = sentGln.DigitalType;
            glnToUpdate.Public = sentGln.Public;
            glnToUpdate.AddressId = sentGln.AddressId;
            glnToUpdate.ContactId = sentGln.ContactId;
            glnToUpdate.UseParentAddress = sentGln.UseParentAddress;
            glnToUpdate.Version = sentGln.Version + 1;
            glnToUpdate.TrustActive = sentGln.TrustActive;
            glnToUpdate.SuspensionReason = sentGln.SuspensionReason;
            glnToUpdate.SuspensionDate = sentGln.SuspensionDate;
            glnToUpdate.ParentGln = sentGln.ParentGln;
            glnToUpdate.Department = sentGln.Department;
            glnToUpdate.Function = sentGln.Function;
            glnToUpdate.DeliveryNote = sentGln.DeliveryNote;
            glnToUpdate.NumberOfChildren = glnToUpdate.Children.Count;

            var parent = IGlnRegistryDb.Glns.SingleOrDefault(gln => gln.OwnGln == glnToUpdate.ParentGln);

            if (parent != null)
            {
                glnToUpdate.ParentDescriptionPurpose = parent.TruthDescriptionPurpose;
                glnToUpdate.ParentId = parent.Id;

                if (parent.TierLevel >= 5)
                {
                    glnToUpdate.TierLevel = parent.TierLevel;
                }
                else
                {
                    glnToUpdate.TierLevel = parent.TierLevel + 1;
                }
 
            }

            if (!glnToUpdate.TrustActive)
            {
                if (HttpContext.Current != null)
                {
                    var suspendedBy = HttpContext.Current.User.Identity.Name;
                    glnToUpdate.SuspendedBy = suspendedBy;
                }
                else
                {
                    glnToUpdate.SuspendedBy = "UNKNOWN";
                }
            }

            if (!glnToUpdate.AlternativeSystemIsTruth && glnToUpdate.Assigned)
                glnToUpdate.TruthDescriptionPurpose = glnToUpdate.FriendlyDescriptionPurpose;

            if (!glnToUpdate.Active)
            {
                glnToUpdate.TrustActive = false;
                glnToUpdate.SuspensionReason = "National deactivated";
                glnToUpdate.SuspensionDate = DateTime.Now;
            }

            glnToUpdate.LastUpdate = DateTime.Now;

            return glnToUpdate;
        }

        public IEnumerable<Gln> GetGlnAssociations(int glnId)
        {
            var glnAssociations = new List<Gln>();

            var associations =
                IGlnRegistryDb.GlnAssociations.Where(a => a.GlnId1 == glnId || a.GlnId2 == glnId).ToList();

            if(Equals(associations, null) || Equals(associations.Count, 0))
                return glnAssociations;

            foreach (var a in associations)
            {
                // Add to return list if not passed in Id, don't want to return itself
                if (!Equals(a.GlnId1, glnId))
                {
                    var gln = IGlnRegistryDb.Glns.Find(a.GlnId1);
                    glnAssociations.Add(gln);
                }
                // Add to return list if not passed in Id, don't want to return itself
                if (!Equals(a.GlnId2, glnId))
                {
                    var gln = IGlnRegistryDb.Glns.Find(a.GlnId2);
                    glnAssociations.Add(gln);
                }
            }

            return glnAssociations;
        }

        public void AssignNewGlnAssociation(int glnId1, int glnId2)
        {
            var associations =
                IGlnRegistryDb.GlnAssociations.Where(id => id.GlnId1 == glnId1 
                                                        && id.GlnId2 == glnId2 
                                                        || id.GlnId1 == glnId2
                                                        && id.GlnId2 == glnId1).ToList();

            // If an association either way doesn't already exists then add, don't want to create duplicate associations
            if (!associations.Any())
            {
                var ass = new GlnAssociation() { GlnId1 = glnId1, GlnId2 = glnId2 };
                IGlnRegistryDb.GlnAssociations.Add(ass);
            }
        }

        public void RemoveGlnAssociation(int glnId1, int glnId2)
        {
            var associations =
                IGlnRegistryDb.GlnAssociations.Where(id => id.GlnId1 == glnId1 && id.GlnId2 == glnId2 ||
                                                                  id.GlnId1 == glnId2 && id.GlnId2 == glnId1).ToList();

            foreach (var glnAssociation in associations)
            {
                IGlnRegistryDb.GlnAssociations.Remove(glnAssociation);
            }
        }

        public void RemoveAllGlnAssociation(int glnId)
        {
            var associations =
                IGlnRegistryDb.GlnAssociations.Where(id => id.GlnId1 == glnId || id.GlnId2 == glnId).ToList();

            foreach (var glnAssociation in associations)
            {
                IGlnRegistryDb.GlnAssociations.Remove(glnAssociation);
            }
        }
        public Gln GetGlnsByGln(string gln)
        {
            var glnsFound = IGlnRegistryDb.Glns.SingleOrDefault(bc => bc.OwnGln == gln);

            return glnsFound;
        }

        public IGlnRegistryDb IGlnRegistryDb
        {
            get { return _context as IGlnRegistryDb; }
        }

        private void TraverseChildren(Gln gln)
        {
            Parallel.ForEach(gln.Children, child =>
            {
                
                if (!Equals(child, null))
                {
                    child.Id = child.Id;

                    ChildGlnIds.Add(child.Id);

                    TraverseChildren(child);
                }
            });
        }
        public List<int> GetGlnIdsForEntireFamily(int parentId, List<int> previousList)
        {
            var parent = IGlnRegistryDb.Glns.SingleOrDefault(gln => gln.Id == parentId && gln.Assigned);

            if (!Equals(parent, null))
            {
                var childrenIds = parent.Children.Select(g => g.Id).ToList();

                if (childrenIds.Any())
                {
                    previousList.AddRange(childrenIds);

                    foreach (var id in childrenIds)
                    {
                        previousList = GetGlnIdsForEntireFamily(id, previousList);
                    }
                }
            }
            return previousList;
        }
        public List<int> GetGlnIdsOfParentsChildren(int parentId)
        {
            var parent = IGlnRegistryDb.Glns.SingleOrDefault(gln => gln.Id == parentId && gln.Assigned);

            if (parent != null)
            {
                var childrenIds = parent.Children.Where(c => c.Assigned).Select(g => g.Id).ToList();

                if (childrenIds.Any())
                    return childrenIds;
            }

            return new List<int>();
        }

        // Get tag ids
        public List<int> GetGlnIdsFromTagIds(List<int> tagIds)
        {
            if (!tagIds.Any() || Equals(tagIds, null))
                return null;

            var glnIds = IGlnRegistryDb.Tags.Where(t => tagIds.Contains(t.GlnTagTypeId)).Select(g => g.GlnId).ToList();

            return glnIds;
        }

        private IQueryable<Gln> GetGlnByTagTypes(IQueryable<Gln> query, List<int> tagIds)
        {
            if (tagIds.All(tt => tt == 0))
                return query;

            var glnIdsFromTagIds = GetGlnIdsFromTagIds(tagIds);

            query = query.Where(g => glnIdsFromTagIds.Contains(g.Id));

            return query;
        }

        public QueryResult<GlnDto> GetGlnsByQuery(GlnQuery queryObj)
        {
            queryObj = ConfigureForSearchTerm(queryObj);

            var query = IGlnRegistryDb.Glns.AsQueryable();
            query = query.Where(g => g.Assigned);

            var result = new QueryResult<GlnDto>();

            if (!queryObj.ChildrenOnly)
            {
                query = GetGlnsByQueryChildrenOnly(query, queryObj);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(queryObj.SearchTerm))
                {
                    query = GetGlnsByQueryChildrenOnly(query, queryObj);
                }
                else
                {
                    query = GetGlnsByQueryAll(query, queryObj);
                }
            }

            if (!Equals(queryObj.TagTypeIds, null))
                query = GetGlnByTagTypes(query, queryObj.TagTypeIds);

            query = query.ApplyGlnFiltering(queryObj);

            var columnsMap = new Dictionary<string, Expression<Func<Gln, object>>>()
            {
                ["friendlyDescriptionPurpose"] = gln => gln.FriendlyDescriptionPurpose,
                ["parentDescription"] = gln => gln.ParentDescriptionPurpose,
                ["addressLineTwo"] = gln => gln.Address.AddressLineTwo,
                ["department"] = gln => gln.Department,
                ["function"] = gln => gln.Function,
                ["level"] = gln => gln.TierLevel,

            };

            query = query.ApplyingOrdering(queryObj, columnsMap);

            result.TotalItems = query.Count();

            if (string.IsNullOrWhiteSpace(queryObj.SearchTerm))
            {
                query = query.ApplyPaging(queryObj);
                result.CurrentPage = queryObj.Page;
                result.TotalPages = Math.Ceiling((double)result.TotalItems / queryObj.PageSize);
            }
            else
            {
                result.CurrentPage = 1;
                result.TotalPages = 1;
            }

            result.Items = query.Select(DtoHelper.CreateGlnIncludeChildrenDto);

            return result;
        }

        private GlnQuery ConfigureForSearchTerm(GlnQuery queryObj)
        {
            if (!string.IsNullOrWhiteSpace(queryObj.SearchTerm))
            {
                queryObj.SortBy = "parentDescription";
            }
            else
            {
                queryObj.SortBy = "friendlyDescriptionPurpose";
            }

            return queryObj;
        }

        public IQueryable<Gln> GetGlnsByQueryChildrenOnly(IQueryable<Gln> query, GlnQuery queryObj)
        {
            var getParent = IGlnRegistryDb.Glns.Include(p => p.Children)
                .FirstOrDefault(p => p.OwnGln == queryObj.ParentGln);

            if (!Equals(getParent, null))
            {
                ChildGlnIds = GetGlnIdsOfParentsChildren(getParent.Id);
                query = query.Where(g => ChildGlnIds.Contains(g.Id));

                if (!string.IsNullOrWhiteSpace(queryObj.SearchTerm))
                {
                    if (SearchTermIsGlnNumber(queryObj))
                    {
                        query = query.Where(g => ChildGlnIds.Contains(g.Id) && g.Assigned &&
                                                 (g.OwnGln.Contains(queryObj.SearchTerm)));
                    }
                    else
                    {
                        query = query.Where(g => g.Assigned &&
                                         (g.FriendlyDescriptionPurpose.Contains(queryObj.SearchTerm) ||
                                          g.TruthDescriptionPurpose.Contains(queryObj.SearchTerm)));
                    }
                }
            }

            return query;
        }

        public IQueryable<Gln> GetGlnsByQueryAll(IQueryable<Gln> query, GlnQuery queryObj)
        {
            var getParent = IGlnRegistryDb.Glns.FirstOrDefault(p => p.OwnGln == queryObj.ParentGln);
            var ids = new List<int>();

            if (!Equals(getParent, null))
            {
                ids = GetGlnIdsForEntireFamily(getParent.Id, ids);
            }

            if (SearchTermIsGlnNumber(queryObj))
            {
                query = query.Where(g => ids.Contains(g.Id) && g.Assigned &&
                                                       g.OwnGln.Contains(queryObj.SearchTerm));
            }
            else
            {
                query = query.Where(g => ids.Contains(g.Id) && g.Assigned &&
                                (g.FriendlyDescriptionPurpose.Contains(queryObj.SearchTerm) ||
                                 g.TruthDescriptionPurpose.Contains(queryObj.SearchTerm)));
            }

            return query;
        }

        private bool SearchTermIsGlnNumber(GlnQuery queryObj)
        {
            // Check first three digits of search term if all are numbers then assume they are searching by GLN
            return queryObj.SearchTerm.Take(3).All(char.IsNumber);
        }

        private IEnumerable<Gln> QueryGlnSearch(string searchTerm)
        {
            var friendlyDescriptionResults = IGlnRegistryDb.Glns.Where(bc => bc.Assigned)
                .Where(bc => bc.OwnGln.StartsWith(searchTerm) || bc.FriendlyDescriptionPurpose.Contains(searchTerm));

            var departmentResults = IGlnRegistryDb.Glns.Where(bc => bc.Assigned)
                .Where(bc => bc.Department.Contains(searchTerm));

            var functionResults = IGlnRegistryDb.Glns.Where(bc => bc.Assigned)
                .Where(bc => bc.Function.Contains(searchTerm));

            var queryResult = friendlyDescriptionResults.Union(departmentResults);
            queryResult = queryResult.Union(functionResults);
            queryResult.OrderBy(bc => bc.FriendlyDescriptionPurpose).ThenBy(bc => bc.Department);

            return queryResult.ToList();
        }

        public AdditionalContact GetAdditionalContact(int id)
        {
           return IGlnRegistryDb.AdditionalContacts.SingleOrDefault(ac => ac.Id == id);
        }

        public bool PrimaryContactExists(int pcId)
        {
            return IGlnRegistryDb.PrimaryContacts.Any( pc => pc.Id == pcId);
        }
        public bool AdditionalContactExists(int acId)
        {
            return IGlnRegistryDb.AdditionalContacts.Any( ac => ac.Id == acId);
        }

        private List<string> SplitString(string stringToBeSplit)
        {
            var wordList = new List<string>();
            var delimStr = " ,/:";
            var delimiter = delimStr.ToCharArray();

            var splitString = stringToBeSplit.Split(delimiter);

            foreach (var word in splitString)
            {
                wordList.Add(word);
            }

            return wordList;
        }

    }
}