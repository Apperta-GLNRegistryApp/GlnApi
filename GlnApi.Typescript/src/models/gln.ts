//Global Location Number (GLN) Registry API
//Copyright (C) 2018  University Hospitals Plymouth NHS Trust
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
import { GlnSummary, IGlnSummary } from './gln-summary';
import { AdditionalContact, IAdditionalContact } from './additional-contact';
import { Address, IAddress } from './address';
import { Contact, IContact } from './contact';
import { GlnTag, IGlnTag } from './gln-tag';

export class Gln implements IGln {

    Id: number;
    Assigned: boolean;
    FriendlyDescriptionPurpose: string;
    Active: boolean;
    OwnGln: string;
    ParentGln: string;
    ParentDescriptionPurpose: string;
    FunctionalType: boolean;
    LegalType: boolean;
    DigitalType: boolean;
    PhysicalType: boolean;
    CreationDate: Date;
    UseParentAddress: boolean;
    Verified: boolean;
    Public: boolean;
    TrustActive: boolean;
    SuspensionReason: string;
    SuspendedBy: string;
    SuspensionDate: Date;
    ContactId: number;
    AddressId: number;
    Version: number;
    NumberOfChildren: number;
    Primary: boolean;
    TruthDescriptionPurpose: string;
    AlternativeSystemIsTruth: boolean;
    Department: string;
    Function: string;
    TierLevel: number;

    Address: Address = new Address();
    PrimaryContact: Contact = new Contact();
    AdditionalContacts: AdditionalContact[];
    Children: GlnSummary[];
    Tags: GlnTag[];
}

export interface IGln {

    Id: number;
    Assigned: boolean;
    FriendlyDescriptionPurpose: string;
    TruthDescriptionPurpose: string;
    Active: boolean;
    OwnGln: string;
    ParentGln: string;
    ParentDescriptionPurpose: string;
    FunctionalType: boolean;
    LegalType: boolean;
    DigitalType: boolean;
    PhysicalType: boolean;
    CreationDate: Date;
    UseParentAddress: boolean;
    Verified: boolean;
    Public: boolean;
    TrustActive: boolean;
    SuspensionReason: string;
    SuspendedBy: string;
    SuspensionDate: Date;
    ContactId: number;
    AddressId: number;
    Version: number;
    NumberOfChildren: number;
    Primary: boolean;
    AlternativeSystemIsTruth: boolean;
    Department: string;
    Function: string;
    TierLevel: number;

    Address: IAddress;
    PrimaryContact: IContact;
    AdditionalContacts: IAdditionalContact[];
    Children: IGlnSummary[];
    Tags: IGlnTag[];
}


