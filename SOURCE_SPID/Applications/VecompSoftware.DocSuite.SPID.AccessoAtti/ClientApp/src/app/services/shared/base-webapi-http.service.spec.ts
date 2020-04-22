/// <reference path="../../../../node_modules/@types/jasmine/index.d.ts" />
import { TestBed, inject } from '@angular/core/testing';

import { BaseWebapiHttpService } from './base-webapi-http.service';

describe('HttpService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
        providers: [BaseWebapiHttpService]
    });
  });

    it('should be created', inject([BaseWebapiHttpService], (service: BaseWebapiHttpService) => {
    expect(service).toBeTruthy();
  }));
});
