import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ExchangeTilesDialogComponent } from './exchange-tiles-dialog.component';

describe('ExchangeTilesDialogComponent', () => {
  let component: ExchangeTilesDialogComponent;
  let fixture: ComponentFixture<ExchangeTilesDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ExchangeTilesDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ExchangeTilesDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
