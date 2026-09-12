import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { acceptJsonInterceptor } from './core/interceptors/accept-json.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
     provideHttpClient(
      withInterceptors([acceptJsonInterceptor])
    ),
  ],
};
