import { Component, OnInit, Input } from '@angular/core';
import { Message } from 'src/app/_models/Message';
import { UserService } from '_services/User.service';
import { AuthService } from '_services/auth.service';
import { AlertifyService } from '_services/alertify.service';
import { tap } from 'rxjs/operators';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {
@Input() recipientId: number;
messages: Message[];
newMessage: any = {};
  constructor(private userService: UserService,
              private authService: AuthService,
              private alertify: AlertifyService) { }

  ngOnInit() {
    this.loadMessages();
  }

  loadMessages() {
    const currentUserId = +this.authService.decodedToken.nameid; // + 符号起转换成数字的作用
    this.userService.getMessageThread(this.authService.decodedToken.nameid,
      this.recipientId)
      .pipe(
        tap(messages => {
          // tslint:disable-next-line:prefer-for-of
          for (let i = 0 ; i < messages.length; i++) { // 选出数组中未读的消息
          if (messages[i].isRead === false
            && messages[i].recipientId === currentUserId) {
          this.userService.MarkAsRead(currentUserId, messages[i].id);
          }
          }
        })
      )
      .subscribe(message => {
      this.messages = message;
      }, error => {
      this.alertify.error(error);
      });
  }

  sendMessage() {
  this.newMessage.recipientId = this.recipientId; // 父元素输入进来的
  this.userService.sendMessage(this.authService.decodedToken.nameid, this.newMessage).subscribe(
    (message: Message) => {
    this.messages.unshift(message);
    this.newMessage.content = '';
    }, error => {
      this.alertify.error(error);
    }
  );
  }
}
