import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Input() valuesFromHome: any;
  @Output() cancelRegister = new EventEmitter();
model: any = {}; // 空对象
  constructor() { }

  ngOnInit() {
  }

  register() {
    console.log(this.model);
  }

  cancel() {
    // console.log('取消');
    this.cancelRegister.emit(false);
  }


}
