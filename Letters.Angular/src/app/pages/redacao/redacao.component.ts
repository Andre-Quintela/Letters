import { Component } from '@angular/core';
import { EssayService, EssayRequest, EssayResponse } from '../../services/essay.service';
import { AuthService } from '../../services/auth.service';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-redacao',
  templateUrl: './redacao.component.html',
  styleUrls: ['./redacao.component.css'],
  providers: [MessageService]
})
export class RedacaoComponent {
  redacaoContent: string = '';
  tema: string = '';
  caracterCount: number = 0;
  wordCount: number = 0;
  isLoading: boolean = false;
  correcaoResultado: any = null;
  currentEssayId: string | null = null;

  temasSugeridos = [
    'A importância da educação digital no século XXI',
    'Sustentabilidade e responsabilidade ambiental',
    'O impacto das redes sociais na sociedade contemporânea',
    'Desigualdade social no Brasil: causas e soluções',
    'Tecnologia e mercado de trabalho: desafios e oportunidades'
  ];

  constructor(
    private essayService: EssayService,
    private authService: AuthService,
    private messageService: MessageService
  ) {}

  onContentChange(event: any) {
    this.redacaoContent = event.target.value;
    this.caracterCount = this.redacaoContent.length;
    this.wordCount = this.redacaoContent.trim().split(/\s+/).filter(word => word.length > 0).length;
  }

  selecionarTema(tema: string) {
    this.tema = tema;
  }

  corrigirRedacao() {
    if (!this.redacaoContent.trim() || !this.tema) {
      this.messageService.add({
        severity: 'warn',
        summary: 'Atenção',
        detail: 'Por favor, selecione um tema e escreva sua redação.'
      });
      return;
    }

    const user = this.authService.getCurrentUser();
    if (!user) {
      this.messageService.add({
        severity: 'error',
        summary: 'Erro',
        detail: 'Usuário não autenticado.'
      });
      return;
    }

    this.isLoading = true;

    const request: EssayRequest = {
      theme: this.tema,
      content: this.redacaoContent,
      userId: user.id
    };

    // Primeiro, criar a redação
    this.essayService.createEssay(request).subscribe({
      next: (essay: EssayResponse) => {
        this.currentEssayId = essay.id;
        
        // Depois, corrigir a redação
        this.essayService.correctEssay(essay.id).subscribe({
          next: (correctedEssay: EssayResponse) => {
            this.correcaoResultado = {
              nota: correctedEssay.score || 0,
              comentarios: [correctedEssay.correctionFeedback || 'Sem comentários'],
              pontosFortesEncontrados: correctedEssay.strengths || [],
              melhorias: correctedEssay.improvements || [],
              analiseDetalhada: correctedEssay.detailedAnalysis || ''
            };
            
            this.isLoading = false;
            
            this.messageService.add({
              severity: 'success',
              summary: 'Sucesso',
              detail: 'Redação corrigida com sucesso!'
            });
          },
          error: (error) => {
            console.error('Erro ao corrigir redação:', error);
            this.isLoading = false;
            this.messageService.add({
              severity: 'error',
              summary: 'Erro',
              detail: 'Erro ao corrigir redação. Tente novamente.'
            });
          }
        });
      },
      error: (error) => {
        console.error('Erro ao criar redação:', error);
        this.isLoading = false;
        this.messageService.add({
          severity: 'error',
          summary: 'Erro',
          detail: 'Erro ao criar redação. Tente novamente.'
        });
      }
    });
  }

  limparRedacao() {
    this.redacaoContent = '';
    this.tema = '';
    this.caracterCount = 0;
    this.wordCount = 0;
    this.correcaoResultado = null;
    this.currentEssayId = null;
  }
}