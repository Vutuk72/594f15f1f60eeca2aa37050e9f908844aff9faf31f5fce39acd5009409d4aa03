import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { signal } from '@angular/core';
import { App } from './app';
import { AuthService } from './auth/auth.service';

describe('App', () => {
  // Skutečný AuthService by se snažil připojit ke Keycloaku – v testu ho nahradíme falešným.
  const fakeAuth = {
    authenticated: signal(false),
    username: signal<string | null>(null),
    roles: signal<string[]>([]),
    login: () => Promise.resolve(),
    logout: () => Promise.resolve(),
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [App],
      providers: [provideHttpClient(), { provide: AuthService, useValue: fakeAuth }],
    }).compileComponents();
  });

  it('should create the app', () => {
    const fixture = TestBed.createComponent(App);
    const app = fixture.componentInstance;
    expect(app).toBeTruthy();
  });

  it('should show login button when not authenticated', async () => {
    const fixture = TestBed.createComponent(App);
    await fixture.whenStable();
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('h1')?.textContent).toContain('Ekospol');
    expect(compiled.querySelector('button.primary')?.textContent).toContain('Přihlásit');
  });
});
