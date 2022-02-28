import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-matches-dashboard',
  templateUrl: './matches-dashboard.component.html',
  styleUrls: ['./matches-dashboard.component.scss']
})
export class MatchesDashboardComponent implements OnInit {

  matches: any[] = new Array<any>(10);

  constructor() { }

  ngOnInit(): void {
  }

}
