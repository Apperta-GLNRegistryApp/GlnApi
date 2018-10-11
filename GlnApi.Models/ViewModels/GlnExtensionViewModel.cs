//Global Location Number (GLN) Registry API
//Copyright (C) 2018  University Hospitals Plymouth NHS Trust
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿namespace GlnApi.Models.ViewModels
{
    public class GlnExtensionViewModel
    {
        public int Id { get; set; }
        public string ExtensionNumber { get; set; }
        public string DescriptionPurpose { get; set; }
        public bool Active { get; set; }
        public string ExtendingGln { get; set; }
        public string ExtendingGlnDescription { get; set; }
        public int BarcodeId { get; set; }
    }
}