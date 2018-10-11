//Global Location Number (GLN) Registry API
//Copyright (C) 2018  University Hospitals Plymouth NHS Trust
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿import { IGlnTagType, GlnTagType } from './gln-tag-type';

export class GlnTag implements IGlnTag {
    GlnTagId: number;
    GlnTagTypeId: number;
    GlnId: number;
    TypeKey: string;
    Active: boolean;
    GlnTagType: GlnTagType = new GlnTagType();

}

export interface IGlnTag {
    GlnTagId: number;
    GlnTagTypeId: number;
    GlnId: number;
    TypeKey: string;
    Active: boolean;
    GlnTagType: IGlnTagType;

}

