import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { environment } from './../src/environments/environment';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { User } from 'src/app/_models/User';
import { PaginatedResult } from 'src/app/_models/Pagination';
import { map } from 'rxjs/operators';

// const httpOptions = {
//   headers: new HttpHeaders({
//     Authorization: 'Bearer ' + localStorage.getItem('token')
//   })
// };

@Injectable({
  providedIn: 'root'
})
export class UserService {
baseUrl = environment.apiUrl;
constructor(private http: HttpClient) { }

getUsers(page?, itemsPerPage?, userParams?): Observable<PaginatedResult<User[]>> { // 返回类型是Observable<User>
  const paginatedResult: PaginatedResult<User[]> = new PaginatedResult<User[]>();
  let params = new HttpParams();

  if (page != null && itemsPerPage != null) {
    params = params.append('pageNumber', page);
    params = params.append('pageSize', itemsPerPage);
  }

  if (userParams != null) {
    params = params.append('minAge', userParams.minAge);
    params = params.append('maxAge', userParams.maxAge);
    params = params.append('gender', userParams.gender);
  }

  return this.http.get<User[]>(this.baseUrl + 'users', { observe: 'response', params})
  .pipe(
  map(response => {
    paginatedResult.result = response.body;
    if (response.headers.get('Pagination') != null) {
      paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
    }
    return paginatedResult;
  })
  );
}

getUser(id): Observable<User> {
  return this.http.get<User>(this.baseUrl + 'users/' + id);
}

updateUser(id: number, user: User) {
 return this.http.put(this.baseUrl + 'users/' + id, user);
}

setMainPhoto(userId: number, id: number) { // 前一个是用户的id 后一个是照片的id
 // tslint:disable-next-line:max-line-length
 return this.http.post(this.baseUrl + 'users/' + userId + '/photos/' + id + '/setMain', {}); // {}表示一个空对象，由于本次post请求不携带对象，因此设置为空，携带的信息主要在url里
}

deletePhoto(userId: number, id: number) {
  return this.http.delete(this.baseUrl + 'users/' + userId + '/photos/' + id);
}

}
