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
    
    public partial class RenameLogTable : DbMigration
    {
        public override void Up()
        {
            MoveTable(name: "dbo.Logs", newSchema: "GLNREGISTRY");
        }
        
        public override void Down()
        {
            MoveTable(name: "GLNREGISTRY.Logs", newSchema: "dbo");
        }
    }
}
