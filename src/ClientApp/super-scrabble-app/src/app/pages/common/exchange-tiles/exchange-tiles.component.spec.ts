import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ExchangeTilesComponent } from './exchange-tiles.component';

describe('ExchangeTilesComponent', () => {
  let component: ExchangeTilesComponent;
  let fixture: ComponentFixture<ExchangeTilesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ExchangeTilesComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ExchangeTilesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
