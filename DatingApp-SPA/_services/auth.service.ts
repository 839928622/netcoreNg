import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { from } from 'rxjs';

@Injectable({ // Injectable 装饰器 允许我们注入别的东西到 当前这个service
  providedIn: 'root'
})
export class AuthService {

  baseUrl = 'http://localhost:4200/api/auth/';
constructor(private http: HttpClient) 
{
 
}

login(model: any) {
  return this.http.post(this.baseUrl + 'login', model)
        .pipe(
           map((response: any) => {
                const user = response;
                if (user) {
                  localStorage.setItem('token', user.token);
                }

               })
        );
}
}
