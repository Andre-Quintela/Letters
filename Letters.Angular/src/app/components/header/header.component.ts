import { Component, OnInit } from '@angular/core';
import { MenuItem } from 'primeng/api';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrl: './header.component.css'
})
export class HeaderComponent implements OnInit {
  menuItems: MenuItem[] = [];
  userMenuItems: MenuItem[] = [];
  userName: string = '';

  constructor(
    private router: Router,
    private authService: AuthService
  ) {}

  ngOnInit() {
    this.authService.currentUser$.subscribe(user => {
      if (user) {
        this.userName = user.name || 'Usuário';
      }
    });

    this.menuItems = [
      {
        label: 'Home',
        icon: 'pi pi-home',
        command: () => this.router.navigate(['/home'])
      },
      {
        label: 'Escrever Redação',
        icon: 'pi pi-pencil',
        command: () => this.router.navigate(['/redacao'])
      },
      {
        label: 'Histórico',
        icon: 'pi pi-history',
        command: () => this.router.navigate(['/historico'])
      }
    ];

    this.userMenuItems = [
      {
        label: 'Meu Perfil',
        icon: 'pi pi-user',
        command: () => this.router.navigate(['/perfil'])
      },
      {
        separator: true
      },
      {
        label: 'Sair',
        icon: 'pi pi-sign-out',
        command: () => this.logout()
      }
    ];
  }

  logout() {
    this.authService.logout();
  }
}
