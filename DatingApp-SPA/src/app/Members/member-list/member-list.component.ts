import { AlertifyService } from '../../../../_services/alertify.service';
import { UserService } from '../../../../_services/User.service';
import { Component, OnInit } from '@angular/core';
import { User } from '../../_models/User';
import { ActivatedRoute } from '@angular/router';
import { Pagination, PaginatedResult } from 'src/app/_models/Pagination';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
  users: User[];
  pagination: Pagination;
  user: User = JSON.parse(localStorage.getItem('user'));
  genderList = [{value: 'male', display: '男士'}, {value: 'female', display: '女士'}];
  userParams: any = {}; // 定义一个any对象，

  constructor(private userService: UserService, private alertify: AlertifyService, private route: ActivatedRoute) { }

  ngOnInit() {
    // this.loadUsers(); // 组件初始化时加载用户信息
    this.route.data.subscribe(data => {
      this.users = data.users.result; // data.users 应该是路由上定义的，或者尽量与之同名方便理解
      this.pagination = data.users.pagination; // 获取分页的用户
    });

    this.userParams.gender = this.user.gender === 'female' ? 'male' : 'female' ;
    this.userParams.minAge = 18 ;
    this.userParams.maxAge = 99 ; // 初始筛选条件
  }

  resetFilters() {
    this.userParams.gender = this.user.gender === 'female' ? 'male' : 'female' ;
    this.userParams.minAge = 18 ;
    this.userParams.maxAge = 99 ; // 重置 初始筛选条件
    this.loadUsers();
  }

  loadUsers() {
   this.userService.getUsers(this.pagination.currentPage, this.pagination.itemsPerPage, this.userParams)
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
