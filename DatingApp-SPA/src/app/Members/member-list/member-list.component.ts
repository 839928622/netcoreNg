import { AlertifyService } from '../../../../_services/alertify.service';
import { UserService } from '../../../../_services/User.service';
import { Component, OnInit } from '@angular/core';
import { User } from '../../_models/User';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
  users: User[];

  constructor(private userService: UserService, private alertify: AlertifyService) { }

  ngOnInit() {
    this.loadUsers(); // 组件初始化时加载用户信息
  }

  loadUsers() {
   this.userService.getUsers().subscribe((users: User[]) => { // 把从API获取到的对象转换为User数组
     this.users = users;
   }, error => {
    this.alertify.error('获取用户列表失败' + error);
   }
   );
  }

}
