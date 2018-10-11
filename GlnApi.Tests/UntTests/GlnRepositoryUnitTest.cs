//Global Location Number (GLN) Registry API
//Copyright (C) 2018  University Hospitals Plymouth NHS Trust
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System;
using System.Collections.Generic;
using System.Linq;
using GlnApi.Models;
using GlnApi.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xunit;
using GlnApi.Tests.TestHelpers;
using Assert = Xunit.Assert;

namespace GlnApi.Tests
{
    [TestClass]
    public class GlnRepositoryUnitTest
    {
        private FakeGlnContext _dbContext;
        private GlnRepository _glnRepository;
        private Gln _sentGln;
        private UnitOfWork _unitOfWork;
        public GlnRepositoryUnitTest()
        {
            _dbContext = new FakeGlnContext();
            _glnRepository = new GlnRepository(_dbContext);
            _sentGln = new Gln();
            _unitOfWork = new UnitOfWork(_dbContext);
            InitGlnValues();
            _dbContext.Glns.Add(_sentGln);
        }

        private void ChangeGlnValues()
        {
            _sentGln.Id = 1;
            _sentGln.CreationDate = DateTime.Now;
            _sentGln.FriendlyDescriptionPurpose = "changed";
            _sentGln.TruthDescriptionPurpose = "changed";
            _sentGln.ParentGln = "changed";
            _sentGln.SuspendedBy = "changed";
            _sentGln.Active = false;
            _sentGln.AddressId = 1;
            _sentGln.AlternativeSystemIsTruth = false;
            _sentGln.TrustActive = false;
            _sentGln.ParentId = 2;
            _sentGln.AlternativeSystemOfTruthName = "changed";
            _sentGln.AlternativeSystemPK = "changed";
            _sentGln.Assigned = false;
            _sentGln.ContactId = 1;
            _sentGln.Department = "changed";
            _sentGln.DigitalType = false;
            _sentGln.FunctionalType = false;
            _sentGln.DigitalType = false;
            _sentGln.LegalType = false;
            _sentGln.Primary = false;
            _sentGln.ParentDescriptionPurpose = "changed";
            _sentGln.LastUpdate = DateTime.Now;
        }
        private void InitGlnValues()
        {
            _sentGln.Id = 1;
            _sentGln.OwnGln = "tier" + 1.ToString();
            _sentGln.CreationDate = DateTime.Now;
            _sentGln.FriendlyDescriptionPurpose = "initial";
            _sentGln.TruthDescriptionPurpose = "initial";
            _sentGln.ParentGln = "inital";
            _sentGln.SuspendedBy = "inital";
            _sentGln.Active = true;
            _sentGln.AddressId = 1;
            _sentGln.AlternativeSystemIsTruth = true;
            _sentGln.TrustActive = true;
            _sentGln.AlternativeSystemOfTruthName = "inital";
            _sentGln.AlternativeSystemPK = "inital";
            _sentGln.Assigned = true;
            _sentGln.ContactId = 1;
            _sentGln.Department = "inital";
            _sentGln.DigitalType = true;
            _sentGln.FunctionalType = true;
            _sentGln.DigitalType = true;
            _sentGln.LegalType = true;
            _sentGln.Primary = true;
            _sentGln.ParentDescriptionPurpose = "inital";
            _sentGln.LastUpdate = DateTime.Now;
        }

        [Fact]
        [Trait("Category", "GetGlnToUpdate")]
        public void GetGlnToUpdate_UpdateFriendlyDescription()
        {
            // Arrange
            ChangeGlnValues();

            // Act
            ChangeGlnValues();
            var result = _unitOfWork.Glns.UpdateGln(_sentGln);

            // Check
            Assert.Equal("CHANGED", result.FriendlyDescriptionPurpose.ToUpper());
        }

        [Fact]
        [Trait("Category", "GetGlnToUpdate")]
        public void GetGlnToUpdate_ChangeParent()
        {
            // Arrange
            // Create fake data
            var currentParentGln = new Gln
            {
                Id = 2,
                OwnGln = "Current Parent",
                FriendlyDescriptionPurpose = "Current Parent",
                TrustActive = true
            };
            var newParentGln = new Gln
            {
                Id = 3,
                OwnGln = "New parent",
                FriendlyDescriptionPurpose = "New parent",
                TrustActive = true
            };

            _dbContext.Glns.Add(currentParentGln);
            _dbContext.Glns.Add(newParentGln);

            // Act
            _sentGln.ParentGln = "New parent";
            var result = _glnRepository.UpdateGln(_sentGln);

            // Check
            Assert.Equal(newParentGln.FriendlyDescriptionPurpose.ToUpper(), result.ParentGln.ToUpper());
            Assert.Equal(newParentGln.Id, result.ParentId);
        }

        [Fact]
        [Trait("Category", "GetGlnToUpdate")]
        public void GetGlnToUpdate_SetSuspendedByIfNotTrustActive()
        {
            // Arrange
            // Create fake data
            var suspendedBy = "unknown";
;
            _sentGln.TrustActive = false;

            // Act
            var result = _glnRepository.UpdateGln(_sentGln);

            // Check
            Assert.Equal(suspendedBy.ToUpper(), result.SuspendedBy.ToUpper());
        }

        [Fact]
        [Trait("Category", "GetGlnToUpdate")]
        public void GetGlnToUpdate_UpdateTruthToFriendlyIfAssignedAndIsntAlternativeSystem()
        {
            // Arrange
            // Create fake data
            _sentGln.AlternativeSystemIsTruth = false;

            // Act
            var result = _glnRepository.UpdateGln(_sentGln);

            // Check
            Assert.Equal(_sentGln.FriendlyDescriptionPurpose.ToUpper(), result.TruthDescriptionPurpose.ToUpper());
        }

        [Fact]
        [Trait("Category", "GetGlnToUpdate")]
        public void GetGlnToUpdate_ActiveDeactivated()
        {
            // Arrange
            // Create fake data
            _sentGln.Active = false;
            var now = DateTime.Now;
            // Act
            var result = _glnRepository.UpdateGln(_sentGln);

            // Check
            Assert.Equal("National deactivated", result.SuspensionReason);
            Assert.NotEqual(now, result.SuspensionDate);
        }

        [Fact]
        [Trait("Category", "GetGlnAssociations")]
        public void GetGlnAssociations_GetAssociations()
        {
            // Arrange
            // Create fake data
            var gln2 = new Gln();
            gln2.Id = 2;
            gln2.FriendlyDescriptionPurpose = "two";
            _dbContext.Glns.Add(gln2);

            var gln3 = new Gln();
            gln3.Id = 3;
            gln3.FriendlyDescriptionPurpose = "three";
            _dbContext.Glns.Add(gln3);

            var association1 = new GlnAssociation();

            association1.Id = 1;
            association1.GlnId1 = 1;
            association1.GlnId2 = 2;

            _dbContext.GlnAssociations.Add(association1);

            var association2 = new GlnAssociation();

            association2.Id = 2;
            association2.GlnId1 = 1;
            association2.GlnId2 = 3;

            _dbContext.GlnAssociations.Add(association2);

            // Act
            var result = _glnRepository.GetGlnAssociations(1);

            // Check
            Assert.Equal(2, result.Count());
            Assert.Collection(result, item => Assert.Contains("two", item.FriendlyDescriptionPurpose),
                                      item => Assert.Contains("three", item.FriendlyDescriptionPurpose));
        }

        [Fact]
        [Trait("Category", "GetGlnAssociations")]
        public void GetGlnAssociations_GetNoAssociations_ReturnEmptyGlnList()
        {
            // Arrange
            // Create fake data

            // Act
            var result = _glnRepository.GetGlnAssociations(1);

            // Check
            Assert.Equal(false, result.Any());
        }

        [Fact]
        [Trait("Category", "AssignNewGlnAssociation")]
        public void GetGlnAssociations_AssignNewGlnAssociation_DbSetContainsNewAssociation()
        {
            // Arrange
            var gln2 = new Gln();
            gln2.Id = 2;
            gln2.FriendlyDescriptionPurpose = "two";
            _dbContext.Glns.Add(gln2);

            var gln3 = new Gln();
            gln3.Id = 3;
            gln3.FriendlyDescriptionPurpose = "three";
            _dbContext.Glns.Add(gln3);

            // Act
            _glnRepository.AssignNewGlnAssociation(2, 3);
            _glnRepository.AssignNewGlnAssociation(1, 3);

            // Check
            Assert.Equal(2, _glnRepository.IGlnRegistryDb.GlnAssociations.Count());
        }

        [Fact]
        [Trait("Category", "AssignNewGlnAssociation")]
        public void AssignNewGlnAssociation_EnsureDuplicateAssociationsAreNotCreated_DbSetDoesNotContainDuplicateAssociation()
        {
            // Arrange
            // Create fake data
            var gln2 = new Gln();
            gln2.Id = 2;
            gln2.FriendlyDescriptionPurpose = "two";
            _dbContext.Glns.Add(gln2);

            var gln3 = new Gln();
            gln3.Id = 3;
            gln3.FriendlyDescriptionPurpose = "three";
            _dbContext.Glns.Add(gln3);

            // Act
            _glnRepository.AssignNewGlnAssociation(1, 2);
            _glnRepository.AssignNewGlnAssociation(2, 1);
            // This is adding an duplicate association
            _glnRepository.AssignNewGlnAssociation(1, 2);
            // This the association that was added twice, should only be one instance of it saved
            var result = _glnRepository.GetGlnAssociations(1);

            // Check
            Assert.Equal(1, result.Count());
            Assert.Equal(1, _glnRepository.IGlnRegistryDb.GlnAssociations.Count());
        }

        [Fact]
        [Trait("Category", "RemoveGlnAssociation")]
        public void RemoveGlnAssociation_EnsureAssociationIsRemoved_DbSetDoesNotContainRemovedAssociation()
        {
            // Arrange
            // Create fake data
            var gln2 = new Gln();
            gln2.Id = 2;
            gln2.FriendlyDescriptionPurpose = "two";
            _dbContext.Glns.Add(gln2);

            var gln3 = new Gln();
            gln3.Id = 3;
            gln3.FriendlyDescriptionPurpose = "three";
            _dbContext.Glns.Add(gln3);

            // Act
            _glnRepository.AssignNewGlnAssociation(1, 2);
            _glnRepository.AssignNewGlnAssociation(2, 3);
            _glnRepository.AssignNewGlnAssociation(1, 3);

            _glnRepository.RemoveGlnAssociation(1, 2);

            var result = _dbContext.GlnAssociations.Find(1);

            // Check
            Assert.Equal(null, result);
            Assert.Equal(2, _glnRepository.IGlnRegistryDb.GlnAssociations.Count());
        }
        [Fact]
        [Trait("Category", "RemoveAllGlnAssociation")]
        public void RemoveAllGlnAssociation_EnsureAssociationIsRemoved_DbSetDoesNotContainAnyAssociations()
        {
            // Arrange
            var gln2 = new Gln();
            gln2.Id = 2;
            gln2.FriendlyDescriptionPurpose = "two";
            _dbContext.Glns.Add(gln2);

            var gln3 = new Gln();
            gln3.Id = 3;
            gln3.FriendlyDescriptionPurpose = "three";
            _dbContext.Glns.Add(gln3);

            // Act
            _glnRepository.AssignNewGlnAssociation(1, 2);
            _glnRepository.AssignNewGlnAssociation(1, 3);

            _glnRepository.RemoveAllGlnAssociation(1);

            var result = _dbContext.GlnAssociations.Find(1);

            // Assert
            Assert.Equal(null, result);
            Assert.Equal(0, _glnRepository.IGlnRegistryDb.GlnAssociations.Count());
        }
        [Fact]
        [Trait("Category", "GetGlnsByGln")]
        public void GetGlnByGln_CorrectGlnIsReturned()
        {
            // Arrange
            var gln2 = new Gln()
            {
                Id = 2,
                FriendlyDescriptionPurpose = "two",
                OwnGln = "1234567890123"
            };
            
            _dbContext.Glns.Add(gln2);

            var gln3 = new Gln()
            {
                Id = 3,
                FriendlyDescriptionPurpose = "three",
                OwnGln = "3210987654321"
            };

            _dbContext.Glns.Add(gln3);

            // Act
            var resultOne = _glnRepository.GetGlnsByGln("3210987654321");

            // Assert
            Assert.Equal(3, resultOne.Id);
            Assert.Equal("three", resultOne.FriendlyDescriptionPurpose);
            Assert.Equal(0, _glnRepository.IGlnRegistryDb.GlnAssociations.Count());
        }

        private void GenerateFiveTierFamily()
        {
            // gln1 already created in the constructor _sentGln
            // So need to create five gln starting from id = 2
            var id = 2;

            var gln2 = new Gln()
            {
                Id = 2,
                FriendlyDescriptionPurpose = id.ToString(),
                OwnGln = "tier" + 2.ToString(),
                Assigned = true,
                ParentId = 1
            };

            _dbContext.Glns.Add(gln2);
            _dbContext.SaveChanges();
            // Create 2 children for tier2 GLN
            CreateNumberOfChildrenAddToParent(2, 2);

            var gln3 = new Gln()
            {
                Id = _dbContext.Glns.Count() + 1,
                FriendlyDescriptionPurpose = id.ToString(),
                OwnGln = "tier" + 3.ToString(),
                Assigned = true,
                ParentId = 2
            };

            _dbContext.Glns.Add(gln3);
            _dbContext.SaveChanges();
            // Create 3 children for tier3 GLN
            CreateNumberOfChildrenAddToParent(3, 3);

            var gln4 = new Gln()
            {
                Id = _dbContext.Glns.Count() + 1,
                FriendlyDescriptionPurpose = id.ToString(),
                OwnGln = "tier" + 4.ToString(),
                Assigned = true,
                ParentId = 3
            };

            _dbContext.Glns.Add(gln4);
            _dbContext.SaveChanges();
            // Create 4 children for tier4 GLN
            CreateNumberOfChildrenAddToParent(4, 4);

            var gln5 = new Gln()
            {
                Id = _dbContext.Glns.Count() + 1,
                FriendlyDescriptionPurpose = id.ToString(),
                OwnGln = "tier" + 5.ToString(),
                Assigned = true,
                ParentId = 4
            };

            _dbContext.Glns.Add(gln5);
            _dbContext.SaveChanges();
            // Create 5 children for tier5 GLN
            CreateNumberOfChildrenAddToParent(5, 5);

            var getGln1 = _dbContext.Glns.SingleOrDefault(g => g.OwnGln == "tier1");
            var getGln2 = _dbContext.Glns.SingleOrDefault(g => g.OwnGln == "tier2");
            var getGln3 = _dbContext.Glns.SingleOrDefault(g => g.OwnGln == "tier3");
            var getGln4 = _dbContext.Glns.SingleOrDefault(g => g.OwnGln == "tier4");
            var getGln5 = _dbContext.Glns.SingleOrDefault(g => g.OwnGln == "tier5");

            // This creates the 5 tier family structure
            getGln1.Children.Add(getGln2);
            getGln2.Children.Add(getGln3);
            getGln3.Children.Add(getGln4);
            getGln4.Children.Add(getGln5);

            _dbContext.SaveChanges();
        }

        private void CreateNumberOfChildrenAddToParent(int tier, int numberOfChildrenRequired)
        {
            var numberOfChildren = 0;
            var gln = _dbContext.Glns.SingleOrDefault(g => g.OwnGln == "tier" + tier);

            while (numberOfChildren < numberOfChildrenRequired)
            {
                var childGln = new Gln()
                {
                    Id = _dbContext.Glns.Count() + 1,
                    FriendlyDescriptionPurpose = (_dbContext.Glns.Count() + 1).ToString(),
                    OwnGln = (_dbContext.Glns.Count() + 1).ToString(),
                    ParentId = gln.Id,
                    ParentGln = gln.OwnGln,
                    ParentDescriptionPurpose = gln.FriendlyDescriptionPurpose,
                    Assigned = true
                };

                _dbContext.Glns.Add(childGln);
                gln.Children.Add(childGln);
                _dbContext.SaveChanges();

                numberOfChildren++;
            }
        }

        private void RenameGlnsFriendlyDescriptions()
        {
            foreach (var gln in _dbContext.Glns)
            {
                gln.FriendlyDescriptionPurpose = gln.Id.ToString() + "tier";
            }
        }

        [Fact]
        [Trait("Category", "GetGlnIdsForEntireFamily")]
        public void GetGlnIdsForEntireFamily_NoParentFound()
        {
            // Arrange
            var familyIds = _glnRepository.GetGlnIdsForEntireFamily(1000, new List<int>());

            // Act

            // Assert
            Assert.Equal(0, familyIds.Count());
        }

        [Theory]
        [InlineData(1, 18)] // Tier1 1 => 19 - 1 = 18
        [InlineData(2, 17)] // Tier2 2 => 19 - 2 = 17
        [InlineData(3, 14)] // Tier3 3 + 2 => 19 - 5 = 14
        [InlineData(4, 10)] // Tier4 4 + 3 + 2 => 19 - 9 = 10
        [InlineData(5, 5)] // Tier5 5 + 4 + 3 + 2 => 19 - 14 = 5
        [Trait("Category", "GetGlnIdsForEntireFamily")]
        public void GetGlnIdsForEntireFamily_AllFiveTierTest(int number, int totalIds)
        {
            // Arrange
            GenerateFiveTierFamily();
            
            var gln = _dbContext.Glns.SingleOrDefault(g => g.OwnGln == "tier" + number.ToString());

            var familyIds = _glnRepository.GetGlnIdsForEntireFamily(gln.Id, new List<int>());

            // Act

            // Assert
            Assert.Equal(totalIds, familyIds.Count);
        }

        [Fact]
        [Trait("Category", "GetGlnIdsOfParentsChildren")]
        public void GetGlnIdsOfParentsChildren_NoParent()
        {
            // Arrange
            var childrenIds = _glnRepository.GetGlnIdsOfParentsChildren(1000);

            // Act

            // Assert
            Assert.Equal(0, childrenIds.Count);
        }

        [Fact]
        [Trait("Category", "GetGlnIdsOfParentsChildren")]
        public void GetGlnIdsOfParentsChildren_ReturnChildrenIds()
        {
            // Arrange
            GenerateFiveTierFamily();
            
            var gln = _dbContext.Glns.SingleOrDefault(g => g.OwnGln == "tier5");

            var childrenIds = _glnRepository.GetGlnIdsOfParentsChildren(gln.Id);

            // Act

            // Assert
            Assert.Equal(5, childrenIds.Count);
        }

        [Fact]
        [Trait("Category", "GetGlnsByQuery")]
        public void GetGlnsByQuery_SearchTermIncluded()
        {
            // If search term included then returned GLN's should be ordered by parent description
            // Arrange
            var gln = _dbContext.Glns.SingleOrDefault(g => g.OwnGln == "tier1");

            GenerateFiveTierFamily();
            RenameGlnsFriendlyDescriptions();

            var query = new GlnQuery()
            {
                ParentGln = gln.OwnGln,
                ChildrenOnly = false,
                SearchTerm = "tier",
                Functional = false,
                Legal = false,
                Digital = false,
                Physical = false,
                Private = false,
                Public = false,
                AllStatus = false
            };

            // Act
            var result = _glnRepository.GetGlnsByQuery(query);

            // Assert

        }
    }
}
