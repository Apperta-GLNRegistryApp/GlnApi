//Global Location Number (GLN) Registry API
//Copyright (C) 2018  University Hospitals Plymouth NHS Trust
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System.Collections.Generic;
using GlnApi.Extensions;

namespace GlnApi.Models
{
    public class GlnQuery : IQueryObject
    {
        public bool Physical { get; set; }
        public bool Functional { get; set; }
        public bool Digital { get; set; }
        public bool Legal { get; set; }
        public bool Public { get; set; }
        public bool MatchAllTypes { get; set; }
        public bool Private { get; set; }
        public bool TrustActive { get; set; }
        public bool AllStatus { get; set; }
        public string SortBy { get; set; }
        public string ThenSortBy { get; set; }
        public int Page { get; set; }
        public bool IsSortAscending { get; set; }
        public byte PageSize { get; set; }
        public string SearchTerm { get; set; }
        public bool ChildrenOnly { get; set; }
        public string ParentGln { get; set; }
        public bool StartsWith { get; set; }
        public List<int> TagTypeIds { get; set; }
    }
}