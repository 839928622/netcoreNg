import { AlertifyService } from '../../../_services/alertify.service';
import { UserService } from '../../../_services/User.service';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { User } from '../_models/User';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Message } from '../_models/Message';
import { AuthService } from '_services/auth.service';

@Injectable()
export class MessagesResolver implements Resolve<Message[]> {
    pageNumber = 1;
    pageSize = 5 ;
    messageContainer = 'Unread' ;
    constructor(private userService: UserService,
                private router: Router,
                private alertify: AlertifyService,
                private authService: AuthService
                ) {}

    resolve(route: ActivatedRouteSnapshot): Observable<Message[]> {
        // tslint:disable-next-line:max-line-length
        return this.userService.getMessages(
            this.authService.decodedToken.nameid, // 当前用户id
            this.pageNumber , // 页数
            this.pageSize, // 页面大小
            this.messageContainer // 消息容器
            ).pipe( // 使用resolver的时候，不需要subscribe,它自动subscribe.使用pipe是想要捕获其中的错误 resolver:溶剂；分解器[电子]
            catchError(error => {
               this.alertify.error('从服务器获取消息失败' + error) ;
               this.router.navigate(['/home']); // 这里如果还路由至member的话就会触发死循环
               return of(null);
            })
        );
    }
}
