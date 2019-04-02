import { HttpClientModule } from '@angular/common/http';
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { ValueComponent } from './value/value.component';


@NgModule({
   declarations: [
      AppComponent,
      ValueComponent // 使用自动生成组件的方式，会自动把组件加载进来
   ],
   imports: [
      BrowserModule,
      AppRoutingModule,
      HttpClientModule// 引入这个模组之后，我们就可以使用该模组提供的service，比如httpclient，可以用来发送get请求
      ],
   providers: [],
   bootstrap: [
      AppComponent
   ]
})
export class AppModule { }
