import { AlertifyService } from './../../../_services/alertify.service';
import { AuthService } from './../../../_services/auth.service';
import { Injectable } from '@angular/core';
import { CanActivate, UrlTree, Router } from '@angular/router';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router, private alertify: AlertifyService) {}

  canActivate(): boolean | UrlTree {
    if (this.authService.loggedIn()) {

      return true; // 如果我们返回true，那么就可以激活路由
    }
    this.alertify.error('请登录后重试！！！');
    this.router.navigate(['/home']);
    return false;
  }
  
}
