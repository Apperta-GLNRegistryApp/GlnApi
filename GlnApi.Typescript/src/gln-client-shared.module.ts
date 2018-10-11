//Global Location Number (GLN) Registry API
//Copyright (C) 2018  University Hospitals Plymouth NHS Trust
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
import { NgModule } from '@angular/core';
import { GlnService } from './services/gln.service';
import { GlnTagTypeService } from './services/gln-tag-type.service';
import { GlnTagService } from './services/gln-tag.service';
import { AddressService } from './services/address.service';
import { PrimaryContactService } from './services/primary-contact.service';
import { AdditionalContactService } from './services/additional-contact.service';
import { IprService } from './services/ipr.service';

@NgModule({
  declarations: [  
  ],
  providers: [ 
      GlnService,
      AddressService,
      PrimaryContactService,
      AdditionalContactService,
      IprService,
      GlnTagService,
      GlnTagTypeService
  ],
  exports: [  
  ],
  imports: [    
  ]
})

export class GlnClientShared { }
