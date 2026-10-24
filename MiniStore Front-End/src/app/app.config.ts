import {
  ApplicationConfig,
  inject,
  provideAppInitializer,
  provideBrowserGlobalErrorListeners
} from '@angular/core';
import { provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { ApiSecurityService } from './core/api/api-security.service';
import { ApiSessionService } from './core/api/api-session.service';
import { csrfInterceptor } from './core/api/csrf.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideHttpClient(withFetch(), withInterceptors([csrfInterceptor])),
    provideAppInitializer(() => {
      const security = inject(ApiSecurityService);
      const session = inject(ApiSessionService);
      return Promise.allSettled([security.initialize(), session.load()]);
    }),
    provideRouter(routes)
  ]
};
