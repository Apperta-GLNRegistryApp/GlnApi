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

namespace GlnApi.Models
{
    public class PageItem<TItem> where TItem : class
    {
        public PageItem()
        {
            Items = new List<TItem>();
        }
        public int PageIndex { get; set; }
        public IEnumerable<TItem> Items { get; set; }
        
    }
    public class PaginationDto<TItem> where TItem : class
    {
        public PaginationDto()
        {
            PageOneItems = new List<TItem>();
            PageTwoItems = new List<TItem>();
            PageThreeItems = new List<TItem>();
        }
        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; }
        public int TotalItems { get; set; }
        public double TotalPages { get; set; }
        public int PageOneIndex { get; set; }
        public int PageTwoIndex { get; set; }
        public int PageThreeIndex { get; set; }
        public IEnumerable<TItem> Items { get; set; }
        public IEnumerable<TItem> PageOneItems { get; set; }
        public IEnumerable<TItem> PageTwoItems { get; set; }
        public IEnumerable<TItem> PageThreeItems { get; set; }
        public string SearchTerm { get; set; } = "";
        public string PreviousSearchTerm { get; set; } = "";

        public PaginationDto(int currentPage, int itemsPerPage, int totalItems, IEnumerable<TItem> items ) : this()
        {
            if (Equals(items, null))
                throw new ArgumentNullException("No items for display supplied");

            CurrentPage = currentPage;
            ItemsPerPage = itemsPerPage;
            TotalItems = totalItems;
            Items = items;
            
            CalcualteTotalPages();

            AssignItemsToPages();
            AssignIndexToPages();
        }

        public void Reset(int currentPage, int itemsPerPage, int totalItems, IEnumerable<TItem> items)
        {
            if (Equals(items, null))
                throw new ArgumentNullException("No items for display supplied");

            CurrentPage = currentPage;
            ItemsPerPage = itemsPerPage;
            TotalItems = totalItems;
            Items = items;

            CalcualteTotalPages();

            AssignItemsToPages();
            AssignIndexToPages();
        }

        public void AssignItemsToPages()
        {
            PageOneItems = Items.Take(ItemsPerPage);
            PageTwoItems = Items.Skip(ItemsPerPage).Take(ItemsPerPage);
            PageThreeItems = Items.Skip(ItemsPerPage * 2).Take(ItemsPerPage);
        }

        public void AssignIndexToPages()
        {
            if (CurrentPage > 1)
            {
                PageOneIndex = CurrentPage;
                PageTwoIndex = CurrentPage + 1;
                PageThreeIndex = CurrentPage + 2;
            }
            else
            {
                PageOneIndex = CurrentPage;
                PageTwoIndex = CurrentPage + 1;
                PageThreeIndex = CurrentPage + 2;
            }
        }

        private void CalcualteTotalPages()
        {
            TotalPages = Math.Ceiling((double)TotalItems / ItemsPerPage);
        }
    }
}