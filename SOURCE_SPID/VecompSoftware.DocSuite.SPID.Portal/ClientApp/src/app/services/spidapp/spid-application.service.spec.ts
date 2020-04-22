/// <reference path="../../../../node_modules/@types/jasmine/index.d.ts" />
import { TestBed, inject } from '@angular/core/testing';

import { SpidApplicationService } from './spid-application.service';

describe('SpidApplicationService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [SpidApplicationService]
    });
  });

  it('should be created', inject([SpidApplicationService], (service: SpidApplicationService) => {
    expect(service).toBeTruthy();
  }));
});
