//Global Location Number (GLN) Registry API
//Copyright (C) 2018  University Hospitals Plymouth NHS Trust
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
namespace GlnApi
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("GLNREGISTRY.Extensions")]
    public partial class Extension
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string ExtensionNumber { get; set; }

        [Required]
        public string DescriptionPurpose { get; set; }

        public bool Active { get; set; }

        [Required]
        [StringLength(50)]
        public string ExtendingGln { get; set; }

        [Required]
        public string ExtendingGlnDescription { get; set; }

        public int BarcodeId { get; set; }

        public virtual Gln Gln { get; set; }
    }
}
