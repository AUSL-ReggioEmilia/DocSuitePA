/// <reference path="../../../../../node_modules/@types/jasmine/index.d.ts" />
import { TestBed } from '@angular/core/testing';

import { CacheInterceptorService } from './cache-interceptor.service';

describe('CacheInterceptorService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: CacheInterceptorService = TestBed.get(CacheInterceptorService);
    expect(service).toBeTruthy();
  });
});
