//Global Location Number (GLN) Registry API
//Copyright (C) 2018  University Hospitals Plymouth NHS Trust
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
namespace GlnApi
{
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("GLNREGISTRY.GlnAssociations")]
    public partial class GlnAssociation
    {
        public int Id { get; set; }

        public int GlnId1 { get; set; }

        public int GlnId2 { get; set; }
    }
}
