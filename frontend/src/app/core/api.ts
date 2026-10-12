import { InjectionToken } from '@angular/core';

/**
 * URL base da API. Em desenvolvimento, `/api` é encaminhado ao backend pelo
 * proxy do `ng serve` (veja `proxy.conf.json`), o que dispensa CORS e URLs fixas.
 */
export const API_URL = new InjectionToken<string>('API_URL', {
  providedIn: 'root',
  factory: () => '/api',
});
