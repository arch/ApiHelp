import { Injectable } from '@angular/core';
import { Headers, RequestOptions, Response, Http } from '@angular/http';
import { Observable } from 'rxjs/Observable';

@Injectable()
export class ResourceService {
    private url = window.location.origin + "/api/help";

    constructor(private http:Http) { }
    
    GetData():Observable<any>{
        return this.http.get(this.url)
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