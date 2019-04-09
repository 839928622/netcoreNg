import { ActivatedRoute } from '@angular/router';
import { Component, OnInit, ViewChild, HostListener } from '@angular/core';
import { User } from 'src/app/_models/User';
import { AlertifyService } from '_services/alertify.service';
import { NgForm } from '@angular/forms';
import { UserService } from '_services/User.service';
import { AuthService } from '_services/auth.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {
  user: User;
  @ViewChild('editForm') editForm: NgForm;
  @HostListener('window:beforeunload', ['$event'])
  unloadNotification($event: any) {
    if (this.editForm.dirty) {
      $event.returnValue = true;
    }
  }

  // tslint:disable-next-line:max-line-length
  constructor(private route: ActivatedRoute, private alertify: AlertifyService, private userService: UserService, private authService: AuthService) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.user = data.user;
    });
  }

  updateUser() {
  //  console.log(this.user);
    this.userService.updateUser(this.authService.decodedToken.nameid, this.user
      ).subscribe(next => {
        this.alertify.success('保存成功');
        this.editForm.reset(this.user);
      }, error => {
        this.alertify.error('保存失败' + error);
      }
      );
    // this.alertify.success('修改成功');
    // this.editForm.reset(this.user); // 表单状态重置为新的
  }
}
