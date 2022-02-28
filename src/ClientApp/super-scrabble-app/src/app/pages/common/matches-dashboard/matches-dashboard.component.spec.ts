import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MatchesDashboardComponent } from './matches-dashboard.component';

describe('MatchesDashboardComponent', () => {
  let component: MatchesDashboardComponent;
  let fixture: ComponentFixture<MatchesDashboardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MatchesDashboardComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MatchesDashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
