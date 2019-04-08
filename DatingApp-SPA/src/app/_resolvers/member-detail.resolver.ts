import { AlertifyService } from './../../../_services/alertify.service';
import { UserService } from './../../../_services/User.service';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { User } from './../_models/User';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable()
export class MemberDetailResolver implements Resolve<User> {
    constructor(private userService: UserService, private router: Router, private alertify: AlertifyService) {}

    resolve(route: ActivatedRouteSnapshot): Observable<User> {
        return this.userService.getUser(route.params.id).pipe(
            catchError(error => {
               this.alertify.error('正在加载数据中') ;
               this.router.navigate(['/members']);
               return of(null);
            })
        );
    }
}
