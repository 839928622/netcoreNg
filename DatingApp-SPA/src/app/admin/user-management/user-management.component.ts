import { map } from 'rxjs/operators';
import { AlertifyService } from '_services/alertify.service';
import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/_models/User';
import { AdminService } from '_services/admin.service';
import { BsModalService, BsModalRef } from 'ngx-bootstrap';
import { RolesModalComponent } from '../roles-modal/roles-modal.component';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css']
})
export class UserManagementComponent implements OnInit {
  users: User[];
  bsModalRef: BsModalRef;

  constructor(private adminService: AdminService,
              private alertifyService: AlertifyService,
              private modalService: BsModalService) { }

  ngOnInit() {
    this.getUserWithRoles();
  }

  getUserWithRoles() {
    this.adminService.getUserWithRoles().subscribe(
      (users: User[]) => {
        this.users = users;
      }, error => {
        this.alertifyService.error(error);
      }
    );
  }

  editRolesModal(user: User) {
    const initialState = {
      user,
      roles: this.getRolesArray(user)
      // list: [
      //   user,
      //   roles: this.getRolesArray(user),
      //   'Do something else',
      //   '...'
      // ],
      // title: 'Modal with component'
    };
    this.bsModalRef = this.modalService.show(RolesModalComponent, {initialState});
    this.bsModalRef.content.updateSelectedRoles.subscribe((values) => {
      const rolesToUpdate = {
      roleNames: [...values.filter(el => el.checked === true).map(el => el.value)]
      }; // api RoleEditDto中期待的是 RoleNames
      if (rolesToUpdate) {
        this.adminService.updateUserRoles(user, rolesToUpdate).subscribe(() => {
          user.roles = [...rolesToUpdate.roleNames]; // ...创建一个新的数组，里面重新包含修改后的新的角色
        }, error => {
          this.alertifyService.error('编辑角色失败');
        });
      }
    });
  }

  private getRolesArray(user) {
    const roles = []; // 空数组
    const userRoles = user.roles;
    const availableRoles: any[] = [
      {name: '管理员(Admin)', value: 'Admin'},
      {name: '审核员(Moderator)', value: 'Moderator'},
      {name: '一般会员(Member)', value: 'Member'},
      {name: '贵宾(VIP)' , value: 'VIP'}
    ];

    // tslint:disable-next-line:prefer-for-of
    for (let i = 0; i < availableRoles.length; i++) {
      let isMatch = false ;
      // tslint:disable-next-line:prefer-for-of
      for (let j = 0; j < userRoles.length; j++) {
        if (availableRoles[i].value === userRoles[j]) {
          isMatch = true ;
          availableRoles[i].checked = true ;
          roles.push(availableRoles[i]);
          break;
        }
      }
      if (!isMatch) {
        availableRoles[i].checked = false;
        roles.push(availableRoles[i]);
      }
    }
    return roles;
  }
}
