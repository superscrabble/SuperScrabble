import { Injectable } from '@angular/core';
import { GameType } from '../common/enums/game-type';
import { GameConfig } from '../models/game-configuaration/game-config';
import { MatchProps } from '../models/game-configuaration/match-props';

@Injectable({
  providedIn: 'root'
})
export class MatchmakingService {

  gameConfigs: GameConfig[] = [];
  currentConfigIndex: number = 0;
  matchProps: MatchProps = new MatchProps();
  //TODO: probably add such configs for other types; move this into a structure
  additionalTeamConfigs: GameConfig[] = [];
  additionalTeamConfigsIndex: number = 0;

  constructor() {
    //REMOVE
    this.matchProps.type = GameType.Duo;

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
      },
      {
        title: "Изберете време за таймера",
        gameOptions: [
          {
            title: "Бързо",
            description: "Играй самостоятелно срещу други играчи",
            hint: "",
            value: 1,
            backgroundColorHex: ""
          },
          {
            title: "Стандартно",
            description: "Играй заедно с приятел или случаен играч срещу други отбори",
            hint: "",
            value: 2,
            backgroundColorHex: ""
          },
          {
            title: "Бавно",
            description: "Играй заедно с приятел или случаен играч срещу други отбори",
            hint: "",
            value: 3,
            backgroundColorHex: ""
          }
        ]
      }
    ]

    this.additionalTeamConfigs = [
      {
        title: "Играй с:",
        gameOptions: [
          {
            title: "Случаен играч",
            description: "Играй самостоятелно срещу други играчи",
            hint: "",
            value: 1,
            backgroundColorHex: ""
          },
          {
            title: "Приятел",
            description: "Играй заедно с приятел или случаен играч срещу други отбори",
            hint: "",
            value: 2,
            backgroundColorHex: ""
          }
        ]
      }
    ]
  }

  chooseOption() {
    
    /*if((this.currentGameConfigIndex > this.chosenOptionValuesByConfigurationIndex.length)
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
    }*/

    this.currentConfigIndex++;
  }

  getCurrentConfig() {
    if(this.isLastBasicConfig() && this.isTeamGame()) {
      return this.additionalTeamConfigs[this.additionalTeamConfigsIndex];
    }

    return this.gameConfigs[this.currentConfigIndex];
  }

  previousConfig() {
    if(this.currentConfigIndex > 0) {
      //this.chosenOptionValuesByConfigurationIndex.pop();
      this.currentConfigIndex--;
    }
  }

  isTeamGame() {
    return this.matchProps.type == GameType.Duo;
  }

  isFirstConfig() : boolean {
    return this.currentConfigIndex == 0;
  }

  private isLastBasicConfig() : boolean {
    return (this.currentConfigIndex + 1) > this.gameConfigs.length;
  }

  isLastConfig() : boolean {
    //TODO: probably add additional check
    //If TeamGame() and a parther is chosen

    if(this.isTeamGame() && this.isLastBasicConfig()) {

    }

    return (this.currentConfigIndex + 1) == this.gameConfigs.length;
  }
}
