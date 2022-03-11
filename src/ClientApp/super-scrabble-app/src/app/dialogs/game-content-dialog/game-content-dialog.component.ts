import { Component, EventEmitter, Inject, Input, OnInit, Output } from '@angular/core';
import { AngularFireRemoteConfig } from '@angular/fire/compat/remote-config';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Log } from 'src/app/models/enums/log';
import { Action } from 'src/app/models/action';

interface GameContentDialogData {
  gameLogs: Log[];
  teams: any[];
  currentUserName: string;
  userNamesOfPlayersWhoHaveLeftTheGame: string[];
  showWordMeaningOf: Function;
  isDuoGame: boolean;
}

export enum ShowingComponent {
  Scoreboard,
  GameLogs,
  WordInfo
}

@Component({
  selector: 'app-game-content-dialog',
  templateUrl: './game-content-dialog.component.html',
  styleUrls: ['./game-content-dialog.component.css']
})
//TODO: change the class's name
export class GameContentDialogComponent implements OnInit {

  showingComponent: ShowingComponent = ShowingComponent.Scoreboard;
  showingComponentType: typeof ShowingComponent = ShowingComponent;

  data: GameContentDialogData;
  
  scoreboardLabel: string = "";
  gameLogsLabel: string = "";
  wordInfoLabel: string = "";

  constructor(@Inject(MAT_DIALOG_DATA) public _data: GameContentDialogData,
              private remoteConfig: AngularFireRemoteConfig) {
    this.data = _data;

    this.loadRemoteConfigTexts();
  }

  private loadRemoteConfigTexts() {
    //AppConfig.isRemoteConfigFetched = false;
    this.remoteConfig.fetchAndActivate().then(hasActivatedTheFetch => {
      this.remoteConfig.getAll().then(all => {
        //AppConfig.isRemoteConfigFetched = true;
        this.scoreboardLabel = all["ScoreboardLabel"].asString()!;
        this.gameLogsLabel = all["GameLogsLabel"].asString()!;
        this.wordInfoLabel = all["WordInfoLabel"].asString()!;
      })
    })
  }

  ngOnInit(): void {
  }

  showWordMeaningOf(value: string) {
    this.data.showWordMeaningOf.call(value);
  }

  changeShowingComponent(value: ShowingComponent) {
    this.showingComponent = value;
  }
}
