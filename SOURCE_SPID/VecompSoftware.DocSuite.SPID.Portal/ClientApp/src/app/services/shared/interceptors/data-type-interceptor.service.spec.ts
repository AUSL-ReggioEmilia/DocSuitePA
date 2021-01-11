/// <reference path="../../../../../node_modules/@types/jasmine/index.d.ts" />
import { TestBed, inject } from '@angular/core/testing';

import { DataTypeInterceptorService } from './data-type-interceptor.service';

describe('DataTypeInterceptorService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [DataTypeInterceptorService]
    });
  });

  it('should be created', inject([DataTypeInterceptorService], (service: DataTypeInterceptorService) => {
    expect(service).toBeTruthy();
  }));
});
