import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Photo } from 'src/app/_models/Photo';
import { FileUploader } from 'ng2-file-upload';
import { environment } from 'src/environments/environment';
import { AuthService } from '_services/auth.service';
import { UserService } from '_services/User.service';
import { AlertifyService } from '_services/alertify.service';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {
  @Input() photos: Photo[];
  @Output() getMemberPhotoChange = new EventEmitter<string>();
    uploader: FileUploader ; // = new FileUploader({url: URL}); FileUploader将被另外初始化
   hasBaseDropZoneOver = false;
   hasAnotherDropZoneOver = false;
   baseUrl = environment.apiUrl;
   currentMain: Photo; // 当前的主照片


  // public fileOverAnother(e:any):void {
  //   this.hasAnotherDropZoneOver = e;
  // }

  constructor(private authService: AuthService, private userService: UserService, private alertify: AlertifyService ) { }

  ngOnInit() { // 用户-用户拥有的照片
  this.initializeUploader();
 }

  fileOverBase(e: any): void {
    this.hasBaseDropZoneOver = e;
  }

  initializeUploader() {
   this.uploader = new FileUploader({
     url: this.baseUrl + 'users/' + this.authService.decodedToken.nameid + '/photos',
     authToken: 'Bearer ' + localStorage.getItem('token'),
     isHTML5: true ,
     allowedFileType: ['image'] ,
     removeAfterUpload: true ,
     autoUpload: false ,
     maxFileSize: 10 * 1024 * 1024
   });

   this.uploader.onAfterAddingFile = (file => {
     file.withCredentials = false;
   });

   this.uploader.onSuccessItem = (item, response, status, headers) => {
     if (response) {
       const res: Photo = JSON.parse(response); // 把响应转为json
       const photo = {
         id: res.id,
         url: res.url,
         dateAdded: res.dateAdded,
         description: res.description,
         isMain: res.isMain
       };

       this.photos.push(photo);
     }
   };
  }

  setMainPhoto(photo: Photo) {
    this.userService.setMainPhoto(this.authService.decodedToken.nameid, photo.id).subscribe(() => {
      // console.log('主照片设置成功');
      this.currentMain = this.photos.filter(p => p.isMain === true)[0]; // 筛选过滤出主照片  filter返回的是photo类型的 数组，由于只有一张主图，因此选择第一张即可
      this.currentMain.isMain = false;
      photo.isMain = true; // 把欲设为主照片的照片设为true
    //  this.getMemberPhotoChange.emit(photo.url); // 子组件上传给父组件图片的url地址信息
      this.authService.changeMemberPhoto(photo.url);
      this.authService.currentUser.photoUrl = photo.url;
      localStorage.setItem('user', JSON.stringify(this.authService.currentUser));
      this.alertify.success('主照片设置成功');
    }, error => {
     this.alertify.error('主照片设置失败' + error);
    }
    );
  }

  deletePhoto(id: number) {
   this.alertify.confirm('确定要删除照片吗？', () => {
     this.userService.deletePhoto(this.authService.decodedToken.nameid, id).subscribe(() => {
      this.photos.splice(this.photos.findIndex(p => p.id === id), 1);
      this.alertify.success('照片删除成功');
     }, error => {
       this.alertify.error('照片删除失败' + error);
     }
     );
   });
  }

}
