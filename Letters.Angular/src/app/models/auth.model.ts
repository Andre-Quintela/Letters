export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  name: string;
  email: string;
  password: string;
  document: string;
  bornDate: string;
  schoolId: string;
  grade: number;
  isTeacher: boolean;
}

export interface AuthResponse {
  success: boolean;
  message: string;
  token: string;
  user?: UserData;
}

export interface UserData {
  id: string;
  name: string;
  email: string;
  document: string;
  bornDate: string;
  schoolId: string;
  grade: number;
  isTeacher: boolean;
}
