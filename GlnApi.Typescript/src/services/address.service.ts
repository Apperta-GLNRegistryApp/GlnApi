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
import { IAddress } from '../models/address';
import { IAddressQuery } from '../models/address-query';

@Injectable()
export class AddressService {
    private baseUrl: string;

    constructor(private _http: HttpClient, @Inject(API_BASE_URL) baseUrl: String) {
        this.baseUrl = baseUrl + '/api/';
    }

    getAddressQueryResults(queryObject: IAddressQuery): Observable<IAddressQuery> {
        return this._http.post(`${this.baseUrl}get-address-query`, queryObject).catch(this.handleError);
    }

    getAddressBySearchTerm(pageNumber: number, pageSize: number, searchTerm: string): Observable<IAddress[]> {
        return this._http.get(`${this.baseUrl}gln-address-page/page-number/${pageNumber}/page-size/${pageSize}/search-term/${searchTerm}`).catch(this.handleError);
    }

    getAddressById(id: number): Observable<IAddress> {
        return this._http.get(`${this.baseUrl}gln-address/${id}`).catch(this.handleError);
    }

    getAddressByGln(gln: string): Observable<IAddress> {
        return this._http.get(`${this.baseUrl}gln-address/${gln}`).catch(this.handleError);
    }

    deactivateAddress(toDeactivateId: number, toReplaceId: number): Observable<IAddress> {
        return this._http.put(`${this.baseUrl}gln-deactivate-address/to-deactivate-id/${toDeactivateId}/to-replace-id/${toReplaceId}`, toDeactivateId).catch(this.handleError);
    }

    updateAddress(address: IAddress): Observable<IAddress> {
        return this._http.put(`${this.baseUrl}gln-update-address`, address).catch(this.handleError);
    }

    addAddress(address: IAddress): Observable<IAddress> {
        return this._http.post(`${this.baseUrl}gln-add-address`, address).catch(this.handleError);
    }

    private handleError(error: any) {
        console.error(error);
        return Observable.throw(error.json || 'Server Error');
    }
}