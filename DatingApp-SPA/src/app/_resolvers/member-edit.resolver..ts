import { AlertifyService } from '../../../_services/alertify.service';
import { UserService } from '../../../_services/User.service';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { User } from '../_models/User';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from '_services/auth.service';

@Injectable()
export class MemberEditResolver implements Resolve<User> {
    // tslint:disable-next-line:max-line-length
    constructor(private userService: UserService, private router: Router, private alertify: AlertifyService, private authService: AuthService) {}

    resolve(route: ActivatedRouteSnapshot): Observable<User> {
        // tslint:disable-next-line:max-line-length
        return this.userService.getUser(this.authService.decodedToken.nameid).pipe( // 使用resolver的时候，不需要subscribe,它自动subscribe.使用pipe是想要捕获其中的错误 resolver:溶剂；分解器[电子]
            catchError(error => {
               this.alertify.error('个人数据加载时出现问题') ;
               this.router.navigate(['/members']);
               return of(null);
            })
        );
    }
}
