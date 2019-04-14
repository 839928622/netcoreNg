
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BehaviorSubject } from 'rxjs';
import { JwtHelperService } from '@auth0/angular-jwt';
import { environment } from 'src/environments/environment';
import { User } from 'src/app/_models/User';

@Injectable({ // Injectable 装饰器 允许我们注入别的东西到 当前这个service
  providedIn: 'root'
})
export class AuthService {

  baseUrl = environment.apiUrl + 'auth/'; // import { JwtHelperService } from '@auth0/angular-jwt';
  jwtHelper = new JwtHelperService();
  decodedToken: any; // 用于解码token中的用户信息
  currentUser: User; // 用户存储登录人的身份信息
  photoUrl = new BehaviorSubject<string>('/DatingApp-SPA/src/assets/user.png');
  currentPhotoUrl = this.photoUrl.asObservable();

constructor(private http: HttpClient) {
 }

 changeMemberPhoto(photoUrl: string) {
   this.photoUrl.next(photoUrl);
 }

login(model: any) {
  return this.http.post(this.baseUrl + 'login', model)
        .pipe(
           map((response: any) => {
                const user = response; // response 是api返回的对象
                if (user) {
                  localStorage.setItem('token', user.token);
                  // tslint:disable-next-line:max-line-length
                  localStorage.setItem('user', JSON.stringify(user.user)); // 第二个参数需要的是string，然而api返回的是对象，即model中的User对象，所以需要转换成字符串。最后存储在localstorage
                  this.currentUser = user.user; // 这里如果希望该值能够一直被保存，则需要保存到app-component
                  this.decodedToken = this.jwtHelper.decodeToken(user.token); // 解码
                //  console.log(this.decodedToken); // 可以暂时输入解码的信息看是否正确
                  // tslint:disable-next-line:max-line-length
                  this.changeMemberPhoto(this.currentUser.photoUrl); // 当用户登录后，就替换掉初始值 之后currentPhotoUrl就可以被订阅，当currentPhotoUrl更新，则注入该值的组件也随着更新
                }

               })
        );
}

register(model: User) {
 return this.http.post(this.baseUrl + 'register', model);
}

loggedIn() {
const token = localStorage.getItem('token');
return !this.jwtHelper.isTokenExpired(token); // 检查token是否过期，返回布尔类型
}
}
