import { User } from 'src/app/_models/User';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  baseUrl = environment.apiUrl;

constructor(private http: HttpClient) { }

getUserWithRoles() {
  return this.http.get(this.baseUrl + 'admin/usersWithRoles');
}

updateUserRoles(user: User, roles: {}) {
  return this.http.post(this.baseUrl + 'admin/editRoles/' + user.userName, roles);
}
}
