//Global Location Number (GLN) Registry API
//Copyright (C) 2018  University Hospitals Plymouth NHS Trust
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
using GlnApi.Models;

namespace GlnApi
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("GLNREGISTRY.Glns")]
    public partial class Gln
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Gln()
        {
            Extensions = new HashSet<Extension>();
            Children = new HashSet<Gln>();
            Tags = new HashSet<GlnTag>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string FriendlyDescriptionPurpose { get; set; }

        public bool Active { get; set; }

        [Required]
        [StringLength(50)]
        public string OwnGln { get; set; }

        [StringLength(50)]
        public string ParentGln { get; set; }

        public DateTime CreationDate { get; set; }

        public bool UseParentAddress { get; set; }

        public bool? Verified { get; set; }

        public bool FunctionalType { get; set; }

        public bool LegalType { get; set; }

        public bool DigitalType { get; set; }

        public bool PhysicalType { get; set; }

        public bool Public { get; set; }

        public bool Assigned { get; set; }

        public int AddressId { get; set; }

        public int ContactId { get; set; }

        public string SuspensionReason { get; set; }

        public int Version { get; set; }

        public int NumberOfChildren { get; set; }

        public bool TrustActive { get; set; }

        [StringLength(200)]
        public string SuspendedBy { get; set; }

        public bool Primary { get; set; }

        public string ParentDescriptionPurpose { get; set; }

        public DateTime SuspensionDate { get; set; }

        [Required]
        [StringLength(255)]
        public string TruthDescriptionPurpose { get; set; }

        public bool AlternativeSystemIsTruth { get; set; }

        public string AlternativeSystemPK { get; set; }

        [StringLength(255)]
        public string AlternativeSystemOfTruthName { get; set; }

        public int? TierLevel { get; set; }

        [StringLength(500)]
        public string Department { get; set; }

        [StringLength(255)]
        public string Function { get; set; }

        public DateTime? LastUpdate { get; set; }

        public int? ParentId { get; set; }
        [StringLength(510)]
        public string DeliveryNote { get; set; }

        public virtual Address Address { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Extension> Extensions { get; set; }

        public virtual PrimaryContact PrimaryContact { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Gln> Children { get; set; }
        public virtual ICollection<GlnTag> Tags { get; set; }

        public virtual Gln Parent { get; set; }
    }
}
