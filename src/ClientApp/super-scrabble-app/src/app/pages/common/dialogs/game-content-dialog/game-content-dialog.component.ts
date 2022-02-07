import { Component, EventEmitter, Inject, Input, OnInit, Output } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Action } from 'src/app/models/action';

interface GameContentDialogData {
  gameLogs: Action[];
  teams: any[];
  currentUserName: string;
  userNamesOfPlayersWhoHaveLeftTheGame: string[];
  showWordMeaningOf: Function;
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

  constructor(@Inject(MAT_DIALOG_DATA) public _data: GameContentDialogData) {
    this.data = _data;
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
