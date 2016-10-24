import { Injectable } from '@angular/core';
import { Headers, RequestOptions, Response, Http } from '@angular/http';
import { Observable } from 'rxjs/Observable';

@Injectable()
export class PointService {
    private url = window.location.origin + "/api/help/get";

    constructor(private http:Http) { }
    
    GetData(para:string):Observable<any>{
        return this.http.get(this.url,{search:para})
        .map(this.extractData)
        .catch(this.handleError);
    }
    
    private extractData(res: Response) {
        let body = res.json();
        return body || {};
    }

    private handleError(error: any) {
        // In a real world app, we might use a remote logging infrastructure
        // We'd also dig deeper into the error to get a better message
        let errMsg = (error.message) ? error.message :
            error.status ? `${error.status} - ${error.statusText}` : 'Server error';
        console.error(errMsg); // log to console instead
        return Observable.throw(errMsg);
    }
}