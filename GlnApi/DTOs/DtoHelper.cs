//Global Location Number (GLN) Registry API
//Copyright (C) 2018  University Hospitals Plymouth NHS Trust
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System.Linq;
using GlnApi.DTOs;
using GlnApi.Models;

namespace GlnApi.Helpers
{
    public static class DtoHelper
    {
        private static GlnDto CreateBasicGlnDto(Gln gln)
        {
            var glnDto = new GlnDto()
            {
                Id = gln.Id,
                FriendlyDescriptionPurpose = gln.FriendlyDescriptionPurpose,
                Active = gln.Active,
                OwnGln = gln.OwnGln,
                ParentGln = gln.ParentGln,
                CreationDate = gln.CreationDate,
                UseParentAddress = gln.UseParentAddress,
                Verified = gln.Verified.Value,
                FunctionalType = gln.FunctionalType,
                LegalType = gln.LegalType,
                DigitalType = gln.DigitalType,
                PhysicalType = gln.PhysicalType,
                Public = gln.Public,
                Assigned = gln.Assigned,
                AddressId = gln.AddressId,
                ContactId = gln.ContactId,
                SuspensionReason = gln.SuspensionReason,
                Version = gln.Version,
                NumberOfChildren = gln.Children.Count,
                TrustActive = gln.TrustActive,
                SuspendedBy = gln.SuspendedBy,
                Primary = gln.Primary,
                ParentDescriptionPurpose = gln.ParentDescriptionPurpose,
                SuspensionDate = gln.SuspensionDate,
                TruthDescriptionPurpose = gln.TruthDescriptionPurpose,
                AlternativeSystemIsTruth = gln.AlternativeSystemIsTruth,
                Department = gln.Department,
                Function = gln.Function,
                DeliveryNote = gln.DeliveryNote
            };

            return glnDto;
        }
        public static GlnDto CreateGlnIncludeChildrenDto(Gln gln)
        {
            var glnDto = CreateBasicGlnDto(gln);

            if (!Equals(gln.Address, null))
                glnDto.Address = CreateAddressDto(gln.Address);

            if (!Equals(gln.PrimaryContact, null))
                glnDto.PrimaryContact = CreatePrimaryContactDto(gln.PrimaryContact);

            if (!Equals(gln.Parent, null))
                glnDto.Parent = CreateGlnSummaryDto(gln.Parent);

            if (!Equals(gln.TierLevel, null))
                glnDto.TierLevel = gln.TierLevel.Value;

            if (!Equals(gln.Children, null))
                glnDto.Children = gln.Children.Where(c => c.Assigned).OrderBy(c => c.FriendlyDescriptionPurpose).Select(CreateGlnSummaryDto).ToList();

            if (!Equals(gln.Tags, null))
                glnDto.Tags = gln.Tags.Where(t => t.Active).OrderBy(t => t.GlnTagType.Description).Select(CreateGlnTagDto).ToList();

            return glnDto;
        }
        public static GlnDto CreateGlnDto(Gln gln)
        {
            var glnDto = CreateBasicGlnDto(gln);

            if (!Equals(gln.Address, null))
                glnDto.Address = CreateAddressDto(gln.Address);

            if (!Equals(gln.PrimaryContact, null))
                glnDto.PrimaryContact = CreatePrimaryContactDto(gln.PrimaryContact);

            if (!Equals(gln.TierLevel, null))
                glnDto.TierLevel = gln.TierLevel.Value;

            if (!Equals(gln.Tags, null))
                glnDto.Tags = gln.Tags.Where(t => t.Active).OrderBy(t => t.GlnTagType.Description).Select(CreateGlnTagDto).ToList();

            return glnDto;
        }
        public static GlnSummaryDto CreateGlnSummaryDto(Gln gln)
        {
                
            var glnSummaryDto = new GlnSummaryDto()
            {
                Id = gln.Id,
                FriendlyDescriptionPurpose = gln.FriendlyDescriptionPurpose,
                Active = gln.Active,
                OwnGln = gln.OwnGln,
                ParentGln = gln.ParentGln,
                ParentDescriptionPurpose = gln.ParentDescriptionPurpose,
                FunctionalType = gln.FunctionalType,
                LegalType = gln.LegalType,
                DigitalType = gln.DigitalType,
                PhysicalType = gln.PhysicalType,
                NumberOfChildren = gln.NumberOfChildren,
                TrustActive = gln.TrustActive,
                SuspensionReason = gln.SuspensionReason,
                SuspendedBy = gln.SuspendedBy,
                SuspensionDate = gln.SuspensionDate,
                Version = gln.Version,
                Primary = gln.Primary,
                TruthDescriptionPurpose = gln.TruthDescriptionPurpose,
                AlternativeSystemIsTruth = gln.AlternativeSystemIsTruth,
                Public = gln.Public,
                Department = gln.Department,
                Function = gln.Function,
            };

            if (!Equals(gln.TierLevel, null))
                glnSummaryDto.Level = gln.TierLevel.Value;

            return glnSummaryDto;
        }

        public static AddressDto CreateAddressDto(Address address)
        {
            var addressDto = new AddressDto()
            {
                Id = address.Id,
                AddressLineOne = address.AddressLineOne,
                AddressLineTwo = address.AddressLineTwo,
                AddressLineThree = address.AddressLineThree,
                AddressLineFour = address.AddressLineFour,
                City = address.City,
                RegionCounty = address.RegionCounty,
                Postcode = address.Postcode,
                Country = address.Country,
                Level = address.Level,
                Room = address.Room,
                Active = address.Active,
                DeliveryNote = address.DeliveryNote,
                Version = address.Version
            };

            return addressDto;
        }

        public static PrimaryContactDto CreatePrimaryContactDto(PrimaryContact contact)
        {
            var contactDto = new PrimaryContactDto()
            {
                Id = contact.Id,
                Name = contact.Name,
                Email = contact.Email,
                Function = contact.Function,
                Salutation = contact.Salutation,
                Telephone = contact.Telephone,
                Fax = contact.Fax,
                Active = contact.Active,
                Version = contact.Version
            };

            return contactDto;
        }
        public static AdditionalContactDto CreateAdditionalContactDto(AdditionalContact additionalContact)
        {
            var additionalContactDto = new AdditionalContactDto()
            {
                Id = additionalContact.Id,
                Name = additionalContact.Name,
                Email = additionalContact.Email,
                System = additionalContact.System,
                Salutation = additionalContact.Salutation,
                Telephone = additionalContact.Telephone,
                Fax = additionalContact.Fax,
                Role = additionalContact.Role,
                TrustUsername = additionalContact.TrustUsername,
                Active = additionalContact.Active,
                NotificationSubscriber = additionalContact.NotificationSubscriber,
                Version = additionalContact.Version
            };

            return additionalContactDto;
        }
        public static GlnExtensionDto CreateExtensionsDto(Extension glnExtension)
        {
            var glnExtensionDto = new GlnExtensionDto()
            {
                Id = glnExtension.Id,
                ExtensionNumber = glnExtension.ExtensionNumber,
                DescriptionPurpose = glnExtension.DescriptionPurpose,
                Active = glnExtension.Active,
                ExtendingGln = glnExtension.ExtendingGln,
                ExtendingGlnDescription = glnExtension.ExtendingGlnDescription,
                BarcodeId = glnExtension.BarcodeId
            };

            return glnExtensionDto;
        }

        public static IprDto CreateIprDto(Ipr ipr)
        {
            var IprDto = new IprDto()
            {
                Id = ipr.Id,
                IprName = ipr.IprName,
                Active = ipr.Active.Value,
                IprImageAddress = ipr.IprImageAddress
            };

            return IprDto;
        }

        public static GlnTagTypeDto CreateGlnTagTypeDto(GlnTagType glnTagType)
        {
            var tagTypeDto = new GlnTagTypeDto()
            {
                GlnTagTypeId = glnTagType.GlnTagTypeId,
                Code = glnTagType.Code,
                Description = glnTagType.Description,
                Active = glnTagType.Active
            };
                //tagTypeDto.GlnTags = glnTagType.GlnTags?.Select(t => new GlnTagDto()
                //{
                //    Active = t.Active,
                //    GlnId = t.GlnId,
                //    GlnTagTypeId = t.GlnTagTypeId,
                //    GlnTagId = t.GlnTagId,
                //    TypeKey = t.TypeKey
                //}).ToList();

            return tagTypeDto;
        }

        public static GlnTagDto CreateGlnTagDto(GlnTag glnTag)
        {
            var glnTagDto = new GlnTagDto()
            {
                Active = glnTag.Active,
                GlnId = glnTag.GlnId,
                GlnTagId = glnTag.GlnTagId,
                GlnTagTypeId = glnTag.GlnTagTypeId,
                TypeKey = glnTag.TypeKey
            };

            if (!Equals(glnTag.GlnTagType, null))
                glnTagDto.GlnTagType = CreateGlnTagTypeDto(glnTag.GlnTagType);

            return glnTagDto;
        }
    }
}