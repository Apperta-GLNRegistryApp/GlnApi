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
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "GLNREGISTRY.AdditionalContacts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Email = c.String(nullable: false, maxLength: 200),
                        System = c.String(maxLength: 250),
                        Telephone = c.String(nullable: false, maxLength: 50),
                        Fax = c.String(maxLength: 50),
                        Active = c.Boolean(nullable: false),
                        Salutation = c.String(maxLength: 50),
                        Version = c.Int(nullable: false),
                        Role = c.String(maxLength: 200),
                        TrustUsername = c.String(maxLength: 200),
                        NotificationSubscriber = c.Boolean(nullable: false),
                        Name = c.String(nullable: false, maxLength: 500),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "GLNREGISTRY.Addresses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AddressLineOne = c.String(nullable: false, maxLength: 500),
                        AddressLineTwo = c.String(maxLength: 500),
                        AddressLineThree = c.String(maxLength: 500),
                        AddressLineFour = c.String(maxLength: 500),
                        City = c.String(nullable: false, maxLength: 200),
                        RegionCounty = c.String(nullable: false, maxLength: 200),
                        Postcode = c.String(nullable: false, maxLength: 100),
                        Country = c.String(nullable: false, maxLength: 200),
                        Level = c.Short(nullable: false),
                        Room = c.String(maxLength: 200),
                        Active = c.Boolean(nullable: false),
                        DeliveryNote = c.String(),
                        Version = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "GLNREGISTRY.Glns",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FriendlyDescriptionPurpose = c.String(nullable: false, maxLength: 255),
                        Active = c.Boolean(nullable: false),
                        OwnGln = c.String(nullable: false, maxLength: 50),
                        ParentGln = c.String(maxLength: 50),
                        CreationDate = c.DateTime(nullable: false),
                        UseParentAddress = c.Boolean(nullable: false),
                        Verified = c.Boolean(),
                        FunctionalType = c.Boolean(nullable: false),
                        LegalType = c.Boolean(nullable: false),
                        DigitalType = c.Boolean(nullable: false),
                        PhysicalType = c.Boolean(nullable: false),
                        Public = c.Boolean(nullable: false),
                        Assigned = c.Boolean(nullable: false),
                        AddressId = c.Int(nullable: false),
                        ContactId = c.Int(nullable: false),
                        SuspensionReason = c.String(),
                        Version = c.Int(nullable: false),
                        NumberOfChildren = c.Int(nullable: false),
                        TrustActive = c.Boolean(nullable: false),
                        SuspendedBy = c.String(maxLength: 200),
                        Primary = c.Boolean(nullable: false),
                        ParentDescriptionPurpose = c.String(),
                        SuspensionDate = c.DateTime(nullable: false),
                        TruthDescriptionPurpose = c.String(nullable: false, maxLength: 255),
                        AlternativeSystemIsTruth = c.Boolean(nullable: false),
                        AlternativeSystemPK = c.String(),
                        AlternativeSystemOfTruthName = c.String(maxLength: 255),
                        TierLevel = c.Int(),
                        Department = c.String(maxLength: 500),
                        Function = c.String(maxLength: 255),
                        LastUpdate = c.DateTime(),
                        ParentId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("GLNREGISTRY.Glns", t => t.ParentId)
                .ForeignKey("GLNREGISTRY.PrimaryContacts", t => t.ContactId)
                .ForeignKey("GLNREGISTRY.Addresses", t => t.AddressId)
                .Index(t => t.AddressId)
                .Index(t => t.ContactId)
                .Index(t => t.ParentId);
            
            CreateTable(
                "GLNREGISTRY.Extensions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ExtensionNumber = c.String(nullable: false, maxLength: 50),
                        DescriptionPurpose = c.String(nullable: false),
                        Active = c.Boolean(nullable: false),
                        ExtendingGln = c.String(nullable: false, maxLength: 50),
                        ExtendingGlnDescription = c.String(nullable: false),
                        BarcodeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("GLNREGISTRY.Glns", t => t.BarcodeId)
                .Index(t => t.BarcodeId);
            
            CreateTable(
                "GLNREGISTRY.PrimaryContacts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Email = c.String(nullable: false, maxLength: 200),
                        Function = c.String(maxLength: 300),
                        Salutation = c.String(maxLength: 50),
                        Telephone = c.String(nullable: false, maxLength: 50),
                        Fax = c.String(maxLength: 50),
                        Active = c.Boolean(nullable: false),
                        Version = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 500),
                        ForPhysicals = c.Boolean(),
                        ForFunctionals = c.Boolean(),
                        ForLegals = c.Boolean(),
                        ForDigitals = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "GLNREGISTRY.GlnTags",
                c => new
                    {
                        GlnTagId = c.Int(nullable: false, identity: true),
                        GlnTagTypeId = c.Int(nullable: false),
                        GlnId = c.Int(nullable: false),
                        TypeKey = c.String(),
                        Active = c.Boolean(nullable: false),
                        UserCreated = c.String(),
                        UserModified = c.String(),
                        CreatedDateTime = c.DateTime(nullable: false),
                        ModifiedDateTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.GlnTagId)
                .ForeignKey("GLNREGISTRY.GlnTagTypes", t => t.GlnTagTypeId, cascadeDelete: true)
                .ForeignKey("GLNREGISTRY.Glns", t => t.GlnId)
                .Index(t => t.GlnTagTypeId)
                .Index(t => t.GlnId);
            
            CreateTable(
                "GLNREGISTRY.GlnTagTypes",
                c => new
                    {
                        GlnTagTypeId = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        Code = c.String(),
                        UserCreated = c.String(),
                        UserModified = c.String(),
                        Active = c.Boolean(nullable: false),
                        CreatedDateTime = c.DateTime(),
                        ModifiedDateTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.GlnTagTypeId);
            
            CreateTable(
                "GLNREGISTRY.GlnAssociations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GlnId1 = c.Int(nullable: false),
                        GlnId2 = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "GLNREGISTRY.Ipr",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        IprName = c.String(maxLength: 100),
                        IprImageAddress = c.String(maxLength: 100),
                        Active = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("GLNREGISTRY.Glns", "AddressId", "GLNREGISTRY.Addresses");
            DropForeignKey("GLNREGISTRY.GlnTags", "GlnId", "GLNREGISTRY.Glns");
            DropForeignKey("GLNREGISTRY.GlnTags", "GlnTagTypeId", "GLNREGISTRY.GlnTagTypes");
            DropForeignKey("GLNREGISTRY.Glns", "ContactId", "GLNREGISTRY.PrimaryContacts");
            DropForeignKey("GLNREGISTRY.Extensions", "BarcodeId", "GLNREGISTRY.Glns");
            DropForeignKey("GLNREGISTRY.Glns", "ParentId", "GLNREGISTRY.Glns");
            DropIndex("GLNREGISTRY.GlnTags", new[] { "GlnId" });
            DropIndex("GLNREGISTRY.GlnTags", new[] { "GlnTagTypeId" });
            DropIndex("GLNREGISTRY.Extensions", new[] { "BarcodeId" });
            DropIndex("GLNREGISTRY.Glns", new[] { "ParentId" });
            DropIndex("GLNREGISTRY.Glns", new[] { "ContactId" });
            DropIndex("GLNREGISTRY.Glns", new[] { "AddressId" });
            DropTable("GLNREGISTRY.Ipr");
            DropTable("GLNREGISTRY.GlnAssociations");
            DropTable("GLNREGISTRY.GlnTagTypes");
            DropTable("GLNREGISTRY.GlnTags");
            DropTable("GLNREGISTRY.PrimaryContacts");
            DropTable("GLNREGISTRY.Extensions");
            DropTable("GLNREGISTRY.Glns");
            DropTable("GLNREGISTRY.Addresses");
            DropTable("GLNREGISTRY.AdditionalContacts");
        }
    }
}
