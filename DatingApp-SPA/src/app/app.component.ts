import { AuthService } from './../../_services/auth.service';
import { Component, OnInit } from '@angular/core';
import { JwtHelperService } from '@auth0/angular-jwt';
import { User } from './_models/User';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  jwtHelper = new JwtHelperService();
  title = 'DatingApp-SPA';

  constructor(private authService: AuthService) { }

  ngOnInit() {
    const token = localStorage.getItem('token');
    const user: User = JSON.parse(localStorage.getItem('user')); // 获取本地存储的user，之后再转换为json对象，最后赋值给user
    if (token) {
    this.authService.decodedToken = this.jwtHelper.decodeToken(token); // this.authService.decodedToken提高到全局 app-root
    }

    if (user) {
      this.authService.currentUser = user;
      this.authService.changeMemberPhoto(user.photoUrl);
    }
  }
}


