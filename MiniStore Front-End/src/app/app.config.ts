import {
  ApplicationConfig,
  inject,
  provideAppInitializer,
  provideBrowserGlobalErrorListeners
} from '@angular/core';
import { HttpClient, provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { ApiSecurityService } from './core/api/api-security.service';
import { ApiSessionService } from './core/api/api-session.service';
import { csrfInterceptor } from './core/api/csrf.interceptor';
import { authFailureInterceptor } from './core/api/auth-failure.interceptor';
import { operationErrorToastInterceptor } from './core/api/operation-error-toast.interceptor';
import { firstValueFrom } from 'rxjs';
import { BranchContextService } from './core/branch-context/branch-context.service';
import { branchContextInterceptor } from './core/branch-context/branch-context.interceptor';
import { BranchWorkspace } from './features/branches/branches-api.service';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideHttpClient(withFetch(), withInterceptors([branchContextInterceptor, csrfInterceptor, operationErrorToastInterceptor, authFailureInterceptor])),
    provideAppInitializer(() => {
      const security = inject(ApiSecurityService);
      const session = inject(ApiSessionService);
      const http = inject(HttpClient);
      const branchContext = inject(BranchContextService);
      return (async () => {
        await Promise.allSettled([security.initialize(), session.load()]);
        if (!session.can('Branches.View')) { branchContext.clear(); return; }
        try {
          const page = await firstValueFrom(http.get<BranchWorkspace>('/api/v1/branches'));
          branchContext.initialize(session.session(), page.branches);
        } catch { branchContext.clear(); }
      })();
    }),
    provideRouter(routes)
  ]
};
