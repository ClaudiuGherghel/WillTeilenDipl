import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddEditImages } from './add-edit-images';

describe('AddEditImages', () => {
  let component: AddEditImages;
  let fixture: ComponentFixture<AddEditImages>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddEditImages]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AddEditImages);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
