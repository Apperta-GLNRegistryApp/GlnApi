//Global Location Number (GLN) Registry API
//Copyright (C) 2018  University Hospitals Plymouth NHS Trust
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
namespace GlnApi
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("GLNREGISTRY.PrimaryContacts")]
    public partial class PrimaryContact
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PrimaryContact()
        {
            Glns = new HashSet<Gln>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Email { get; set; }

        [StringLength(300)]
        public string Function { get; set; }

        [StringLength(50)]
        public string Salutation { get; set; }

        [Required]
        [StringLength(50)]
        public string Telephone { get; set; }

        [StringLength(50)]
        public string Fax { get; set; }

        public bool Active { get; set; }

        public int Version { get; set; }

        [Required]
        [StringLength(500)]
        public string Name { get; set; }

        public bool? ForPhysicals { get; set; }

        public bool? ForFunctionals { get; set; }

        public bool? ForLegals { get; set; }

        public bool? ForDigitals { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Gln> Glns { get; set; }
    }
}
