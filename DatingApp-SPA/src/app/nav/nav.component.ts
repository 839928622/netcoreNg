import { AuthService } from './../../../_services/auth.service';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
 model: any = {};
  constructor(private authService: AuthService) { } // 注入我们需要的 service

  ngOnInit() {
  }

  login() {
    // console.log(this.model); 下面使用service的内部方法 login
    this.authService.login(this.model).subscribe(next => {
     console.log('登录成功');

    }, error => {
      console.log('登录失败');
    }
    );
  }

  loggedIn() {
   const token = localStorage.getItem('token');
   return !!token; // 如果token有值，则返回true
  }

  logout() {
    localStorage.removeItem('token');
    console.log('登出');
  }
}
