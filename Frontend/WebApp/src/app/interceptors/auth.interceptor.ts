import { HttpInterceptorFn } from "@angular/common/http";
import { inject } from "@angular/core";
import { Router } from "@angular/router";
import { catchError, throwError } from "rxjs";

export const authInterceptor: HttpInterceptorFn = (req, next) => {
    const router = inject(Router);
    const token = localStorage.getItem('token');

    let cloned = req;
    if (token) {
        cloned = req.clone({
            setHeaders: {
                Authorization: `Bearer ${token}`
            }
        });
    }

    return next(cloned).pipe(
        catchError((error) => {
            if (error.status === 401) {
                // nicht eingeloggt → Login-Seite
                router.navigate(['/login']);
            }
            if (error.status === 403) {
                // keine Berechtigung → z. B. Startseite
                router.navigate(['/categories']);
            }
            return throwError(() => error);
        })
    );
};