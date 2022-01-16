import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChangeLettersDialogComponent } from './change-letters-dialog.component';

describe('ChangeLettersDialogComponent', () => {
  let component: ChangeLettersDialogComponent;
  let fixture: ComponentFixture<ChangeLettersDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ChangeLettersDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ChangeLettersDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
