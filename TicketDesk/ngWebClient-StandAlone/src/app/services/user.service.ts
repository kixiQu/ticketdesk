import { UserDetails } from './../models/user-details';
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpRequest, HttpResponse, HttpErrorResponse } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';
import { catchError, retry } from 'rxjs/operators';
import { ErrorObservable } from 'rxjs/observable/ErrorObservable';
import * as settings from '../app-settings';

@Injectable()
export class UserService {
  constructor(private http: HttpClient) { }

  getAdContactCardInfo(userName: string): Observable<UserDetails> {
    const url = settings.activeDirectoryUserURL + userName;
    console.warn(`getting contact info for ${userName} from ${url}`);
    return this.http.get<UserDetails>(url)
      .pipe(catchError(this.handleError));
  }

  getUserPermissions() {
    return this.http.get<{userPermissions: String}>(settings.getUserPermissions).map(res => {
      return res.userPermissions;
    });
  }

  private handleError(error: HttpErrorResponse): ErrorObservable {
    if (error.error instanceof ErrorEvent) {
      // ... this is a client side error, handle it!
      console.error(`client error occurred: ${error.error.message}`);
    } else {
      // ... this is a server error!
      console.error(`server error occurred, status code ${error.status}`);
    }
    return new ErrorObservable('Experiencing some issues, we are sorry');
  }
}
