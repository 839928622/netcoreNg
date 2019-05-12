import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { environment } from './../src/environments/environment';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { User } from 'src/app/_models/User';
import { PaginatedResult } from 'src/app/_models/Pagination';
import { map } from 'rxjs/operators';
import { Message } from 'src/app/_models/Message';


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

getUsers(page?, itemsPerPage?, userParams?, likesParam?): Observable<PaginatedResult<User[]>> { // 返回类型是Observable<User>
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
    params = params.append('orderBy', userParams.orderBy);
  }

  if ( likesParam === 'Likers') {
  params = params.append('likers', 'true');
  }

  if (likesParam === 'Likees') {
    params = params.append('likees', 'true');
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

sendLike(id: number, recipientId: number) { // 第二个参数：id是列表中展示的用户的id，第一个参数是当前账户所有者的id
 return this.http.post(this.baseUrl + 'users/' + id + '/like/' + recipientId, {});
}

// 获取通信消息的方法

getMessages(id: number, page?, itemsPerPage?, messageContainer?) {
const paginatedResult: PaginatedResult<Message[]> = new PaginatedResult<Message[]>();

let params = new HttpParams();

params = params.append('MessageContainer', messageContainer);

if (page != null && itemsPerPage != null) {
  params = params.append('pageNumber', page);
  params = params.append('pageSize', itemsPerPage);
}

return this.http.get<Message[]>(this.baseUrl + 'users/' + id + '/messages', {observe: 'response', params})
 .pipe(
   map(response => {
     paginatedResult.result = response.body;
     if (response.headers.get('Pagination') !== null) {
      paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
    }
     return paginatedResult ;
   })
 );
}

getMessageThread(id: number, recipientId: number) {
 return this.http.get<Message[]>(this.baseUrl + 'users/' + id + '/messages/thread/' + recipientId);
}
}
