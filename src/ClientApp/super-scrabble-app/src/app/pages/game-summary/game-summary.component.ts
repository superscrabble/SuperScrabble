import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { WebRequestsService } from 'src/app/services/web-requests.service';
import { ErrorHandler } from 'src/app/services/error-handler';
import { LoadingScreenService } from 'src/app/services/loading-screen.service';
import { AngularFireRemoteConfig } from '@angular/fire/compat/remote-config';

@Component({
  selector: 'app-game-summary',
  templateUrl: './game-summary.component.html',
  styleUrls: ['./game-summary.component.css']
})
export class GameSummaryComponent implements OnInit {
  
  pointsByUserNames: any[] = new Array();
  gameOutcomeMessage: string = "";
  gameOutcomeNumber: number = 0;

  summaryWinText: string = "";
  summaryDefeatText: string = "";
  summaryDrawText: string = "";
  summaryPageCaption: string = "";
  summaryScoreboardPlayerLabel: string = "";
  summaryScoreboardPointsLabel: string = "";

  constructor(private router: Router, private webRequestsService: WebRequestsService,
              private errorHandler: ErrorHandler, private loadingScreenService: LoadingScreenService,
              private remoteConfig: AngularFireRemoteConfig) { 
    this.loadRemoteConfigTexts();
  }
  
  private loadRemoteConfigTexts() {
    //AppConfig.isRemoteConfigFetched = false;
    this.remoteConfig.fetchAndActivate().then(hasActivatedTheFetch => {
      this.remoteConfig.getAll().then(all => {
        //AppConfig.isRemoteConfigFetched = true;
        this.summaryWinText = all["SummaryWinText"].asString()!;
        this.summaryDefeatText = all["SummaryDefeatText"].asString()!;
        this.summaryDrawText = all["SummaryDrawText"].asString()!;
        this.summaryPageCaption = all["SummaryPageCaption"].asString()!;
        this.summaryScoreboardPlayerLabel = all["SummaryScoreboardPlayerLabel"].asString()!;
        this.summaryScoreboardPointsLabel = all["SummaryScoreboardPointsLabel"].asString()!;

        this.gameOutcomeMessage = this.getRemoteConfigGameOutcomeMessage(this.gameOutcomeMessage);
      })
    })
  }

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
    
    //this.gameOutcomeMessage = this.getRemoteConfigGameOutcomeMessage(this.gameOutcomeMessage);    

    this.loadingScreenService.stopShowingLoadingScreen();
  }

  getRemoteConfigGameOutcomeMessage(gameOutcomeMessage: string) : string {
    switch(gameOutcomeMessage) {
      case("Win"): {
        return this.summaryWinText;
      }
      case("Defeat"): {
        return this.summaryDefeatText;
      }
      case("Draw"): {
        return this.summaryDrawText;
      }
      default: {
        return this.summaryDrawText;
      }
    }
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
