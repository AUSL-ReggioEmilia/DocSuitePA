import { TestBed } from '@angular/core/testing';

import { BaseLocalHttpService } from './base-local-http.service';

describe('BaseLocalHttpService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: BaseLocalHttpService = TestBed.get(BaseLocalHttpService);
    expect(service).toBeTruthy();
  });
});
