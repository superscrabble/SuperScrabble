import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GameInviteFriendsDialogComponent } from './game-invite-friends-dialog.component';

describe('GameInviteFriendsDialogComponent', () => {
  let component: GameInviteFriendsDialogComponent;
  let fixture: ComponentFixture<GameInviteFriendsDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ GameInviteFriendsDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(GameInviteFriendsDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
