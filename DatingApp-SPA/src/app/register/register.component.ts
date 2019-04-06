import { AlertifyService } from './../../../_services/alertify.service';
import { AuthService } from './../../../_services/auth.service';
import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  // @Input() valuesFromHome: any; 为了感受ng的组件通信功能
  @Output() cancelRegister = new EventEmitter();
model: any = {}; // 空对象
  constructor(private authService: AuthService, private alertify: AlertifyService) { }

  ngOnInit() {
  }

  register() {
    // console.log(this.model);
    this.authService.register(this.model).subscribe(() => {
                   this.alertify.success('注册成功');
                   // console.log('注册成功');
                  }, error => {
                   this.alertify.error(error);
                  //  console.log(error + '注册失败');
                  }
                  );
  }

  cancel() {
    // console.log('取消');
    this.cancelRegister.emit(false);
  }


}
