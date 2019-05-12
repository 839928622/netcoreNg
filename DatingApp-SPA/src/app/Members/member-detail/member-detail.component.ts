import { User } from './../../_models/User';
import { AlertifyService } from './../../../../_services/alertify.service';
import { UserService } from './../../../../_services/User.service';
import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgxGalleryOptions, NgxGalleryImage, NgxGalleryAnimation } from 'ngx-gallery';
import { TabsetComponent } from 'ngx-bootstrap';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit {
  @ViewChild('memberTabs') memberTabs: TabsetComponent;
  user: User;
  galleryOptions: NgxGalleryOptions[] ;
  galleryImages: NgxGalleryImage[] ;

  constructor(private userService: UserService, private alertify: AlertifyService, private route: ActivatedRoute) { }

  ngOnInit() {
    // this.loadUser();
    this.route.data.subscribe(data => {
      this.user = data.user ; // 在组件被激活前先加载数据 个人理解是路由获取到了数据，最终组件再去路由取
    });

    this.route.queryParams.subscribe(params => {
    const selectedTab = params.tab; // 原文使用params['tab'] ,ng7不适用，params就是query上的参数，点出来即可
    this.memberTabs.tabs[selectedTab > 0 ? selectedTab : 0].active = true;
  });

    this.galleryOptions = [
      { // 下面的配置是设置gallery的样式
        width: '500px',
        height: '500px',
        imagePercent: 100,
        thumbnailsColumns: 4,
        imageAnimation: NgxGalleryAnimation.Slide,
        preview: false
      }
    ];

    this.galleryImages = this.getImages();

  }

  // loadUser() {
  // // tslint:disable-next-line:no-string-literal
  // this.userService.getUser( + this.route.snapshot.params['id']).subscribe((user: User) => {
  //   this.user = user;
  // }, error => {
  //   this.alertify.error('获取该用户详情失败' + error);
  // }
  // );
  // }

  getImages() {
    const imageUrls = [];
    // tslint:disable-next-line:prefer-for-of
    for (let i = 0; i < this.user.photos.length; i++) {
    imageUrls.push({
      small: this.user.photos[i].url,
      medium: this.user.photos[i].url,
      big: this.user.photos[i].url,
      description: this.user.photos[i].description
    });
    }

    return imageUrls;
  }

 selectTab(tabId: number) {
   this.memberTabs.tabs[tabId].active = true;
 }
}
