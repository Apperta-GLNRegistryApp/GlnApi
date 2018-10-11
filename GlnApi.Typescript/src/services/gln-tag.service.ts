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
import { IGlnTag } from '../models/gln-tag';

@Injectable()
export class GlnTagService {
    private baseUrl: string;

    constructor(private _http: HttpClient, @Inject(API_BASE_URL) baseUrl: String) {
        this.baseUrl = baseUrl + '/api/';
    }

    getTags(): Observable<IGlnTag[]> {
        return this._http.get(`${this.baseUrl}get/gln-tags`).catch(this.handleError);
    }

    getTagById(id: number): Observable<IGlnTag> {
        return this._http.get(`${this.baseUrl}get/gln-tags/id/${id}`).catch(this.handleError);
    }

    getTagByGlnId(glnId: number): Observable<IGlnTag> {
        return this._http.get(`${this.baseUrl}get/gln-tags/gln-id/${glnId}`).catch(this.handleError);
    }

    getTagByGln(gln: string): Observable<IGlnTag> {
        return this._http.get(`${this.baseUrl}get/gln-tags/gln/${gln}`).catch(this.handleError);
    }

    updateTag(tag: IGlnTag): Observable<IGlnTag> {
        return this._http.put(`${this.baseUrl}put/gln-tags`, tag).catch(this.handleError);
    }

    createTag(tag: IGlnTag): Observable<IGlnTag> {
        return this._http.post(`${this.baseUrl}post/gln-tags`, tag).catch(this.handleError);
    }

    deleteTagById(id: number): Observable<IGlnTag> {
        return this._http.delete(`${this.baseUrl}delete/gln-tags/id/${id}`).catch(this.handleError);
    }

    private handleError(error: any) {
        console.error(error);
        return Observable.throw(error.json || 'Server Error');
    }
}