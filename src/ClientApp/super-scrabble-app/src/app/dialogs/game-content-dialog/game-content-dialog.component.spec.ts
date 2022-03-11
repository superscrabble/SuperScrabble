import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GameContentDialogComponent } from './game-content-dialog.component';

describe('GameContentDialogComponent', () => {
  let component: GameContentDialogComponent;
  let fixture: ComponentFixture<GameContentDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ GameContentDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(GameContentDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
