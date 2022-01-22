import { Injectable } from '@angular/core';
import { GameType } from '../common/enums/game-type';
import { ConfigsPath } from '../models/game-configuaration/configs-path';
import { GameConfig } from '../models/game-configuaration/game-config';
import { MatchProps } from '../models/game-configuaration/match-props';

@Injectable({
  providedIn: 'root'
})
export class MatchmakingService {

  standardConfigs: ConfigsPath;
  additionalConfigs: Map<GameType, ConfigsPath> = new Map();
  matchProps: MatchProps = new MatchProps();
  isOnAdditionalConfigs: boolean = false;

  //TODO: add property for currentConfig

  constructor() {
    //REMOVE
    //this.matchProps.type = GameType.Duo;
    const nameof = <T>(name: keyof T) => name;

    this.standardConfigs = new ConfigsPath([
      {
        title: "Изберете вариант",
        inputPropName: nameof<MatchProps>("type"),
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
        inputPropName: nameof<MatchProps>("timerType"),
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
        inputPropName: nameof<MatchProps>("timerTimeType"),
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
    ]);

    this.additionalConfigs.set(GameType.Duo, new ConfigsPath([
      {
        title: "Играй с:",
        inputPropName: "Nan",
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
    ]));
  }

  chooseOption(value: any) {
    //console.log("Prop value");
    //console.log(this.getCurrentConfig()?.inputPropName);
    const a: string = "${this.getCurrentConfig().inputPropName}";
    let prop = this.matchProps["teamCount"];

    //this.matchProps[Object.getOwnPropertyNames()[0]] = 1;
    prop = value;

    if(this.isOnAdditionalConfigs) {
      this.additionalConfigs.get(this.matchProps.type)?.nextConfig();
      return;
    }

    if(this.standardConfigs.isLastConfig()) {
      this.standardConfigs.nextConfig();
      if(this.additionalConfigs.has(this.matchProps.type)) {
        this.isOnAdditionalConfigs = true;
      }
      return;
    }
    
    this.standardConfigs.nextConfig();
  }

  getCurrentConfig() : GameConfig{
    let currentConfig = this.standardConfigs.getCurrentConfig();
    if(this.isOnAdditionalConfigs) {
      let additionalConfig = this.additionalConfigs.get(this.matchProps.type)?.getCurrentConfig();
      if(additionalConfig) {
        currentConfig = additionalConfig;
      }
    }

    return currentConfig;
  }

  previousConfig() {
    if(this.isOnAdditionalConfigs) {
      if(this.additionalConfigs.get(this.matchProps.type)?.isFirstConfig()) {
        this.isOnAdditionalConfigs = false;
      }

      this.additionalConfigs.get(this.matchProps.type)?.previousConfig;
      return;
    }
    this.standardConfigs.previousConfig();
  }

  isTeamGame() {
    return this.matchProps.type == GameType.Duo;
  }

  isFirstConfig() : boolean {
    return this.standardConfigs.isFirstConfig();
  }

  isLastConfig() : boolean {
    //TODO: simplify this
    if(this.standardConfigs.isLastConfig() && this.standardConfigs.isFinished) {
      if(this.isOnAdditionalConfigs) {
        if(this.additionalConfigs.get(this.matchProps.type)?.isFinished) {
          return true;
        }
        return false;
      }
      return true;
    }
    return false;
  }
}
