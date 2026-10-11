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

  // Formulář "Můj profil" – odešle jméno a telefon na PUT /api/users/me (vytvoří nebo upraví záznam).
  protected saveProfile(name: string, telNumbers: string): void {
    const url = '/api/users/me';
    this.http.put(url, { name: name || null, telNumbers }).subscribe({
      next: (body) => this.result.set({ url: `PUT ${url}`, status: 200, body }),
      error: (err: HttpErrorResponse) =>
        this.result.set({
          url: `PUT ${url}`,
          status: err.status,
          // U 400 vrací API popis chyb validace – ten chceme vidět celý.
          body: err.status === 400 ? err.error : this.errorText(err.status),
        }),
    });
  }

  // Formulář "Přidat kontakt" (jen admin) – POST /api/users. Prázdná pole posílá jako null.
  protected createContact(name: string, telNumbers: string, email: string): void {
    const url = '/api/users';
    this.http.post(url, { name: name || null, telNumbers, email: email || null }).subscribe({
      next: (body) => this.result.set({ url: `POST ${url}`, status: 200, body }),
      error: (err: HttpErrorResponse) =>
        this.result.set({
          url: `POST ${url}`,
          status: err.status,
          body: err.status === 400 ? err.error : this.errorText(err.status),
        }),
    });
  }

  // Testovací data (jen admin): POST vygeneruje uživatele, DELETE je zase smaže.
  protected testData(method: 'POST' | 'DELETE'): void {
    const url = method === 'POST' ? '/api/test-data?count=50' : '/api/test-data';
    this.http.request(method, url).subscribe({
      next: (body) => this.result.set({ url: `${method} ${url}`, status: 200, body }),
      error: (err: HttpErrorResponse) =>
        this.result.set({ url: `${method} ${url}`, status: err.status, body: this.errorText(err.status) }),
    });
  }

  private errorText(status: number): string {
    if (status === 401) return 'Nepřihlášen – endpoint vyžaduje přihlášení.';
    if (status === 403) return 'Přihlášen, ale chybí role admin.';
    if (status === 404) return 'Nenalezeno.';
    return 'Chyba při volání API.';
  }
}
