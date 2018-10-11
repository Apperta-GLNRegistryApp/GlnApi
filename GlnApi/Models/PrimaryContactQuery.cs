//Global Location Number (GLN) Registry API
//Copyright (C) 2018  University Hospitals Plymouth NHS Trust
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using GlnApi.Extensions;

namespace GlnApi.Repository
{
    public class PrimaryContactQuery : IQueryObject
    {
        public bool Active { get; set; }
        public string SortBy { get; set; }
        public string ThenSortBy { get; set; }
        public bool IsSortAscending { get; set; }
        public int Page { get; set; }
        public byte PageSize { get; set; }
        public string SearchTerm { get; set; }
    }
}