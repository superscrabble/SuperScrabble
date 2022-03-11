import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChangeWildcardDialogComponent } from './change-wildcard-dialog.component';

describe('ChangeWildcardDialogComponent', () => {
  let component: ChangeWildcardDialogComponent;
  let fixture: ComponentFixture<ChangeWildcardDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ChangeWildcardDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ChangeWildcardDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
