import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
registerMode = false;
values: any;
  constructor(private http: HttpClient) { }

  ngOnInit() {
    this.getValues();
  }
  registerToggle() {
    this.registerMode = !this.registerMode;
  }

  getValues() {
    this.http.get('http://localhost:5000/api/values').subscribe(response => { // response表示一个callback，回调函数
                  this.values = response;
                 },
    error => { // 发生错误执行的方法
      console.log(error);
    }
    );
  }

  canCancleRegisterMode(registerMode: boolean) {
  // tslint:disable-next-line:max-line-length
  this.registerMode = registerMode; // this.registerMode表示上面声明的registerMode，等号后面的registerMode是参数 this.registerMode的初始值是false，也就是说注册页面是不显示的，当用户点击注册后触发registerToggle()事件，this.registerMode变为true，然后注册页面显示出来，邀请注册的页面消失。
  }

}
