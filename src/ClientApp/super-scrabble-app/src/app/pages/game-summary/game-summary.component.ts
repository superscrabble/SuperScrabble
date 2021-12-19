import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { WebRequestsService } from 'src/app/services/web-requests.service';

@Component({
  selector: 'app-game-summary',
  templateUrl: './game-summary.component.html',
  styleUrls: ['./game-summary.component.css']
})
export class GameSummaryComponent implements OnInit {

  constructor(private router: Router, private webRequestsService: WebRequestsService) { }

  ngOnInit(): void {
    const url = window.location.href;
    const params = url.split("/");
    let id = params[params.length - 2];

    this.loadGameSummary(id);
  }

  loadGameSummary(id: string) {
    const url = 'api/games/summary/' + id;
    
    this.webRequestsService.getAuthorized(url)
    .subscribe({
      next: this.handleLoadGameSummaryResponse.bind(this),
      error: this.handleLoadGameSummaryError.bind(this)
    });
  }

  handleLoadGameSummaryResponse(res: HttpResponse<any>): void {
    let summaryModel = JSON.parse(res.body);
    console.log(summaryModel);
  }

  handleLoadGameSummaryError(error: any): void {

  }
}
