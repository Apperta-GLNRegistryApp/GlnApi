//Global Location Number (GLN) Registry API
//Copyright (C) 2018  University Hospitals Plymouth NHS Trust
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
using System;

namespace GlnApi.Models.ViewModels
{
    public class GlnTagViewModel
    {
        public int GlnTagId { get; set; }
        public int GlnTagTypeId { get; set; }
        public int GlnId { get; set; }
        public string TypeKey { get; set; }
        public bool Active { get; set; }
        public string UserCreated { get; set; }
        public string UserModified { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime? ModifiedDateTime { get; set; }

        public virtual GlnTagTypeViewModel GlnTagType { get; set; }
    }
}