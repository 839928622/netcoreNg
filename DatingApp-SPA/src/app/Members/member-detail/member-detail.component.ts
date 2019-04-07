import { User } from './../../_models/User';
import { AlertifyService } from './../../../../_services/alertify.service';
import { UserService } from './../../../../_services/User.service';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit {
  user: User;

  constructor(private userService: UserService, private alertify: AlertifyService, private route: ActivatedRoute) { }

  ngOnInit() {
    this.loadUser();
  }

  loadUser() {
  // tslint:disable-next-line:no-string-literal
  this.userService.getUser( + this.route.snapshot.params['id']).subscribe((user: User) => {
    this.user = user;
  }, error => {
    this.alertify.error('获取该用户详情失败' + error);
  }
  );
  }
}
