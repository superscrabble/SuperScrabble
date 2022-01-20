import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GameOptionComponent } from './game-option.component';

describe('GameOptionComponent', () => {
  let component: GameOptionComponent;
  let fixture: ComponentFixture<GameOptionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ GameOptionComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(GameOptionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
