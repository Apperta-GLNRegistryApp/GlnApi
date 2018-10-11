-- Global Location Number (GLN) Registry API
-- Copyright (C) 2018  University Hospitals Plymouth NHS Trust
-- You should have received a copy of the GNU Affero General Public License
-- along with this program.  If not, see <http://www.gnu.org/licenses/>.
-- 
-- See LICENSE in the project root for license information.

-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 07/25/2017 16:17:50
-- Generated from EDMX file: C:\GS1 Api\gln-registry-aspNet\DbDiagram\GLNdbDiagram.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [AgileDev_Live];
GO
IF SCHEMA_ID(N'GLNREGISTRY') IS NULL EXECUTE(N'CREATE SCHEMA [GLNREGISTRY]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[GLNREGISTRY].[FK_BarcodeAddress]', 'F') IS NOT NULL
    ALTER TABLE [GLNREGISTRY].[Glns] DROP CONSTRAINT [FK_BarcodeAddress];
GO
IF OBJECT_ID(N'[GLNREGISTRY].[FK_BarcodeContact]', 'F') IS NOT NULL
    ALTER TABLE [GLNREGISTRY].[Glns] DROP CONSTRAINT [FK_BarcodeContact];
GO
IF OBJECT_ID(N'[GLNREGISTRY].[FK_BarcodeExtensions]', 'F') IS NOT NULL
    ALTER TABLE [GLNREGISTRY].[Extensions] DROP CONSTRAINT [FK_BarcodeExtensions];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[GLNREGISTRY].[AdditionalContacts]', 'U') IS NOT NULL
    DROP TABLE [GLNREGISTRY].[AdditionalContacts];
GO
IF OBJECT_ID(N'[GLNREGISTRY].[Addresses]', 'U') IS NOT NULL
    DROP TABLE [GLNREGISTRY].[Addresses];
GO
IF OBJECT_ID(N'[GLNREGISTRY].[Directorates]', 'U') IS NOT NULL
    DROP TABLE [GLNREGISTRY].[Directorates];
GO
IF OBJECT_ID(N'[GLNREGISTRY].[Divisions]', 'U') IS NOT NULL
    DROP TABLE [GLNREGISTRY].[Divisions];
GO
IF OBJECT_ID(N'[GLNREGISTRY].[Extensions]', 'U') IS NOT NULL
    DROP TABLE [GLNREGISTRY].[Extensions];
GO
IF OBJECT_ID(N'[GLNREGISTRY].[GlnAssociations]', 'U') IS NOT NULL
    DROP TABLE [GLNREGISTRY].[GlnAssociations];
GO
IF OBJECT_ID(N'[GLNREGISTRY].[Glns]', 'U') IS NOT NULL
    DROP TABLE [GLNREGISTRY].[Glns];
GO
IF OBJECT_ID(N'[GLNREGISTRY].[Ipr]', 'U') IS NOT NULL
    DROP TABLE [GLNREGISTRY].[Ipr];
GO
IF OBJECT_ID(N'[GLNREGISTRY].[PrimaryContacts]', 'U') IS NOT NULL
    DROP TABLE [GLNREGISTRY].[PrimaryContacts];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Addresses'
CREATE TABLE [GLNREGISTRY].[Addresses] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [AddressLineOne] nvarchar(500)  NOT NULL,
    [AddressLineTwo] nvarchar(500)  NULL,
    [AddressLineThree] nvarchar(500)  NULL,
    [AddressLineFour] nvarchar(500)  NULL,
    [City] nvarchar(200)  NOT NULL,
    [RegionCounty] nvarchar(200)  NOT NULL,
    [Postcode] nvarchar(100)  NOT NULL,
    [Country] nvarchar(200)  NOT NULL,
    [Level] smallint  NOT NULL,
    [Room] nvarchar(200)  NULL,
    [Active] bit  NOT NULL,
    [DeliveryNote] nvarchar(max)  NULL,
    [Version] int  NOT NULL
);
GO

-- Creating table 'PrimaryContacts'
CREATE TABLE [GLNREGISTRY].[PrimaryContacts] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Email] nvarchar(200)  NOT NULL,
    [Function] nvarchar(300)  NOT NULL,
    [Salutation] nvarchar(50)  NULL,
    [Telephone] nvarchar(50)  NOT NULL,
    [Fax] nvarchar(50)  NULL,
    [Active] bit  NOT NULL,
    [Version] int  NOT NULL,
    [Name] nvarchar(500)  NOT NULL,
    [ForPhysicals] bit  NULL,
    [ForFunctionals] bit  NULL,
    [ForLegals] bit  NULL,
    [ForDigitals] bit  NULL
);
GO

-- Creating table 'Directorates'
CREATE TABLE [GLNREGISTRY].[Directorates] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Active] int  NOT NULL
);
GO

-- Creating table 'AdditionalContacts'
CREATE TABLE [GLNREGISTRY].[AdditionalContacts] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Email] nvarchar(200)  NOT NULL,
    [System] nvarchar(250)  NULL,
    [Telephone] nvarchar(50)  NOT NULL,
    [Fax] nvarchar(50)  NULL,
    [Active] bit  NOT NULL,
    [Salutation] nvarchar(50)  NULL,
    [Version] int  NOT NULL,
    [Role] nvarchar(200)  NULL,
    [TrustUsername] nvarchar(200)  NULL,
    [NotificationSubscriber] bit  NOT NULL,
    [Name] nvarchar(500)  NOT NULL
);
GO

-- Creating table 'Extensions'
CREATE TABLE [GLNREGISTRY].[Extensions] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ExtensionNumber] nvarchar(50)  NOT NULL,
    [DescriptionPurpose] nvarchar(max)  NOT NULL,
    [Active] bit  NOT NULL,
    [ExtendingGln] nvarchar(50)  NOT NULL,
    [ExtendingGlnDescription] nvarchar(max)  NOT NULL,
    [BarcodeId] int  NOT NULL
);
GO

-- Creating table 'Divisions'
CREATE TABLE [GLNREGISTRY].[Divisions] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Active] int  NOT NULL
);
GO

-- Creating table 'GlnAssociations'
CREATE TABLE [GLNREGISTRY].[GlnAssociations] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [GlnId1] int  NOT NULL,
    [GlnId2] int  NOT NULL
);
GO

-- Creating table 'Glns'
CREATE TABLE [GLNREGISTRY].[Glns] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [FriendlyDescriptionPurpose] nvarchar(max)  NOT NULL,
    [Active] bit  NOT NULL,
    [OwnGln] nvarchar(50)  NOT NULL,
    [ParentGln] nvarchar(50)  NULL,
    [CreationDate] datetime  NOT NULL,
    [UseParentAddress] bit  NOT NULL,
    [Verified] bit  NULL,
    [FunctionalType] bit  NOT NULL,
    [LegalType] bit  NOT NULL,
    [DigitalType] bit  NOT NULL,
    [PhysicalType] bit  NOT NULL,
    [Public] bit  NOT NULL,
    [Assigned] bit  NOT NULL,
    [AddressId] int  NOT NULL,
    [ContactId] int  NOT NULL,
    [SuspensionReason] nvarchar(max)  NULL,
    [Version] int  NOT NULL,
    [NumberOfChildren] int  NOT NULL,
    [TrustActive] bit  NOT NULL,
    [SuspendedBy] nvarchar(200)  NULL,
    [Primary] bit  NOT NULL,
    [ParentDescriptionPurpose] nvarchar(max)  NOT NULL,
    [SuspensionDate] datetime  NOT NULL,
    [TruthDescriptionPurpose] nvarchar(max)  NOT NULL,
    [AlternativeSystemIsTruth] bit  NOT NULL,
    [AlternativeSystemPK] nvarchar(max)  NULL,
    [AlternativeSystemOfTruthName] nvarchar(255)  NULL,
    [Department] nvarchar(500)  NULL,
    [Function] nvarchar(250)  NULL,
    [LastUpdate] datetime  NULL,
    [TierLevel] int  NULL
);
GO

-- Creating table 'Iprs'
CREATE TABLE [GLNREGISTRY].[Iprs] (
    [Id] int  NOT NULL,
    [IprName] nvarchar(100)  NULL,
    [IprImageAddress] nvarchar(100)  NULL,
    [Active] bit  NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Addresses'
ALTER TABLE [GLNREGISTRY].[Addresses]
ADD CONSTRAINT [PK_Addresses]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'PrimaryContacts'
ALTER TABLE [GLNREGISTRY].[PrimaryContacts]
ADD CONSTRAINT [PK_PrimaryContacts]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Directorates'
ALTER TABLE [GLNREGISTRY].[Directorates]
ADD CONSTRAINT [PK_Directorates]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'AdditionalContacts'
ALTER TABLE [GLNREGISTRY].[AdditionalContacts]
ADD CONSTRAINT [PK_AdditionalContacts]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Extensions'
ALTER TABLE [GLNREGISTRY].[Extensions]
ADD CONSTRAINT [PK_Extensions]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Divisions'
ALTER TABLE [GLNREGISTRY].[Divisions]
ADD CONSTRAINT [PK_Divisions]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'GlnAssociations'
ALTER TABLE [GLNREGISTRY].[GlnAssociations]
ADD CONSTRAINT [PK_GlnAssociations]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Glns'
ALTER TABLE [GLNREGISTRY].[Glns]
ADD CONSTRAINT [PK_Glns]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Iprs'
ALTER TABLE [GLNREGISTRY].[Iprs]
ADD CONSTRAINT [PK_Iprs]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [AddressId] in table 'Glns'
ALTER TABLE [GLNREGISTRY].[Glns]
ADD CONSTRAINT [FK_BarcodeAddress]
    FOREIGN KEY ([AddressId])
    REFERENCES [GLNREGISTRY].[Addresses]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_BarcodeAddress'
CREATE INDEX [IX_FK_BarcodeAddress]
ON [GLNREGISTRY].[Glns]
    ([AddressId]);
GO

-- Creating foreign key on [BarcodeId] in table 'Extensions'
ALTER TABLE [GLNREGISTRY].[Extensions]
ADD CONSTRAINT [FK_BarcodeExtensions]
    FOREIGN KEY ([BarcodeId])
    REFERENCES [GLNREGISTRY].[Glns]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_BarcodeExtensions'
CREATE INDEX [IX_FK_BarcodeExtensions]
ON [GLNREGISTRY].[Extensions]
    ([BarcodeId]);
GO

-- Creating foreign key on [ContactId] in table 'Glns'
ALTER TABLE [GLNREGISTRY].[Glns]
ADD CONSTRAINT [FK_BarcodeContact]
    FOREIGN KEY ([ContactId])
    REFERENCES [GLNREGISTRY].[PrimaryContacts]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_BarcodeContact'
CREATE INDEX [IX_FK_BarcodeContact]
ON [GLNREGISTRY].[Glns]
    ([ContactId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------