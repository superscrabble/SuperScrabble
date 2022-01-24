import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { GameType } from '../common/enums/game-type';
import { PartherType } from '../common/enums/parther-type';
import { TimerTimeType } from '../common/enums/timer-time-type';
import { TimerType } from '../common/enums/timer-type';
import { ConfigsPath } from '../models/game-configuaration/configs-path';
import { GameConfig } from '../models/game-configuaration/game-config';
import { GameOption } from '../models/game-configuaration/game-option';
import { MatchProps } from '../models/game-configuaration/match-props';
import { GameInviteFriendsDialogComponent } from '../pages/game-configuration/dialogs/game-invite-friends-dialog/game-invite-friends-dialog.component';

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
  //TODO: load data from the server and the text from Firebase

  constructor(private dialog: MatDialog) {
    let teamCountConfig = new GameConfig(
      "Изберете броя на отборите",
      [
        new GameOption(
          "1",
          "",
          2,
        ),
        new GameOption(
          "",
          "",
          3,
        ),
        new GameOption(
          "",
          "",
          4,
        )
      ],
      (option: GameOption) => {
        this.matchProps.teamCount = option.value;
      });
    
    teamCountConfig.isAboutTeamCount = true;

    this.standardConfigs = new ConfigsPath([
      new GameConfig(
        "Изберете вариант",
        [
          new GameOption(
            "Соло",
            "Играй самостоятелно срещу други играчи",
            GameType.Single,
          ),
          new GameOption(
            "Дуо",
            "Играй заедно с приятел или случаен играч срещу други отбори",
            GameType.Duo,
          )
        ],
        (option: GameOption) => {
          this.matchProps.type = option.value;
        }),
      new GameConfig(
        "Изберете таймер",
        [
          new GameOption(
            "Стандартен",
            "Играй самостоятелно срещу други играчи",
            TimerType.Standard,
          ),
          new GameOption(
            "Шах",
            "Играй заедно с приятел или случаен играч срещу други отбори",
            TimerType.Chess,
          )
        ],
        (option: GameOption) => {
          this.matchProps.timerType = option.value;
        }),
      new GameConfig(
        "Изберете време за таймера",
        [
          new GameOption(
            "Бързо",
            "Играй самостоятелно срещу други играчи",
            TimerTimeType.Fast,  
          ),
          new GameOption(
            "Стандартно",
            "Играй заедно с приятел или случаен играч срещу други отбори",
            TimerTimeType.Standard,
          ),
          new GameOption(
            "Бавно",
            "Играй заедно с приятел или случаен играч срещу други отбори",
            TimerTimeType.Slow, 
          ),
          new GameOption(
            "Бавно",
            "Играй заедно с приятел или случаен играч срещу други отбори",
            TimerTimeType.Slow, 
          )
        ],
        (option: GameOption) => {
          this.matchProps.timerTimeType = option.value;
        }
      ),
      teamCountConfig
    ]);

    this.additionalConfigs.set(GameType.Duo, new ConfigsPath([
      new GameConfig(
        "Играй с:",
        [
          new GameOption(
            "Случаен играч",
            "Играй самостоятелно срещу други играчи",
            PartherType.Random, 
          ),
          new GameOption(
            "Приятел",
            "Играй заедно с приятел или случаен играч срещу други отбори",
            PartherType.InviteFriends
          )
        ],
        (option: GameOption) => {
          if(option.value == PartherType.InviteFriends) {
            this.waitingForFriend = true;
            this.dialog.open(GameInviteFriendsDialogComponent);
            //Subscribe on close;
            return;
          }

          this.waitingForFriend = false;
        }
      )]));
  }

  chooseOption(value: GameOption) {
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
