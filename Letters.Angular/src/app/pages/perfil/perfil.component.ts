import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { UserService, UserProfile } from '../../services/user.service';
import { AuthService } from '../../services/auth.service';
import { MessageService } from 'primeng/api';
import { Router } from '@angular/router';

@Component({
  selector: 'app-perfil',
  templateUrl: './perfil.component.html',
  styleUrls: ['./perfil.component.css'],
  providers: [MessageService]
})
export class PerfilComponent implements OnInit {
  profileForm!: FormGroup;
  passwordForm!: FormGroup;
  userProfile: UserProfile | null = null;
  isLoadingProfile = false;
  isUpdatingProfile = false;
  isChangingPassword = false;
  activeTab = 'perfil';

  constructor(
    private fb: FormBuilder,
    private userService: UserService,
    private authService: AuthService,
    private messageService: MessageService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.initForms();
    this.loadUserProfile();
  }

  initForms(): void {
    this.profileForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      document: ['', [Validators.required]],
      bornDate: ['', [Validators.required]],
      schoolId: ['', [Validators.required]],
      grade: ['', [Validators.required, Validators.min(1), Validators.max(12)]]
    });

    this.passwordForm = this.fb.group({
      currentPassword: ['', [Validators.required, Validators.minLength(6)]],
      newPassword: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required]]
    }, { validators: this.passwordMatchValidator });
  }

  passwordMatchValidator(form: FormGroup) {
    const newPassword = form.get('newPassword');
    const confirmPassword = form.get('confirmPassword');
    
    if (newPassword && confirmPassword && newPassword.value !== confirmPassword.value) {
      confirmPassword.setErrors({ passwordMismatch: true });
      return { passwordMismatch: true };
    }
    return null;
  }

  loadUserProfile(): void {
    this.isLoadingProfile = true;
    const currentUser = this.authService.getCurrentUser();
    
    if (!currentUser || !currentUser.id) {
      this.messageService.add({
        severity: 'error',
        summary: 'Erro',
        detail: 'Usuário não autenticado'
      });
      this.router.navigate(['/login']);
      return;
    }

    this.userService.getUserProfile(currentUser.id).subscribe({
      next: (profile) => {
        this.userProfile = profile;
        this.populateForm(profile);
        this.isLoadingProfile = false;
      },
      error: (error) => {
        this.messageService.add({
          severity: 'error',
          summary: 'Erro',
          detail: 'Erro ao carregar perfil'
        });
        console.error('Erro ao carregar perfil:', error);
        this.isLoadingProfile = false;
      }
    });
  }

  populateForm(profile: UserProfile): void {
    const bornDate = new Date(profile.bornDate);
    
    this.profileForm.patchValue({
      name: profile.name,
      email: profile.email,
      document: profile.document,
      bornDate: bornDate,
      schoolId: profile.schoolId,
      grade: profile.grade
    });
  }

  updateProfile(): void {
    if (this.profileForm.invalid) {
      this.markFormGroupTouched(this.profileForm);
      this.messageService.add({
        severity: 'warn',
        summary: 'Atenção',
        detail: 'Preencha todos os campos corretamente'
      });
      return;
    }

    this.isUpdatingProfile = true;
    const currentUser = this.authService.getCurrentUser();
    
    if (!currentUser || !currentUser.id) {
      return;
    }

    const formValue = this.profileForm.value;
    const updateData = {
      ...formValue,
      bornDate: new Date(formValue.bornDate).toISOString()
    };

    this.userService.updateProfile(currentUser.id, updateData).subscribe({
      next: () => {
        this.messageService.add({
          severity: 'success',
          summary: 'Sucesso',
          detail: 'Perfil atualizado com sucesso!'
        });
        this.loadUserProfile();
        this.isUpdatingProfile = false;
      },
      error: (error) => {
        this.messageService.add({
          severity: 'error',
          summary: 'Erro',
          detail: error.error?.message || 'Erro ao atualizar perfil'
        });
        console.error('Erro ao atualizar perfil:', error);
        this.isUpdatingProfile = false;
      }
    });
  }

  changePassword(): void {
    if (this.passwordForm.invalid) {
      this.markFormGroupTouched(this.passwordForm);
      this.messageService.add({
        severity: 'warn',
        summary: 'Atenção',
        detail: 'Preencha todos os campos corretamente'
      });
      return;
    }

    this.isChangingPassword = true;
    const currentUser = this.authService.getCurrentUser();
    
    if (!currentUser || !currentUser.id) {
      return;
    }

    const { currentPassword, newPassword } = this.passwordForm.value;

    this.userService.changePassword(currentUser.id, { currentPassword, newPassword }).subscribe({
      next: () => {
        this.messageService.add({
          severity: 'success',
          summary: 'Sucesso',
          detail: 'Senha alterada com sucesso!'
        });
        this.passwordForm.reset();
        this.isChangingPassword = false;
      },
      error: (error) => {
        this.messageService.add({
          severity: 'error',
          summary: 'Erro',
          detail: error.error?.message || 'Erro ao alterar senha'
        });
        console.error('Erro ao alterar senha:', error);
        this.isChangingPassword = false;
      }
    });
  }

  markFormGroupTouched(formGroup: FormGroup): void {
    Object.keys(formGroup.controls).forEach(key => {
      const control = formGroup.get(key);
      control?.markAsTouched();
    });
  }

  isFieldInvalid(form: FormGroup, fieldName: string): boolean {
    const field = form.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  getErrorMessage(form: FormGroup, fieldName: string): string {
    const field = form.get(fieldName);
    
    if (field?.hasError('required')) {
      return 'Campo obrigatório';
    }
    if (field?.hasError('email')) {
      return 'E-mail inválido';
    }
    if (field?.hasError('minlength')) {
      const minLength = field.errors?.['minlength'].requiredLength;
      return `Mínimo de ${minLength} caracteres`;
    }
    if (field?.hasError('min')) {
      return 'Série inválida';
    }
    if (field?.hasError('max')) {
      return 'Série inválida';
    }
    if (field?.hasError('passwordMismatch')) {
      return 'As senhas não coincidem';
    }
    
    return '';
  }

  setActiveTab(tab: string): void {
    this.activeTab = tab;
  }
}
