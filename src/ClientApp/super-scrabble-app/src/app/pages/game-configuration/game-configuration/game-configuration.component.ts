import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { GameConfig } from 'src/app/models/game-configuaration/game-config';
import { GameOption } from 'src/app/models/game-configuaration/game-option';

@Component({
  selector: 'app-game-configuration',
  templateUrl: './game-configuration.component.html',
  styleUrls: ['./game-configuration.component.css']
})
export class GameConfigurationComponent implements OnInit {

  @Input() gameConfig: GameConfig = { 
    title: "",
    gameOptions: []
  };

  @Output() onChosenOption: EventEmitter<any> = new EventEmitter<any>();

  constructor() {
  }

  ngOnInit(): void {
  }

  chooseOption(gameOption: GameOption) {
    this.onChosenOption.emit({chosenValue: gameOption.value});
  }
}
