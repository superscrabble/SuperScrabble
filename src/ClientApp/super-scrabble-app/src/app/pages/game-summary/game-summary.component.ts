import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { WebRequestsService } from 'src/app/services/web-requests.service';
import { ErrorHandler } from 'src/app/services/error-handler';
import { LoadingScreenService } from 'src/app/services/loading-screen.service';

@Component({
  selector: 'app-game-summary',
  templateUrl: './game-summary.component.html',
  styleUrls: ['./game-summary.component.css']
})
export class GameSummaryComponent implements OnInit {

  constructor(private router: Router, private webRequestsService: WebRequestsService,
              private errorHandler: ErrorHandler, private loadingScreenService: LoadingScreenService) { }

  pointsByUserNames: any[] = new Array();
  gameOutcomeMessage: string = "";
  gameOutcomeNumber: number = 0;

  ngOnInit(): void {
    const url = window.location.href;
    const params = url.split("/");
    let id = params[params.length - 2];

    this.loadGameSummary(id);
  }

  loadGameSummary(id: string) {
    this.loadingScreenService.showLoadingScreen();
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
    this.loadScoreBoard(summaryModel.pointsByUserNames);
    this.gameOutcomeNumber = summaryModel.gameOutcomeNumber;
    this.gameOutcomeMessage = summaryModel.gameOutcomeMessage;
    this.loadingScreenService.stopShowingLoadingScreen();
  }

  handleLoadGameSummaryError(error: any): void {
    this.errorHandler.handle(error.status);
  }

  loadScoreBoard(pointsByUserNames: any): void {
    this.pointsByUserNames = [];
    for(let i = 0; i < pointsByUserNames.length; i++) {
      this.pointsByUserNames.push({key: pointsByUserNames[i].key, value: pointsByUserNames[i].value});
    }
  }
}
