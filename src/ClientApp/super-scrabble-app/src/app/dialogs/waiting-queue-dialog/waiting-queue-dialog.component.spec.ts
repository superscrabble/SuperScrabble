import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WaitingQueueDialogComponent } from './waiting-queue-dialog.component';

describe('WaitingQueueDialogComponent', () => {
  let component: WaitingQueueDialogComponent;
  let fixture: ComponentFixture<WaitingQueueDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ WaitingQueueDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(WaitingQueueDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
