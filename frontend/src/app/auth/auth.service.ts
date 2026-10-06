import { Injectable, computed, signal } from '@angular/core';
import Keycloak from 'keycloak-js';

// Kde běží Keycloak a jaký realm/klient používáme (viz keycloak/realm-ekospol.json).
// Adresu vidí prohlížeč, proto je to localhost:8180 – stejně v Dockeru i při ng serve.
const keycloak = new Keycloak({
  url: 'http://localhost:8180',
  realm: 'ekospol',
  clientId: 'frontend',
});

@Injectable({ providedIn: 'root' })
export class AuthService {
  readonly authenticated = signal(false);
  readonly username = signal<string | null>(null);
  readonly roles = signal<string[]>([]);
  readonly isAdmin = computed(() => this.roles().includes('admin'));

  // Volá se jednou při startu aplikace (app.config.ts).
  // check-sso = zjisti, jestli je uživatel v Keycloaku přihlášený, ale nenuť ho k přihlášení.
  async init(): Promise<void> {
    try {
      const authenticated = await keycloak.init({
        onLoad: 'check-sso',
        silentCheckSsoRedirectUri: `${window.location.origin}/silent-check-sso.html`,
        pkceMethod: 'S256',
        checkLoginIframe: false,
      });
      this.update(authenticated);
      keycloak.onAuthLogout = () => this.update(false);
    } catch (err) {
      // Když Keycloak neběží, aplikace se má i tak načíst (jen bez přihlášení).
      console.error('Keycloak se nepodařilo inicializovat', err);
    }
  }

  login(): Promise<void> {
    return keycloak.login();
  }

  logout(): Promise<void> {
    return keycloak.logout({ redirectUri: window.location.origin });
  }

  // Vrátí platný access token; když brzy vyprší (do 30 s), nejdřív si vyžádá nový.
  async getToken(): Promise<string | undefined> {
    if (!keycloak.authenticated) {
      return undefined;
    }
    try {
      await keycloak.updateToken(30);
      return keycloak.token;
    } catch {
      this.update(false);
      return undefined;
    }
  }

  private update(authenticated: boolean): void {
    this.authenticated.set(authenticated);
    const token = authenticated ? keycloak.tokenParsed : undefined;
    this.username.set((token?.['preferred_username'] as string | undefined) ?? null);
    this.roles.set((token?.['roles'] as string[] | undefined) ?? []);
  }
}
