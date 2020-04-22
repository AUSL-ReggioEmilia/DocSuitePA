/// <reference path="../../../../node_modules/@types/jasmine/index.d.ts" />
import { TestBed, inject } from '@angular/core/testing';

import { BaseLocalHttpService } from './base-local-http.service';

describe('BaseLocalHttpService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [BaseLocalHttpService]
    });
  });

  it('should be created', inject([BaseLocalHttpService], (service: BaseLocalHttpService) => {
    expect(service).toBeTruthy();
  }));
});
