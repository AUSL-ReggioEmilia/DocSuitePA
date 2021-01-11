/// <reference path="../../../../../node_modules/@types/jasmine/index.d.ts" />
import { TestBed, inject } from '@angular/core/testing';

import { CacheInterceptorService } from './cache-interceptor.service';

describe('CacheInterceptorService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [CacheInterceptorService]
    });
  });

  it('should be created', inject([CacheInterceptorService], (service: CacheInterceptorService) => {
    expect(service).toBeTruthy();
  }));
});
