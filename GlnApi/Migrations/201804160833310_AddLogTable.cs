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
    
    public partial class AddLogTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Logs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SystemName = c.String(),
                        LogType = c.Int(nullable: false),
                        MessageType = c.Int(nullable: false),
                        Message = c.String(),
                        SamAccountName = c.String(),
                        CreatedDateTime = c.DateTime(nullable: false),
                        GeneratedExceptionMessage = c.String(),
                        GeneratedInnerExceptionMessage = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Logs");
        }
    }
}
