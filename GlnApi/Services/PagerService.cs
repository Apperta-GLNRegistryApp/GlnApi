//Global Location Number (GLN) Registry API
//Copyright (C) 2018  University Hospitals Plymouth NHS Trust
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System.Collections.Generic;

namespace GlnApi.Services
{
    public class PagerService<TItem> where TItem : class
    {
        private readonly GlnRegistryDb _db;
        public PagerService(GlnRegistryDb db)
        {
            _db = db;

        }
        public int CurrentPage { get; set; }
        public double LastPageToDisplay { get; set; }
        public double FirstPageToDisplay { get; set; }
        public int ItemsPerPage { get; set; }
        public int TotalItems { get; set; }
        public int TotalItemsForDisplay { get; set; }
        public double TotalPages { get; set; }
        public double ItemsToDisplayFillsThisManyPages { get; set; }
        public IEnumerable<TItem> Items { get; set; }




    }
}