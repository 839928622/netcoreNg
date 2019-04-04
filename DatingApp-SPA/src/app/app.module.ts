import { AuthService } from './../../_services/auth.service';
import { HttpClientModule } from '@angular/common/http';
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { ValueComponent } from './value/value.component';
import { NavComponent } from './Nav/Nav.component';
import { FormsModule } from '@angular/forms';
import { from } from 'rxjs';


@NgModule({
   declarations: [
      AppComponent,
      ValueComponent,
      // 使用自动生成组件的方式，会自动把组件加载进来,
      NavComponent
   ],
   imports: [
      BrowserModule,
      AppRoutingModule,
      HttpClientModule, // 引入这个模组之后，我们就可以使用该模组提供的service，比如httpclient，可以用来发送get请求
      FormsModule
   ],
   providers: [
      AuthService
   ],
   bootstrap: [
      AppComponent
   ]
})
export class AppModule { }
