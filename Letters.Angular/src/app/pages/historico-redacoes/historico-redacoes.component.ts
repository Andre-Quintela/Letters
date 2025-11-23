import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { EssayService, Essay } from '../../services/essay.service';
import { AuthService } from '../../services/auth.service';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-historico-redacoes',
  templateUrl: './historico-redacoes.component.html',
  styleUrls: ['./historico-redacoes.component.css'],
  providers: [MessageService]
})
export class HistoricoRedacoesComponent implements OnInit {
  essays: Essay[] = [];
  filteredEssays: Essay[] = [];
  isLoading = false;
  searchTerm = '';
  filterOption = 'todas'; // 'todas', 'corrigidas', 'nao-corrigidas'
  selectedEssay: Essay | null = null;
  showModal = false;

  constructor(
    private essayService: EssayService,
    private authService: AuthService,
    private messageService: MessageService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadEssays();
  }

  loadEssays(): void {
    this.isLoading = true;
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
    
    const userId = currentUser.id;

    this.essayService.getUserEssays(userId).subscribe({
      next: (essays) => {
        this.essays = essays.sort((a, b) => 
          new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()
        );
        this.applyFilters();
        this.isLoading = false;
      },
      error: (error) => {
        this.messageService.add({
          severity: 'error',
          summary: 'Erro',
          detail: 'Erro ao carregar redações'
        });
        console.error('Erro ao carregar redações:', error);
        this.isLoading = false;
      }
    });
  }

  applyFilters(): void {
    let filtered = [...this.essays];

    // Filtro por status de correção
    if (this.filterOption === 'corrigidas') {
      filtered = filtered.filter(e => e.correctedAt != null);
    } else if (this.filterOption === 'nao-corrigidas') {
      filtered = filtered.filter(e => e.correctedAt == null);
    }

    // Filtro por busca
    if (this.searchTerm.trim()) {
      const search = this.searchTerm.toLowerCase();
      filtered = filtered.filter(e => 
        e.theme.toLowerCase().includes(search) ||
        e.content.toLowerCase().includes(search)
      );
    }

    this.filteredEssays = filtered;
  }

  onSearchChange(): void {
    this.applyFilters();
  }

  onFilterChange(): void {
    this.applyFilters();
  }

  viewEssay(essay: Essay): void {
    this.selectedEssay = essay;
    this.showModal = true;
  }

  closeModal(): void {
    this.showModal = false;
    this.selectedEssay = null;
  }

  correctEssay(essay: Essay): void {
    if (!essay.id) return;

    this.messageService.add({
      severity: 'info',
      summary: 'Aguarde',
      detail: 'Enviando redação para correção...'
    });

    this.essayService.correctEssay(essay.id).subscribe({
      next: (correctedEssay) => {
        this.messageService.add({
          severity: 'success',
          summary: 'Sucesso',
          detail: 'Redação corrigida com sucesso!'
        });
        
        // Atualizar a redação na lista
        const index = this.essays.findIndex(e => e.id === essay.id);
        if (index !== -1) {
          this.essays[index] = correctedEssay;
        }
        
        this.applyFilters();
        
        // Se o modal estiver aberto, atualizar a redação selecionada
        if (this.selectedEssay?.id === essay.id) {
          this.selectedEssay = correctedEssay;
        }
      },
      error: (error) => {
        this.messageService.add({
          severity: 'error',
          summary: 'Erro',
          detail: error.error?.message || 'Erro ao corrigir redação'
        });
        console.error('Erro ao corrigir redação:', error);
      }
    });
  }

  deleteEssay(essay: Essay): void {
    if (!essay.id || !confirm('Tem certeza que deseja excluir esta redação?')) {
      return;
    }

    this.essayService.deleteEssay(essay.id).subscribe({
      next: () => {
        this.messageService.add({
          severity: 'success',
          summary: 'Sucesso',
          detail: 'Redação excluída com sucesso!'
        });
        
        this.essays = this.essays.filter(e => e.id !== essay.id);
        this.applyFilters();
        
        if (this.selectedEssay?.id === essay.id) {
          this.closeModal();
        }
      },
      error: (error) => {
        this.messageService.add({
          severity: 'error',
          summary: 'Erro',
          detail: 'Erro ao excluir redação'
        });
        console.error('Erro ao excluir redação:', error);
      }
    });
  }

  getScoreClass(score: number | null | undefined): string {
    if (!score) return '';
    if (score >= 800) return 'score-excellent';
    if (score >= 600) return 'score-good';
    if (score >= 400) return 'score-average';
    return 'score-low';
  }

  formatDate(date: string | Date): string {
    return new Date(date).toLocaleDateString('pt-BR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  goToNewEssay(): void {
    this.router.navigate(['/redacao']);
  }
}
