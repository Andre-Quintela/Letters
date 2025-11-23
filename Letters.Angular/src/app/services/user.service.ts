import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface UserProfile {
  id: string;
  name: string;
  email: string;
  document: string;
  bornDate: string;
  schoolId: string;
  grade: number;
  isTeacher: boolean;
}

export interface UpdateProfile {
  name: string;
  email: string;
  document: string;
  bornDate: string;
  schoolId: string;
  grade: number;
}

export interface ChangePassword {
  currentPassword: string;
  newPassword: string;
}

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private apiUrl = 'https://localhost:7168/api/User';

  constructor(private http: HttpClient) {}

  getUserProfile(userId: string): Observable<UserProfile> {
    return this.http.get<UserProfile>(`${this.apiUrl}/${userId}`);
  }

  updateProfile(userId: string, profile: UpdateProfile): Observable<any> {
    return this.http.put(`${this.apiUrl}/${userId}`, profile);
  }

  changePassword(userId: string, passwords: ChangePassword): Observable<any> {
    return this.http.post(`${this.apiUrl}/${userId}/change-password`, passwords);
  }
}
