//Global Location Number (GLN) Registry API
//Copyright (C) 2018  University Hospitals Plymouth NHS Trust
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿import { Injectable, Inject } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { HttpClient } from '@phnt/summa.authentication';
import { API_BASE_URL } from './../base-url';
import { IGln } from '../models/gln';
import { IGlnSummary } from '../models/gln-summary';
import { IGlnQueryResult } from '../models/gln-query-result';
import { IGlnQuery } from '../models/gln-query';
import { IContact } from '../models/contact';

@Injectable()
export class GlnService {
    private baseUrl: string;

    constructor(private _http: HttpClient, @Inject(API_BASE_URL) baseUrl: String) {
        this.baseUrl = baseUrl + '/api/';
    }

    public getPrimaryGln(): Observable<IGln> {
        return this._http.get(`${this.baseUrl}get-gln-primary`).catch(this.handleError);
    }

    public getGlnQueryResults(queryObject: IGlnQuery): Observable<IGlnQueryResult> {
        return this._http.post(`${this.baseUrl}get-gln-query`, queryObject).catch(this.handleError);
    }

    public getAllAssignedGlns(): Observable<IGln[]> {
        return this._http.get(`${this.baseUrl}get-glns`).catch(this.handleError);
    }

    public getGlnsBySearchTerm(searchTerm: string): Observable<IGln[]> {
        return this._http.get(`${this.baseUrl}gln-search/${searchTerm}`).catch(this.handleError);
    }

    public getGlnsBySearchTermTakeAmount(takeAmount: number, searchTerm: string): Observable<IGln[]> {
        return this._http.get(`${this.baseUrl}gln-search/take/${takeAmount}/search/${searchTerm}`).catch(this.handleError);
    }

    public getNextUnassignedGln(): Observable<IGln> {
        return this._http.get(`${this.baseUrl}next-unassigned-gln`).catch(this.handleError);
    }

    public assignAddressGetNextUnassignedGln(addressId: number): Observable<IGln> {
        return this._http.get(`${this.baseUrl}change-address-get-next-unassigned-gln/${addressId}`).catch(this.handleError);
    }

    public getGlnByGln(glnNumber: string): Observable<IGln> {
        return this._http.get(`${this.baseUrl}gln-by-gln/${glnNumber}`).catch(this.handleError);
    }

    public getGlnById(id: number): Observable<IGln> {
        return this._http.get(`${this.baseUrl}gln-id/${id}`).catch(this.handleError);
    }

    public getChildrenByParentGln(parentGlnNumber: string): Observable<IGln[]> {
        return this._http.get(`${this.baseUrl}child-glns/${parentGlnNumber}`).catch(this.handleError);
    }

    public getChildrenByParentId(parentGlnId: number): Observable<IGln[]> {
        return this._http.get(`${this.baseUrl}child-glns/${parentGlnId}`).catch(this.handleError);
    }

    public getAssociatedGlnsByParentId(parentGlnId: number): Observable<IGln[]> {
        return this._http.get(`${this.baseUrl}gln-associations/${parentGlnId}`).catch(this.handleError);
    }

    public updateGln(gln: IGln): Observable<IGln> {
        return this._http.put(`${this.baseUrl}update-gln`, gln).catch(this.handleError);
    }

    public assignAdditionalContactToGln(glnId: number, additionalContactId: number): Observable<IGln> {
        return this._http.put(`${this.baseUrl}assign-additional-contact-to-gln/gln-id/${glnId}/additional-contact-id/${additionalContactId}`, glnId).catch(this.handleError);
    }

    public removeAdditionalContactFromGln(glnId: number, additionalContactId: number): Observable<IGln> {
        return this._http.put(`${this.baseUrl}remove-additional-contact-from-gln/gln-id/${glnId}/additional-contact-id/${additionalContactId}`, glnId).catch(this.handleError);
    }

    public assignPrimaryContactToGln(glnId: number, newPrimaryContact: IContact): Observable<IGln> {
        return this._http.put(`${this.baseUrl}assign-primary-contact-to-gln/${glnId}`, newPrimaryContact).catch(this.handleError);
    }

    public changeParentOnChildren(origninalParentGln: string, newParentGln: string): Observable<IGln> {
        return this._http.put(`${this.baseUrl}change-parent-on-children/orignal-parent-gln/${origninalParentGln}/new-parent-gln/${newParentGln}`, origninalParentGln).catch(this.handleError);
    }

    public changeParent(glnId: number, originalParentGln: string, newParentGln: string): Observable<IGln> {
        return this._http.put(`${this.baseUrl }${glnId}/original-parent-gln/${originalParentGln}/new-parent-gln/${newParentGln}`, glnId).catch(this.handleError);
    }

    public updateNewAssignedGln(gln: IGln): Observable<IGln> {
        return this._http.put(`${this.baseUrl }new-assigned-gln`, gln).catch(this.handleError);
    }

    public createGlnAssociation(glnIdOne: number, glnIdTwo: number): Observable<IGlnSummary[]> {
        return this._http.post(`${this.baseUrl }create-gln-association/${glnIdOne}/${glnIdTwo}`, glnIdOne).catch(this.handleError);
    }

    public removeGlnAssociation(glnIdOne: number, glnIdTwo: number): Observable<IGlnSummary[]> {
        return this._http.post(`${this.baseUrl }remove-gln-association/${glnIdOne}/${glnIdTwo}`, glnIdOne).catch(this.handleError);
    }

    private handleError(error: any) {
        console.error(error);
        return Observable.throw(error.json || 'Server Error');
    }
}

