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
  constructor(private authService: AuthService) { }

  ngOnInit() {
  }

  register() {
    // console.log(this.model);
    this.authService.register(this.model).subscribe(() => {
                    console.log('注册成功');
                  }, error => {
                    console.log(error + '注册失败');
                  }
                  );
  }

  cancel() {
    // console.log('取消');
    this.cancelRegister.emit(false);
  }


}
