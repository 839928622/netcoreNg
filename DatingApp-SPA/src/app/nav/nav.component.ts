import { AlertifyService } from './../../../_services/alertify.service';
import { AuthService } from './../../../_services/auth.service';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
 model: any = {};
 photoUrl: string;
 // tslint:disable-next-line:no-trailing-whitespace

  constructor(private authService: AuthService, private alertify: AlertifyService, private router: Router) { } // 注入我们需要的 service

  ngOnInit() {
    this.authService.currentPhotoUrl.subscribe(photoUrl => this.photoUrl = photoUrl); // 订阅currentPhotoUrl 当被订阅放发生改变，则推送至订阅方
  }

  login() {
    // console.log(this.model); 下面使用service的内部方法 login
    this.authService.login(this.model).subscribe(next => {
      this.alertify.success('登录成功');
     // console.log('登录成功');

    }, error => {
      this.alertify.error('用户名或密码不正确'); // 使用alertify的错误提示方法
      // console.log(error); // ErrorInterceptor使用之后，直接打error
    }, () => {
     this.router.navigate(['/members']); // 登录成功后路由到member
    }

    );
  }

  loggedIn() {
  //  const token = localStorage.getItem('token');
  //  return !!token; // 如果token有值，则返回true
  return this.authService.loggedIn();
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    this.authService.decodedToken = null;
    this.authService.currentUser = null;
    this.alertify.message('您已退出');
  //  console.log('登出');
    this.router.navigate(['']); // 退出后,会命中路由第一个规则，然后路由至home 主页
  }
}
