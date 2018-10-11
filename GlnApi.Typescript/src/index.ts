//Global Location Number (GLN) Registry API
//Copyright (C) 2018  University Hospitals Plymouth NHS Trust
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
export { GlnClientShared } from './gln-client-shared.module';

export { API_BASE_URL } from './base-url';

export { GlnService } from './services/gln.service';
export { AddressService } from './services/address.service';
export { PrimaryContactService } from './services/primary-contact.service';
export { AdditionalContactService } from './services/additional-contact.service';
export { IprService } from './services/ipr.service';
export { GlnTagService } from './services/gln-tag.service';
export { GlnTagTypeService } from './services/gln-tag-type.service';

export { IGln, Gln } from './models/gln';
export { IContact, Contact } from './models/contact';
export { IAdditionalContact, AdditionalContact } from './models/additional-contact';
export { IAddress, Address } from './models/address';
export { IAddressQuery, AddressQuery } from './models/address-query';
export { IContactQuery, ContactQuery } from './models/contact-query';
export { IGlnQuery, GlnQuery } from './models/gln-query';
export { IGlnQueryResult, GlnQueryResult } from './models/gln-query-result';
export { IGlnSummary, GlnSummary } from './models/gln-summary';
export { IIpr, Ipr } from './models/ipr';
export { IGlnTag, GlnTag } from './models/gln-tag';
export { IGlnTagType, GlnTagType } from './models/gln-tag-type';
export { IGlnTagTypeQuery, GlnTagTypeQuery } from './models/gln-tag-type-query';
