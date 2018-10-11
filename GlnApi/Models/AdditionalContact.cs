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

    [Table("GLNREGISTRY.AdditionalContacts")]
    public partial class AdditionalContact
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Email { get; set; }

        [StringLength(250)]
        public string System { get; set; }

        [Required]
        [StringLength(50)]
        public string Telephone { get; set; }

        [StringLength(50)]
        public string Fax { get; set; }

        public bool Active { get; set; }

        [StringLength(50)]
        public string Salutation { get; set; }

        public int Version { get; set; }

        [StringLength(200)]
        public string Role { get; set; }

        [StringLength(200)]
        public string TrustUsername { get; set; }

        public bool NotificationSubscriber { get; set; }

        [Required]
        [StringLength(500)]
        public string Name { get; set; }
    }
}
