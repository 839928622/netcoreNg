import { Component, OnInit } from '@angular/core';
import { User } from '../_models/User';
import { Pagination, PaginatedResult } from '../_models/Pagination';
import { AuthService } from '_services/auth.service';
import { UserService } from '_services/User.service';
import { ActivatedRoute } from '@angular/router';
import { AlertifyService } from '_services/alertify.service';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit {
  users: User[];
  pagination: Pagination;
  likesParam: string;

  constructor(private authService: AuthService
            , private userService: UserService
            , private route: ActivatedRoute
            , private alertify: AlertifyService ) { }

  ngOnInit() {
    this.route.data.subscribe(data => { // 通过路由，可以拿到resolver里的信息
      this.users = data.users.result;
      this.pagination = data.users.pagination;
    });
    this.likesParam = 'Likers' ;
  }

  loadUsers() {
    this.userService.getUsers(this.pagination.currentPage, this.pagination.itemsPerPage, null, this.likesParam)
    .subscribe((res: PaginatedResult<User[]>) => { // 把从API获取到的对象转换为User数组
      this.users = res.result;
      this.pagination = res.pagination;
    }, error => {
     this.alertify.error('获取用户列表失败' + error);
    }
    );
   }

   pageChanged(event: any ): void {
    this.pagination.currentPage = event.page;
    this.loadUsers();
    }
}
