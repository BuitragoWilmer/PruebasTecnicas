import { HttpClient, HttpParams } from '@angular/common/http';
import { inject } from '@angular/core';
import { environment } from '../../environments/environment';

export abstract class BaseApiService {
  protected readonly http = inject(HttpClient);
  protected readonly baseUrl = `${environment.apiUrl}/api/v${environment.apiVersion}`;


  protected buildParams(params: {
    [key: string]: any;
    filters?: { [key: string]: string };
  }): HttpParams {
    let httpParams = new HttpParams();

    Object.keys(params).forEach((key) => {
      const value = params[key];
      if (value === undefined || value === null || value === '') return;

      if (key === 'filters' && typeof value === 'object') {
        Object.keys(value).forEach((filterKey) => {
          httpParams = httpParams.set(`filters[${filterKey}]`, value[filterKey]);
        });
      } else {
        httpParams = httpParams.set(key, String(value));
      }
    });

    return httpParams;
  }
}