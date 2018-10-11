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
import { IAdditionalContactQuery } from '../models/additional-contact-query';
import { IGlnQueryResult } from '../models/gln-query-result';
import { IAdditionalContact } from '../models/additional-contact';

@Injectable()
export class AdditionalContactService {
    private baseUrl: string;

    constructor(private _http: HttpClient, @Inject(API_BASE_URL) baseUrl: String) {
        this.baseUrl = baseUrl + '/api/';
    }

    getAllAdditionalContacts(): Observable<IAdditionalContact[]> {
        return this._http.get(`${this.baseUrl}gln-additional-contacts`).catch(this.handleError);
    }

    getAdditionalContactById(id: number): Observable<IAdditionalContact> {
        return this._http.get(`${this.baseUrl}gln-additional-contacts/${id}`).catch(this.handleError);
    }

    getAdditionalContactsBySearchTerm(searchTerm: string): Observable<IAdditionalContact[]> {
        return this._http.get(`${this.baseUrl}${searchTerm}`).catch(this.handleError);
    }

    getAdditionalContactsTakeBySearchTerm(searchTerm: string, take: number): Observable<IAdditionalContact[]> {
        return this._http.get(`${this.baseUrl}gln-additional-contacts/take/${take}/search/${searchTerm}`).catch(this.handleError);
    }

    getAdditionalContactQueryResults(queryObject: IAdditionalContactQuery): Observable<IGlnQueryResult> {
        return this._http.post(`${this.baseUrl}gln-additional-contacts-query`, queryObject).catch(this.handleError);
    }

    addAdditionalContact(additionalContact: IAdditionalContact): Observable<IAdditionalContact> {
        return this._http.post(`${this.baseUrl}gln-additional-contacts/add-new-additional-contact/`, additionalContact).catch(this.handleError);
    }

    updateAdditionalContact(additionalContactToUpdate: IAdditionalContact): Observable<IAdditionalContact> {
        return this._http.put(`${this.baseUrl}gln-update-additional-contact`, additionalContactToUpdate).catch(this.handleError);
    }

    deactivateAdditionalContact(toDeactivateId: number, toReplaceId: number): Observable<IAdditionalContact> {
        return this._http.put(`${this.baseUrl}gln-additional-contacts/to-deactivate-id/${toDeactivateId}/to-replace-id/${toReplaceId}`, toDeactivateId).catch(this.handleError);
    }

    private handleError(error: any) {
        console.error(error);
        return Observable.throw(error.json || 'Server Error');
    }
}