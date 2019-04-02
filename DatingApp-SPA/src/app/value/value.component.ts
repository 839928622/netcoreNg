import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';


@Component({
  selector: 'app-value', // alt + o 进入该标签，alt + u返回
  templateUrl: './value.component.html',
  styleUrls: ['./value.component.css']
})
export class ValueComponent implements OnInit {
  values: any;

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.getValues(); // on initialization:即这个组件初始化的时候做什么事情
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

}
