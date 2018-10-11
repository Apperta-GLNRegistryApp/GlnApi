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
import { IContactQuery } from '../models/contact-query';
import { IGlnQueryResult } from '../models/gln-query-result';
import { IContact } from '../models/contact';

@Injectable()
export class PrimaryContactService {
    private baseUrl: string;

    constructor(private _http: HttpClient, @Inject(API_BASE_URL) baseUrl: String) {
        this.baseUrl = baseUrl + '/api/';
    }

    getAllPrimaryContacts(): Observable<IContact[]> {
        return this._http.get(`${this.baseUrl}gln-primary-contacts`).catch(this.handleError);
    }

    getAllPrimaryContactsExcludingId(idToExclude: number): Observable<IContact[]> {
        return this._http.get(`${this.baseUrl}gln-primary-contacts/exclude-primary-contact-id/${idToExclude}`).catch(this.handleError);
    }

    getPrimaryContactById(id: number): Observable<IContact> {
        return this._http.get(`${this.baseUrl}gln-primary-contact-id/${id}`).catch(this.handleError);
    }

    getAllPrimaryContactsBySearchTerm(searchTerm: string): Observable<IContact[]> {
        return this._http.get(`${this.baseUrl}gln-primary-contacts-search/${searchTerm}`).catch(this.handleError);
    }

    getPrimaryContactQueryResults(queryObject: IContactQuery): Observable<IGlnQueryResult> {
        return this._http.post(`${this.baseUrl}gln-primary-contacts-query`, queryObject).catch(this.handleError);
    }

    addPrimaryContact(contact: IContact): Observable<IContact> {
        return this._http.post(`${this.baseUrl}gln-primary-contacts/add-new-primary-contact`, contact).catch(this.handleError);
    }

    updatePrimaryContact(contactToUpdate: IContact): Observable<IContact> {
        return this._http.put(`${this.baseUrl}gln-update-primary-contact`, contactToUpdate).catch(this.handleError);
    }

    deactivatePrimaryContact(toDeactivateId: number, toReplaceId: number): Observable<IContact> {
        return this._http.put(`${this.baseUrl}gln-primary-contacts/to-deactivate-id/${toDeactivateId}/to-replace-id/${toReplaceId}`, toDeactivateId).catch(this.handleError);
    }

    private handleError(error: any) {
        console.error(error);
        return Observable.throw(error.json || 'Server Error');
    }
}