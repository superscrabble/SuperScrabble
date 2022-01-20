import { Component, OnInit } from '@angular/core';
import { GameConfig } from 'src/app/models/game-configuaration/game-config';

@Component({
  selector: 'app-game-configuration-menu',
  templateUrl: './game-configuration-menu.component.html',
  styleUrls: ['./game-configuration-menu.component.css']
})
export class GameConfigurationMenuComponent implements OnInit {

  gameConfigs: GameConfig[] = [];

  constructor() {
    this.gameConfigs = [
      {
        title: "Изберете вариант",
        gameOptions: [
          {
            title: "Соло",
            description: "Играй самостоятелно срещу други играчи",
            hint: "",
            value: 1,
            backgroundColorHex: ""
          },
          {
            title: "Дуо",
            description: "Играй заедно с приятел или случаен играч срещу други отбори",
            hint: "",
            value: 2,
            backgroundColorHex: ""
          }
        ]
      }
    ]
  }

  ngOnInit(): void {
  }

}
