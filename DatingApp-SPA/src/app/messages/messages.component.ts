import { Component, OnInit } from '@angular/core';
import { Message } from '../_models/Message';
import { Pagination, PaginatedResult } from '../_models/Pagination';
import { UserService } from '_services/User.service';
import { AuthService } from '_services/auth.service';
import { ActivatedRoute } from '@angular/router';
import { AlertifyService } from '_services/alertify.service';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {
  messages: Message[] ;
  pagination: Pagination ;
  messageContainer = 'Unread' ;

  constructor(private userService: UserService,
              private authService: AuthService,
              private route: ActivatedRoute,
              private alertify: AlertifyService) { }

  ngOnInit() {
    this.route.data.subscribe(data => { // 从路由获取数据
      this.messages = data.messages.result;
      this.pagination = data.messages.pagination;
    });
  }

  loadMessages() {
    this.userService.getMessages(this.authService.decodedToken.nameid,
      this.pagination.currentPage,
      this.pagination.itemsPerPage,
      this.messageContainer).subscribe((res: PaginatedResult<Message[]>) => {
        this.messages = res.result;
        this.pagination = res.pagination;
      },
      error => {
        this.alertify.error(error);
      });
  }

  pageChanged(event: any): void { // 页面改变事件
    this.pagination.currentPage = event.page;
    this.loadMessages();
  }

 deleteMessage(id: number) {
   this.alertify.confirm('确定要删除该消息吗?', () => {
   this.userService.deleteMessage(id, this.authService.decodedToken.nameid).subscribe(
     () => {
       this.messages.splice(this.messages.findIndex(m => m.id === id), 1);
       this.alertify.success('消息已删除');
     }, error => {
       this.alertify.error('消息删除失败');
     }
   );
   });
 }
}
