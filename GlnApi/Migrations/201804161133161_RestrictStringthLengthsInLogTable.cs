//Global Location Number (GLN) Registry API
//Copyright (C) 2018  University Hospitals Plymouth NHS Trust
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
namespace gln_registry_aspNet.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RestrictStringthLengthsInLogTable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("GLNREGISTRY.Logs", "SystemName", c => c.String(maxLength: 255));
            AlterColumn("GLNREGISTRY.Logs", "SamAccountName", c => c.String(maxLength: 255));
        }
        
        public override void Down()
        {
            AlterColumn("GLNREGISTRY.Logs", "SamAccountName", c => c.String());
            AlterColumn("GLNREGISTRY.Logs", "SystemName", c => c.String());
        }
    }
}
