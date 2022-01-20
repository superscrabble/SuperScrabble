import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GameConfigurationMenuComponent } from './game-configuration-menu.component';

describe('GameConfigurationMenuComponent', () => {
  let component: GameConfigurationMenuComponent;
  let fixture: ComponentFixture<GameConfigurationMenuComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ GameConfigurationMenuComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(GameConfigurationMenuComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
