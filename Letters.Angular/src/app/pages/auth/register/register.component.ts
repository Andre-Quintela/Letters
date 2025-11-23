import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../services/auth.service';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
  providers: [MessageService]
})
export class RegisterComponent implements OnInit {
  registerForm!: FormGroup;
  loading = false;
  maxDate = new Date();
  currentYear = new Date().getFullYear();
  userTypes = [
    { label: 'Aluno', value: false },
    { label: 'Professor', value: true }
  ];
  grades = [
    { label: '1ª Série', value: 1 },
    { label: '2ª Série', value: 2 },
    { label: '3ª Série', value: 3 },
    { label: '4ª Série', value: 4 },
    { label: '5ª Série', value: 5 },
    { label: '6ª Série', value: 6 },
    { label: '7ª Série', value: 7 },
    { label: '8ª Série', value: 8 },
    { label: '9ª Série', value: 9 },
    { label: '1º Ano', value: 10 },
    { label: '2º Ano', value: 11 },
    { label: '3º Ano', value: 12 }
  ];

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private messageService: MessageService
  ) {}

  ngOnInit(): void {
    this.registerForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required]],
      document: ['', [Validators.required]],
      bornDate: ['', [Validators.required]],
      schoolId: ['00000000-0000-0000-0000-000000000000'],
      grade: [1, [Validators.required, Validators.min(1), Validators.max(12)]],
      isTeacher: [false, [Validators.required]]
    }, { validators: this.passwordMatchValidator });
  }

  passwordMatchValidator(form: FormGroup): { [key: string]: boolean } | null {
    const password = form.get('password');
    const confirmPassword = form.get('confirmPassword');
    
    if (password && confirmPassword && password.value !== confirmPassword.value) {
      confirmPassword.setErrors({ passwordMismatch: true });
      return { passwordMismatch: true };
    }
    return null;
  }

  onSubmit(): void {
    if (this.registerForm.valid) {
      this.loading = true;
      
      const formData = { ...this.registerForm.value };
      
      // Converter data para formato ISO
      if (formData.bornDate) {
        const date = new Date(formData.bornDate);
        formData.bornDate = date.toISOString().split('T')[0];
      }

      delete formData.confirmPassword;

      this.authService.register(formData).subscribe({
        next: (response) => {
          this.loading = false;
          if (response.success) {
            this.messageService.add({
              severity: 'success',
              summary: 'Sucesso',
              detail: response.message
            });
            setTimeout(() => {
              this.router.navigate(['/home']);
            }, 1500);
          } else {
            this.messageService.add({
              severity: 'error',
              summary: 'Erro',
              detail: response.message
            });
          }
        },
        error: (error) => {
          this.loading = false;
          this.messageService.add({
            severity: 'error',
            summary: 'Erro',
            detail: error.error?.message || 'Erro ao fazer cadastro'
          });
        }
      });
    } else {
      Object.keys(this.registerForm.controls).forEach(key => {
        const control = this.registerForm.get(key);
        if (control && control.invalid) {
          control.markAsDirty();
          control.updateValueAndValidity();
        }
      });
    }
  }
}
