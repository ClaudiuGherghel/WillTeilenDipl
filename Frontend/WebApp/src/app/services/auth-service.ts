import { Injectable, signal, inject } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { User } from '../models/user.model';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { DatePipe } from '@angular/common';
import { RegisterRequest } from '../models/register-request';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);
  private datePipe = inject(DatePipe);

  private _isAuthenticated = signal(false);
  private _userId = signal<number | null>(null);
  private _role = signal<string | null>(null);

  // ---- Signale ----
  isAuthenticated = this._isAuthenticated.asReadonly();
  userId = this._userId.asReadonly();
  role = this._role.asReadonly();

  private apiUrl = 'https://localhost:7267/api/users'; // ✅ Anpassen!

  constructor() {
    this.restoreSession();
  }

  // ---- Register ---- https://localhost:7267/api/users/register
  // register(user: User): Observable<{ token: string }> {
  //   return this.http.post<{ token: string }>(`${this.apiUrl}/register`, user).pipe(
  //     tap(res => {
  //       this.saveToken(res.token);
  //     })
  //   );
  // }

  register(registerRequest: RegisterRequest): Observable<{ token: string }> {
    console.log(registerRequest);

    return this.http.post<{ token: string }>(`${this.apiUrl}/register`, registerRequest).pipe(
      tap(res => this.saveToken(res.token))
    );
  }




  // ---- Login ----
  login(username: string, password: string): Observable<{ token: string }> {
    return this.http.post<{ token: string }>(`${this.apiUrl}/login`, { username, password }).pipe(
      tap(res => {
        this.saveToken(res.token);
      })
    );
  }

  // ---- Logout ----
  logout() {
    localStorage.removeItem('token');
    this._isAuthenticated.set(false);
    this._userId.set(null);
    this._role.set(null);
    this.router.navigate(['/login']);
  }

  // ---- Token speichern & Signale setzen ----
  private saveToken(token: string) {
    localStorage.setItem('token', token);
    this.decodeAndStore(token);
  }

  // ---- Session beim App-Start prüfen ----
  private restoreSession() {
    const token = localStorage.getItem('token');
    if (token) {
      this.decodeAndStore(token);
    }
  }

  private decodeAndStore(token: string) {
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const exp = payload.exp * 1000; // Sekunden → Millisekunden
      if (Date.now() > exp) {
        this.logout();
        return;
      }
      this._isAuthenticated.set(true);
      this._userId.set(Number(payload.sub));
      this._role.set(payload.role ?? payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ?? null);
    } catch (err) {
      console.error('Token decoding failed', err);
      this.logout();
    }
  }
}
