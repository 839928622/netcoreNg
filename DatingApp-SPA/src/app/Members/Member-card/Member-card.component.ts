import { Component, OnInit, Input } from '@angular/core';
import { User } from 'src/app/_models/User';
import { AuthService } from '_services/auth.service';
import { UserService } from '_services/User.service';
import { AlertifyService } from '_services/alertify.service';

@Component({
  selector: 'app-member-card',
  templateUrl: './Member-card.component.html',
  styleUrls: ['./Member-card.component.css']
})
export class MemberCardComponent implements OnInit {
@Input() user: User; // 我们想要从父组件上获取
  constructor(private authService: AuthService,
              private userService: UserService,
              private alertify: AlertifyService) { }

  ngOnInit() {
  }

  sendLike(id: number) {
    this.userService.sendLike(this.authService.decodedToken.nameid, id)
    .subscribe(data => { // 第二个参数：id是列表中展示的用户的id，第一个参数是当前账户所有者的id
      this.alertify.success('你关注' + this.user.knownAs + '成功');
    }, error => {
    this.alertify.error('关注失败，' + error);
    });
  }
}
