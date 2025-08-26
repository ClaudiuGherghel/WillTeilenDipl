export interface RegisterRequest {
    userName: string;
    password: string;
    email: string;
    firstName: string;
    lastName: string;
    birthDate: string;  // ISO-String
    role: string;       // "User"
    address: string;
    phoneNumber: string;
    geoPostalId: number;
}