import { Component, OnInit } from '@angular/core';
import { GameConfig } from 'src/app/models/game-configuaration/game-config';

@Component({
  selector: 'app-game-configuration-menu',
  templateUrl: './game-configuration-menu.component.html',
  styleUrls: ['./game-configuration-menu.component.css']
})
export class GameConfigurationMenuComponent implements OnInit {

  gameConfigs: GameConfig[] = [];

  currentGameConfigIndex: number = 0;

  chosenOptionValuesByConfigurationIndex: Array<number> = [];

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
      },
      {
        title: "Изберете таймер",
        gameOptions: [
          {
            title: "Стандартен",
            description: "Играй самостоятелно срещу други играчи",
            hint: "",
            value: 1,
            backgroundColorHex: ""
          },
          {
            title: "Шах",
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

  getCurrentIndex() {
    if(this.currentGameConfigIndex >= this.gameConfigs.length) {
      return this.gameConfigs.length - 1;
    }

    return this.currentGameConfigIndex;
  }

  isBackButtonEnabled() {
    return this.currentGameConfigIndex != 0; //this.getCurrentIndex() != 0;
  }

  onChosenOption($event: any) {
    let chosenValue = $event.chosenValue;

    if((this.currentGameConfigIndex > this.chosenOptionValuesByConfigurationIndex.length)
        || (this.currentGameConfigIndex >= this.gameConfigs.length)) {
      return;
    }

    if(this.currentGameConfigIndex == this.chosenOptionValuesByConfigurationIndex.length) {
      this.chosenOptionValuesByConfigurationIndex.push(chosenValue);
    } else {
      this.chosenOptionValuesByConfigurationIndex[this.currentGameConfigIndex] = chosenValue;
    }

    if((this.currentGameConfigIndex + 1) < this.gameConfigs.length) {
      this.currentGameConfigIndex++;
    }
  }

  previousConfig() {
    if(this.currentGameConfigIndex > 0) {
      this.chosenOptionValuesByConfigurationIndex.pop();
      this.currentGameConfigIndex--;
    }
  }

  isStartButtonEnabled() {
    return this.currentGameConfigIndex != 0;
  }

  joinRoom() {

  }
}
