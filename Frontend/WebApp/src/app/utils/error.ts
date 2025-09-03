import { HttpErrorResponse } from '@angular/common/http';

export function extractErrorMessage(error: unknown): string {
    let message = 'Ein unbekannter Fehler ist aufgetreten';

    if (error instanceof HttpErrorResponse) {
        if (error.error) {
            if (typeof error.error === 'string') {
                // Backend gibt reinen Text zurück
                message = error.error;
            } else if (error.error.detail) {
                // ProblemDetails (RFC 7807)
                message = error.error.detail;
            } else if (error.error.message) {
                // Klassische API mit { message: "..." }
                message = error.error.message;
            } else if (Array.isArray(error.error.errors)) {
                // Falls mehrere Validierungsfehler kommen
                message = error.error.errors.join('\n');
            }
        } else {
            message = error.message; // z. B. "Bad Request"
        }
    } else if (error instanceof Error) {
        message = error.message;
    }

    return message;
}
