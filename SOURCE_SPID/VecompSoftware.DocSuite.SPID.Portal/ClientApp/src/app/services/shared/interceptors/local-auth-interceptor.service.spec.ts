/// <reference path="../../../../../node_modules/@types/jasmine/index.d.ts" />
import { TestBed, inject } from '@angular/core/testing';

import { LocalAuthInterceptorService } from './local-auth-interceptor.service';

describe('LocalAuthInterceptorService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [LocalAuthInterceptorService]
    });
  });

  it('should be created', inject([LocalAuthInterceptorService], (service: LocalAuthInterceptorService) => {
    expect(service).toBeTruthy();
  }));
});
