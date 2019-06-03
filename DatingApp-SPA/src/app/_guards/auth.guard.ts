import { AlertifyService } from './../../../_services/alertify.service';
import { AuthService } from './../../../_services/auth.service';
import { Injectable } from '@angular/core';
import { CanActivate, UrlTree, Router, ActivatedRouteSnapshot } from '@angular/router';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router, private alertify: AlertifyService) {}

  canActivate(next: ActivatedRouteSnapshot): boolean | UrlTree {
    const roles = next.firstChild.data.roles as Array<string>; // 这里会去路由，获取admin 路由里的定义的角色信息

    if (roles) {
      const match = this.authService.roleMatch(roles);
      if (match) {
        return true;
      } else {
              this.router.navigate(['members']);
              this.alertify.error('您无权进入该区域');
      }
    }

    if (this.authService.loggedIn()) {

      return true; // 如果我们返回true，那么就可以激活路由
    }
    this.alertify.error('请登录后重试！！！');
    this.router.navigate(['/home']);
    return false;
  }

}
