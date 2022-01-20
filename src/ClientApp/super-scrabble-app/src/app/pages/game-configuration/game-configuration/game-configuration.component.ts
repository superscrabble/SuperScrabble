import { Component, Input, OnInit } from '@angular/core';
import { GameConfig } from 'src/app/models/game-configuaration/game-config';

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

  constructor() {
  }

  ngOnInit(): void {
  }

}
