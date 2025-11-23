import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface EssayRequest {
  theme: string;
  content: string;
  userId: string;
}

export interface EssayResponse {
  id: string;
  theme: string;
  content: string;
  createdAt: Date;
  correctedAt?: Date;
  score?: number;
  correctionFeedback?: string;
  strengths?: string[];
  improvements?: string[];
  detailedAnalysis?: string;
}

@Injectable({
  providedIn: 'root'
})
export class EssayService {
  private apiUrl = 'https://localhost:7168/api/Essay';

  constructor(private http: HttpClient) {}

  createEssay(request: EssayRequest): Observable<EssayResponse> {
    return this.http.post<EssayResponse>(this.apiUrl, request);
  }

  correctEssay(essayId: string): Observable<EssayResponse> {
    return this.http.post<EssayResponse>(`${this.apiUrl}/${essayId}/correct`, {});
  }

  getEssayById(essayId: string): Observable<EssayResponse> {
    return this.http.get<EssayResponse>(`${this.apiUrl}/${essayId}`);
  }

  getUserEssays(userId: string): Observable<EssayResponse[]> {
    return this.http.get<EssayResponse[]>(`${this.apiUrl}/user/${userId}`);
  }
}
