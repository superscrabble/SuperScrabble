import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { TeamType } from '../common/enums/game-type';
import { PartnerType } from '../common/enums/partner-type';
import { PartyType } from '../common/enums/party-type';
import { TimerDifficulty } from '../common/enums/timer-difficulty';
import { TimerType } from '../common/enums/timer-type';
import { ConfigsPath } from '../models/game-configuaration/configs-path';
import { GameConfig } from '../models/game-configuaration/game-config';
import { GameOption } from '../models/game-configuaration/game-option';
import { MatchProps } from '../models/game-configuaration/match-props';
import { GameInviteFriendsDialogComponent } from '../pages/game-configuration/dialogs/game-invite-friends-dialog/game-invite-friends-dialog.component';
import { SignalrService } from './signalr.service';

@Injectable({
  providedIn: 'root'
})
export class MatchmakingService {

  standardConfigs: ConfigsPath;
  additionalConfigs: Map<TeamType, ConfigsPath> = new Map();
  matchProps: MatchProps = new MatchProps();
  isOnAdditionalConfigs: boolean = false;

  //TODO: add property for currentConfig
  //TODO: load data from the server and the text from Firebase

  constructor(private dialog: MatDialog, private signalrService: SignalrService) {
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
        this.matchProps.teamsCount = option.value;
      });
    
    teamCountConfig.isAboutTeamCount = true;

    this.standardConfigs = new ConfigsPath([
      new GameConfig(
        "Изберете вариант",
        [
          new GameOption(
            "Соло",
            "Играй самостоятелно срещу други играчи",
            TeamType.Solo,
          ),
          new GameOption(
            "Дуо",
            "Играй заедно с приятел или случаен играч срещу други отбори",
            TeamType.Duo,
          )
        ],
        (option: GameOption) => {
          this.matchProps.teamType = option.value;
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
            TimerDifficulty.Fast,  
          ),
          new GameOption(
            "Стандартно",
            "Играй заедно с приятел или случаен играч срещу други отбори",
            TimerDifficulty.Normal,
          ),
          new GameOption(
            "Бавно",
            "Играй заедно с приятел или случаен играч срещу други отбори",
            TimerDifficulty.Slow, 
          ),
          new GameOption(
            "Бавно",
            "Играй заедно с приятел или случаен играч срещу други отбори",
            TimerDifficulty.Slow, 
          )
        ],
        (option: GameOption) => {
          this.matchProps.timerDifficulty = option.value;
        }
      ),
      teamCountConfig
    ]);

    this.additionalConfigs.set(TeamType.Duo, new ConfigsPath([
      new GameConfig(
        "Играй с:",
        [
          new GameOption(
            "Случаен играч",
            "Играй самостоятелно срещу други играчи",
            PartnerType.Random, 
          ),
          new GameOption(
            "Приятел",
            "Играй заедно с приятел или случаен играч срещу други отбори",
            PartnerType.Friend
          )
        ],
        (option: GameOption) => {
          this.matchProps.partnerType = option.value;
        }
      )]));
  }

  chooseOption(value: GameOption) {
    if(this.isOnAdditionalConfigs) {
      this.additionalConfigs.get(this.matchProps.teamType)?.getCurrentConfig().selectOption(value);
      this.additionalConfigs.get(this.matchProps.teamType)?.nextConfig();
      return;
    }

    this.standardConfigs.getCurrentConfig().selectOption(value);

    if(this.standardConfigs.isLastConfig()) {
      this.standardConfigs.nextConfig();
      if(this.additionalConfigs.has(this.matchProps.teamType)) {
        this.isOnAdditionalConfigs = true;
      }
      return;
    }
    
    this.standardConfigs.nextConfig();
  }

  getCurrentConfig() : GameConfig{
    let currentConfig = this.standardConfigs.getCurrentConfig();
    if(this.isOnAdditionalConfigs) {
      let additionalConfig = this.additionalConfigs.get(this.matchProps.teamType)?.getCurrentConfig();
      if(additionalConfig) {
        currentConfig = additionalConfig;
      }
    }

    return currentConfig;
  }

  previousConfig() {
    if(this.isOnAdditionalConfigs) {
      if(this.additionalConfigs.get(this.matchProps.teamType)?.isFirstConfig()) {
        this.isOnAdditionalConfigs = false;
      }

      this.additionalConfigs.get(this.matchProps.teamType)?.previousConfig;
      return;
    }
    this.standardConfigs.previousConfig();
  }

  isTeamGame() {
    return this.matchProps.teamType == TeamType.Duo;
  }

  isFirstConfig() : boolean {
    return this.standardConfigs.isFirstConfig();
  }

  isConfigReady() : boolean {
    //TODO: simplify this
    if(this.standardConfigs.isLastConfig() && this.standardConfigs.isFinished) {
      if(this.isOnAdditionalConfigs) {
        if(this.additionalConfigs.get(this.matchProps.teamType)?.isFinished) {
          return true;
        }
        return false;
      }
      return true;
    }
    return false;
  }

  joinRoom(): void {
    this.signalrService.joinRoomWithProps(this.matchProps);
  }

  createParty(type: PartyType) {
    this.signalrService.createParty(type);
  }

  joinParty(code: string) {
    this.signalrService.joinParty(code);
  }

  StartGameFromParty(partyId: string) {
    this.signalrService.StartGameFromParty(partyId);
  }
}
