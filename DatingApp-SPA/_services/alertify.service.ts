import { Injectable } from '@angular/core';
// 在angular.json中引入的alertify.js，意味着alertify在全局可访问，但是我想要在这里使用，我们声明如下
declare let alertify: any;
@Injectable({
  providedIn: 'root'
})
export class AlertifyService {

constructor() { }

 confirm(message: string , okCallback: () => any) {
   alertify.confirm(message, (e) =>  {
     if (e) {
       okCallback();
     } else {}
   });
 }

  success(message: string) {
    alertify.success(message);
  }

  error(message: string) {
    alertify.error(message);
  }

  warning(message: string) {
    alertify.warning(message);
  }

  message(message: string) {
    alertify.message(message);
  }
}
