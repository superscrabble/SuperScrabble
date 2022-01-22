import { Injectable } from '@angular/core';
import { GameType } from '../common/enums/game-type';
import { PartherType } from '../common/enums/parther-type';
import { TimerTimeType } from '../common/enums/timer-time-type';
import { TimerType } from '../common/enums/timer-type';
import { ConfigsPath } from '../models/game-configuaration/configs-path';
import { GameConfig } from '../models/game-configuaration/game-config';
import { GameOption } from '../models/game-configuaration/game-option';
import { MatchProps } from '../models/game-configuaration/match-props';

@Injectable({
  providedIn: 'root'
})
export class MatchmakingService {

  standardConfigs: ConfigsPath;
  additionalConfigs: Map<GameType, ConfigsPath> = new Map();
  matchProps: MatchProps = new MatchProps();
  isOnAdditionalConfigs: boolean = false;
  waitingForFriend: boolean = false;

  //TODO: add property for currentConfig

  constructor() {
    //REMOVE
    //this.matchProps.type = GameType.Duo;
    const nameof = <T>(name: keyof T) => name;

    this.standardConfigs = new ConfigsPath([
      new GameConfig(
        "Изберете вариант",
        [
          {
            title: "Соло",
            description: "Играй самостоятелно срещу други играчи",
            hint: "",
            value: GameType.Single,
            backgroundColorHex: ""
          },
          {
            title: "Дуо",
            description: "Играй заедно с приятел или случаен играч срещу други отбори",
            hint: "",
            value: GameType.Duo,
            backgroundColorHex: ""
          }
        ],
        (option: GameOption) => {
          this.matchProps.type = option.value;
        }),
      new GameConfig(
        "Изберете таймер",
        [
          {
            title: "Стандартен",
            description: "Играй самостоятелно срещу други играчи",
            hint: "",
            value: TimerType.Standard,
            backgroundColorHex: ""
          },
          {
            title: "Шах",
            description: "Играй заедно с приятел или случаен играч срещу други отбори",
            hint: "",
            value: TimerType.Chess,
            backgroundColorHex: ""
          }
        ],
        (option: GameOption) => {
          this.matchProps.timerType = option.value;
        }),
      new GameConfig(
        "Изберете време за таймера",
        [
          {
            title: "Бързо",
            description: "Играй самостоятелно срещу други играчи",
            hint: "",
            value: TimerTimeType.Fast,
            backgroundColorHex: ""
          },
          {
            title: "Стандартно",
            description: "Играй заедно с приятел или случаен играч срещу други отбори",
            hint: "",
            value: TimerTimeType.Standard,
            backgroundColorHex: ""
          },
          {
            title: "Бавно",
            description: "Играй заедно с приятел или случаен играч срещу други отбори",
            hint: "",
            value: TimerTimeType.Slow,
            backgroundColorHex: ""
          }
        ],
        (option: GameOption) => {
          this.matchProps.timerTimeType = option.value;
        }
      )]);

    this.additionalConfigs.set(GameType.Duo, new ConfigsPath([
      new GameConfig(
        "Играй с:",
        [
          {
            title: "Случаен играч",
            description: "Играй самостоятелно срещу други играчи",
            hint: "",
            value: PartherType.Random,
            backgroundColorHex: ""
          },
          {
            title: "Приятел",
            description: "Играй заедно с приятел или случаен играч срещу други отбори",
            hint: "",
            value: PartherType.InviteFriends,
            backgroundColorHex: ""
          }
        ],
        (option: GameOption) => {
          if(option.value == PartherType.InviteFriends) {
            //openDialog
            this.waitingForFriend = true;
            return;
          }

          this.waitingForFriend = false;
        }
      )]));
  }

  chooseOption(value: GameOption) {
    //console.log("Prop value");
    //console.log(this.getCurrentConfig()?.inputPropName);
    const a: string = "${this.getCurrentConfig().inputPropName}";
    //let prop = this.matchProps["teamCount"];

    //this.matchProps[Object.getOwnPropertyNames()[0]] = 1;
    //prop = value;

    console.log("Match props");
    console.log(this.matchProps)

    if(this.isOnAdditionalConfigs) {
      this.additionalConfigs.get(this.matchProps.type)?.getCurrentConfig().selectOption(value);
      this.additionalConfigs.get(this.matchProps.type)?.nextConfig();
      return;
    }

    this.standardConfigs.getCurrentConfig().selectOption(value);

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
    this.waitingForFriend = false;
  }

  isTeamGame() {
    return this.matchProps.type == GameType.Duo;
  }

  isFirstConfig() : boolean {
    return this.standardConfigs.isFirstConfig();
  }

  isConfigReady() : boolean {
    //Check if waiting for friends
    if(this.waitingForFriend) {
      return false;
    }
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
