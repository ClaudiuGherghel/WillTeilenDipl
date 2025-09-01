import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { User, UserChangePwDto, UserPutDo } from '../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private apiUrl = "https://localhost:7267/api/users";
  private http = inject(HttpClient);


  get(id: number) {
    return this.http.get<User>(this.apiUrl + "/get/" + id);
  }

  changePw(id: number, changePw: UserChangePwDto) {
    return this.http.put<UserChangePwDto>(this.apiUrl + "/changepasswordbyuser/" + id, changePw);
  }

  put(id: number, user: UserPutDo) {
    return this.http.put<User>(this.apiUrl + "/updatebyuser/" + id, user);
  }
}
