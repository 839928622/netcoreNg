import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { from } from 'rxjs';
import { JwtHelperService } from '@auth0/angular-jwt';
import { environment } from 'src/environments/environment';

@Injectable({ // Injectable 装饰器 允许我们注入别的东西到 当前这个service
  providedIn: 'root'
})
export class AuthService {

  baseUrl = environment.apiUrl + 'auth/'; // import { JwtHelperService } from '@auth0/angular-jwt';
  jwtHelper = new JwtHelperService();
  decodedToken: any; // 用于解码token中的用户信息

constructor(private http: HttpClient) {
 }

login(model: any) {
  return this.http.post(this.baseUrl + 'login', model)
        .pipe(
           map((response: any) => {
                const user = response;
                if (user) {
                  localStorage.setItem('token', user.token);
                  this.decodedToken = this.jwtHelper.decodeToken(user.token); // 解码
                  console.log(this.decodedToken); // 可以暂时输入解码的信息看是否正确
                }

               })
        );
}

register(model: any) {
 return this.http.post(this.baseUrl + 'register', model);
}

loggedIn() {
const token = localStorage.getItem('token');
return !this.jwtHelper.isTokenExpired(token); // 检查token是否过期，返回布尔类型
}
}
