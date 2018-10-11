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

    [Table("GLNREGISTRY.Addresses")]
    public partial class Address
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Address()
        {
            Glns = new HashSet<Gln>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(500)]
        public string AddressLineOne { get; set; }

        [StringLength(500)]
        public string AddressLineTwo { get; set; }

        [StringLength(500)]
        public string AddressLineThree { get; set; }

        [StringLength(500)]
        public string AddressLineFour { get; set; }

        [Required]
        [StringLength(200)]
        public string City { get; set; }

        [Required]
        [StringLength(200)]
        public string RegionCounty { get; set; }

        [Required]
        [StringLength(100)]
        public string Postcode { get; set; }

        [Required]
        [StringLength(200)]
        public string Country { get; set; }

        public short Level { get; set; }

        [StringLength(200)]
        public string Room { get; set; }

        public bool Active { get; set; }

        public string DeliveryNote { get; set; }

        public int Version { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Gln> Glns { get; set; }
    }
}
