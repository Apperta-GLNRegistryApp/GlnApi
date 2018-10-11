//Global Location Number (GLN) Registry API
//Copyright (C) 2018  University Hospitals Plymouth NHS Trust
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System.Collections.Generic;

namespace GlnApi.Models.ViewModels
{
    public class GlnViewModel
    {
        public GlnViewModel()
        {
            AdditionalContacts = new List<AdditionalContactViewModel>();
            Extensions = new List<GlnExtensionViewModel>();
            Tags = new List<GlnTagViewModel>();
        }
        public int Id { get; set; }
        public string FriendlyDescriptionPurpose { get; set; }
        public bool Active { get; set; }
        public string OwnGln { get; set; }
        public string ParentGln { get; set; }
        public System.DateTime CreationDate { get; set; }
        public bool UseParentAddress { get; set; }
        public bool Verified { get; set; }
        public bool FunctionalType { get; set; }
        public bool LegalType { get; set; }
        public bool DigitalType { get; set; }
        public bool PhysicalType { get; set; }
        public bool Public { get; set; }
        public bool Assigned { get; set; }
        public int AddressId { get; set; }
        public int ContactId { get; set; }
        public bool TrustActive { get; set; }
        public string SuspensionReason { get; set; }
        public int Version { get; set; }
        public int NumberOfChildren { get; set; }
        public string SuspendedBy { get; set; }
        public System.DateTime SuspensionDate { get; set; }
        public bool Primary { get; set; }
        public string ParentDescriptionPurpose { get; set; }
        public string TruthDescriptionPurpose { get; set; }
        public bool AlternativeSystemIsTruth { get; set; }
        public int Level { get; set; }
        public string Department { get; set; }
        public string Function { get; set; }
        public AddressViewModel Address { get; set; }
        public PrimaryContactViewModel PrimaryContact { get; set; }
        public ICollection<AdditionalContactViewModel> AdditionalContacts { get; set; }
        public ICollection<GlnExtensionViewModel> Extensions { get; set; }
        public ICollection<GlnTagViewModel> Tags { get; set; }
        public override string ToString()
        {
            return $"GLN Id: {Id}, Version: {Version}, {OwnGln}, {FriendlyDescriptionPurpose}.";
        }
    }
}