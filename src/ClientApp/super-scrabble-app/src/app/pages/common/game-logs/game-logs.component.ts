import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Action } from 'src/app/models/action';

@Component({
  selector: 'app-game-logs',
  templateUrl: './game-logs.component.html',
  styleUrls: ['./game-logs.component.css']
})
export class GameLogsComponent implements OnInit {

  @Output('showWordMeaningOf') _showWordMeaningOf: EventEmitter<any> = new EventEmitter();
  @Input() gameLogs: Action[] = [];

  constructor() { }

  ngOnInit(): void {
  }

  showWordMeaningOf(value: string) {
    this._showWordMeaningOf.emit(value);
  }

}
