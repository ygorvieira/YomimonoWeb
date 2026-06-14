export interface AuthResponse {
  token: string;
  email: string;
  userName: string;
}

export interface LoginDto {
  email: string;
  password: string;
}

export interface RegisterDto {
  email: string;
  password: string;
  userName: string;
}
