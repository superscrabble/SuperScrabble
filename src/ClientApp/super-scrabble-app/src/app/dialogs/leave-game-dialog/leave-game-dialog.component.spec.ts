import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LeaveGameDialogComponent } from './leave-game-dialog.component';

describe('LeaveGameDialogComponent', () => {
  let component: LeaveGameDialogComponent;
  let fixture: ComponentFixture<LeaveGameDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ LeaveGameDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(LeaveGameDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
