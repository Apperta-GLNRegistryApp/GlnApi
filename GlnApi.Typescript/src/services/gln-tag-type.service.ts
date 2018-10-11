//Global Location Number (GLN) Registry API
//Copyright (C) 2018  University Hospitals Plymouth NHS Trust
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
import { Injectable, Inject } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { HttpClient } from '@phnt/summa.authentication';
import { API_BASE_URL } from './../base-url';
import { IGlnSummary } from '../models/gln-summary';
import { IGlnTagType } from '../models/gln-tag-type';
import { IGlnTagTypeQuery } from '../models/gln-tag-type-query';
import { IGlnQueryResult } from './../models/gln-query-result';
@Injectable()

export class GlnTagTypeService {
    private baseUrl: string;

    constructor(private _http: HttpClient, @Inject(API_BASE_URL) baseUrl: String) {
        this.baseUrl = baseUrl + '/api/';
    }

    getTagTypes(): Observable<IGlnTagType[]> {
        return this._http.get(`${this.baseUrl}get/gln-tag-types`).catch(this.handleError);
    }

    getTagTypeById(id: number): Observable<IGlnTagType> {
        return this._http.get(`${this.baseUrl}get/gln-tag-types/id/${id}`).catch(this.handleError);
    }

    getTagTypesQuery(tagTypeQuery: IGlnTagTypeQuery): Observable<IGlnQueryResult> {
        return this._http.post(`${this.baseUrl}get-gln-tag-type-query`, tagTypeQuery).catch(this.handleError);
    }
    
    getTagTypeByGln(gln: string): Observable<IGlnTagType> {
        return this._http.get(`${this.baseUrl}get/gln-tag-types/gln/${gln}`).catch(this.handleError);
    }
    
    createTagTypes(tagType: IGlnTagType): Observable<IGlnTagType> {
        return this._http.post(`${this.baseUrl}post/gln-tag-types`, tagType).catch(this.handleError);
    }

    updateTagType(tag: IGlnTagType): Observable<IGlnTagType> {
        return this._http.put(`${this.baseUrl}put/gln-tag-types/${tag.GlnTagTypeId}`, tag).catch(this.handleError);
    }

    deleteTagTypeById(id: number): Observable<IGlnTagType> {
        return this._http.delete(`${this.baseUrl}delete/gln-tag-types/id/${id}`).catch(this.handleError);
    }

    private handleError(error: any) {
        console.error(error);
        return Observable.throw(error.json || 'Server Error');
    }
}