import { AlertifyService } from './../../../_services/alertify.service';
import { AuthService } from './../../../_services/auth.service';
import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap';
import { BsLocaleService } from 'ngx-bootstrap/datepicker';
import { from } from 'rxjs';
import { listLocales } from 'ngx-bootstrap/chronos';
import { User } from '../_models/User';
import { Router } from '@angular/router';


@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  // @Input() valuesFromHome: any; 为了感受ng的组件通信功能
  @Output() cancelRegister = new EventEmitter();
// model: any = {}; // 空对象
model: User ;
registerForm: FormGroup ;
bsConfig: Partial<BsDatepickerConfig> ; // 使用Partial之后，BsDatepickerConfig里的必选项可以变成可选项


  constructor(private authService: AuthService,
              private alertify: AlertifyService,
              private formBuilder: FormBuilder,
              private localeService: BsLocaleService,
              private router: Router
              ) { }

  ngOnInit() {
    // this.registerForm = new FormGroup({
    //   username: new FormControl('', [Validators.required, Validators.email]),
    //   password: new FormControl('', [Validators.required, Validators.minLength(9), Validators.maxLength(25)]),
    //   confirmPassword: new FormControl('', Validators.required)

    // }, this.passwordMatchValidator
    // );
    this.bsConfig = {
    containerClass: 'theme-red',
    dateInputFormat: 'YYYY-MM-DD', // 设置日期显示格式
    minDate: new Date(1900, 1, 1), // 设置最小日期显示
    isDisabled: false,
    showWeekNumbers: false,
  };

    this.createRegisterForm();
  }

  createRegisterForm() {
    this.registerForm = this.formBuilder.group(
      {
        gender: ['男'], // 性别
        username: ['', [Validators.required, Validators.email]], // 用户名
        knownAs: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(8)]], // 昵称
        dateOfBirth: ['', [Validators.required]], // 出生日期
        city: ['', [Validators.required]], // 城市
        country: ['', [Validators.required]], // 国家
        password: ['', [Validators.required, Validators.minLength(9), Validators.maxLength(25)]], // 密码
        confirmPassword: ['', [Validators.required]] // 确认密码
      }, {validators: this.passwordMatchValidator} // 自定义数据验证方法：密码和确认密码是否一致
    );
  }

  passwordMatchValidator(formGroup: FormGroup) {
    return formGroup.get('password').value === formGroup.get('confirmPassword').value ? null : { mismatch: true} ;
  }
  register() {
    // // console.log(this.model);
    // this.authService.register(this.model).subscribe(() => {
    //                this.alertify.success('注册成功');
    //                // console.log('注册成功');
    //               }, error => {
    //                this.alertify.error(error);
    //               //  console.log(error + '注册失败');
    //               }
    //               );
     // console.log(this.registerForm.value);
    if (this.registerForm.valid) {
     this.model = Object.assign({}, this.registerForm.value); // 把registerForm里的值（formGroup）转换成User对象
     this.authService.register(this.model).subscribe(() => {
     this.alertify.success('注册成功');
     }, error => {
       this.alertify.error(error);
     }, () => {
    this.authService.login(this.model).subscribe(() => {
     this.router.navigate(['/members']); // 登录成功后自动转到members组件
    });
     }
     );
    }
  }

  cancel() {
    // console.log('取消');
    this.cancelRegister.emit(false);
  }


}
