import { ComponentFixture, TestBed } from '@angular/core/testing';

import { JoinPartyWithCodeDialogComponent } from './join-party-with-code-dialog.component';

describe('JoinPartyWithCodeDialogComponent', () => {
  let component: JoinPartyWithCodeDialogComponent;
  let fixture: ComponentFixture<JoinPartyWithCodeDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ JoinPartyWithCodeDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(JoinPartyWithCodeDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
