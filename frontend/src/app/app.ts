import { Component, inject, signal } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { JsonPipe } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { AuthService } from './auth/auth.service';

interface ApiResult {
  url: string;
  status: number;
  body: unknown;
}

@Component({
  imports: [RouterOutlet, JsonPipe],
  selector: 'app-root',
  styleUrl: './app.scss',
  templateUrl: './app.html',
})
export class App {
  protected readonly auth = inject(AuthService);
  private readonly http = inject(HttpClient);

  protected readonly result = signal<ApiResult | null>(null);

  // Zavolá endpoint API a zobrazí výsledek – i chybu 401/403, ať je vidět, jak funguje ochrana.
  protected call(url: string): void {
    this.http.get(url).subscribe({
      next: (body) => this.result.set({ url, status: 200, body }),
      error: (err: HttpErrorResponse) =>
        this.result.set({ url, status: err.status, body: this.errorText(err.status) }),
    });
  }

  private errorText(status: number): string {
    if (status === 401) return 'Nepřihlášen – endpoint vyžaduje přihlášení.';
    if (status === 403) return 'Přihlášen, ale chybí role admin.';
    return 'Chyba při volání API.';
  }
}
