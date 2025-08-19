import { TestBed } from '@angular/core/testing';

import { GeoPostalService } from './geo-postal-service';

describe('GeoPostalService', () => {
  let service: GeoPostalService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(GeoPostalService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
