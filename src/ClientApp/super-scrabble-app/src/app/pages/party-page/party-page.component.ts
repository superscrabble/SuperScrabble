import { Component, OnInit } from '@angular/core';
import { AngularFireRemoteConfig } from '@angular/fire/compat/remote-config';
import { Router } from '@angular/router';
import { HubConnectionState } from '@microsoft/signalr';
import { PartyType } from 'src/app/models/enums/party-type';
import { TimerType } from 'src/app/models/enums/timer-type';
//import { GameConfig } from 'src/app/models/game-configuaration/game-config';
import { LoadingScreenService } from 'src/app/services/loading-screen.service';
import { MatchmakingService } from 'src/app/services/matchmaking.service';
import { SignalrService } from 'src/app/services/signalr.service';

class PartyData {
  invitationCode: string = "";
  partyType: PartyType = PartyType.Duo;
  //gameConfig?: GameConfig;
  owner: string = "";
  members: string[] = [];
  isOwner: boolean = false;
  configSettings: ConfigSetting[] = [];
}

class ConfigSetting {
  name: string = "";
  options: {
    isSelected: boolean;
    name: string;
    value: number;
  }[] = []
}

@Component({
  selector: 'app-party-page',
  templateUrl: './party-page.component.html',
  styleUrls: ['./party-page.component.scss']
})
export class PartyPageComponent implements OnInit {
  
  partyData: PartyData = new PartyData();
  isPartyReady: boolean = false;
  selectedMatchProps: Map<string, number> = new Map();
  partyTypeString: string = "";
  partyId: string = "";

  constructor(private signalrService: SignalrService, private matchmakingService: MatchmakingService,
              private router: Router, private loadingScreenService: LoadingScreenService,
              private remoteConfig: AngularFireRemoteConfig) {
    this.loadRemoteConfigTexts();

  }

  partyTypeText: string = "";
  codeLabelText: string = "";
  copyBtnText: string = "";
  timerTypeLabel: string = "";
  timerTypeStandardText: string = "";
  timerTypeChessText: string = "";
  timerDifficultyLabel: string = "";
  playersLabel: string = "";
  leaveBtnText: string = "";
  startGameBtnText: string = "";

  private loadRemoteConfigTexts() {
    //AppConfig.isRemoteConfigFetched = false;
    this.remoteConfig.fetchAndActivate().then(hasActivatedTheFetch => {
      this.remoteConfig.getAll().then(all => {
        //AppConfig.isRemoteConfigFetched = true;
        switch(this.partyData.partyType) {
          case(PartyType.Duo): {
            this.partyTypeText = all["PartyTypeDuoText"].asString()!;
            break;
          }
          case(PartyType.Friendly): {
            this.partyTypeText = all["PartyTypeFriendlyText"].asString()!;
            break;
          }
        }

        this.codeLabelText = all["CodeLabelText"].asString()!;
        this.copyBtnText = all["CopyBtnText"].asString()!;
        this.timerTypeLabel = all["TimerTypeLabel"].asString()!;
        this.timerTypeStandardText = all["TimerTypeStandardText"].asString()!;
        this.timerTypeChessText = all["TimerTypeChessText"].asString()!;
        this.timerDifficultyLabel = all["TimerDifficultyLabel"].asString()!;
        this.playersLabel = all["PlayersLabel"].asString()!;
        this.startGameBtnText = all["StartGameBtnText"].asString()!;
        this.leaveBtnText = all["LeaveBtnText"].asString()!;
      })
    })
  }

  getConfigSettingName(name: string) {
    if(name == "TimerType") {
      return this.timerTypeLabel;
    } else if(name == "TimerDifficulty") {
      return this.timerDifficultyLabel;
    } else {
      return name;
    }
  }

  getOptionName(name: string) {
    if(name == "Standard") {
      return this.timerTypeStandardText;
    } else if (name == "Chess") {
      return this.timerTypeChessText;
    } else {
      return name;
    }
  }

  ngOnInit(): void {
    this.loadingScreenService.showLoadingScreen();
    this.signalrService.startConnection();

    const url = window.location.href;
    const params = url.split("/");
    let id = params[params.length - 1];
    this.partyId = id;

    if(this.signalrService.hubConnection
      && this.signalrService.hubConnection.state == HubConnectionState.Connected) {
        this.attachListeners();
        this.signalrService.loadParty(id);
    } else {
        //TODO: Handle slow connection/loading -> showing loading screen
        this.signalrService.hubConnectionStartPromise?.then( () => {
          this.attachListeners();
          this.signalrService.loadParty(id);
        })
    }
  }

  attachListeners() : void {
    //TODO: Handle error, especially on LoadParty error

    this.signalrService.hubConnection?.on("ReceivePartyData", data => {
      this.parsePartyData(data);
      this.loadingScreenService.stopShowingLoadingScreen();
    })

    this.signalrService.hubConnection?.on("EnablePartyStart", () => {
      this.isPartyReady = true;
    })

    this.signalrService.hubConnection?.on("DisablePartyStart", () => {
      this.isPartyReady = true;
    })

    this.signalrService.hubConnection?.on("NewPlayerJoinedParty", data => {
      this.partyData.members.push(data);
    })

    this.signalrService.hubConnection?.on("PartyLeft", data => {
      this.router.navigateByUrl('/');
    })

    this.signalrService.hubConnection?.on("PlayerHasLeftParty", data => {
      this.partyData.members = data.remainingMembers;
      this.partyData.isOwner = data.isOwner;
      this.partyData.owner = data.owner;
      this.isPartyReady = data.isPartyReady;

      alert(data.leaverUserName + " has left the party");
    })

    this.signalrService.hubConnection?.on("UpdateFriendPartyConfigSettings", data => {
      this.partyData.configSettings = data;
    })

    this.signalrService.hubConnection?.on("PartyMemberConnectionIdChanged", () => {
      this.router.navigateByUrl('/');
    })
  }

  parsePartyData(rawServerData: any) {
    this.partyData.invitationCode = rawServerData.invitationCode;
    this.partyData.isOwner = rawServerData.isOwner;
    this.partyData.owner = rawServerData.owner;
    this.partyData.members = rawServerData.members;
    this.partyData.partyType = rawServerData.partyType;
    this.partyData.configSettings = rawServerData.configSettings;
    this.isPartyReady = rawServerData.isPartyReady;
    this.partyTypeString = PartyType[this.partyData.partyType];
  }

  selectConfigSetting(configSetting: ConfigSetting, option: any) {
    configSetting.options.forEach(x => x.isSelected = false);
    option.isSelected = true;
    if(this.partyData.partyType == PartyType.Friendly) {
      let input: Map<string, number> = new Map();

      this.partyData.configSettings.forEach(x => {
        let value = x.options.filter(x => x.isSelected)[0].value;
        input.set(x.name, value);
      })
      
      //TODO: Change this
      let obj = Array.from(input).reduce((obj, [key, value]) => (
        Object.assign(obj, { [key]: value })
      ), {});

      this.signalrService.setFriendPartyConfiguration(obj, this.partyId);
    }
  }

  isPartyOwner(member: string) {
    return this.partyData.owner == member;
  }

  startGame() {
    this.matchmakingService.startGameFromParty(this.partyId);
  }

  leaveParty() {
    this.signalrService.leaveParty(this.partyId);
  }

  isConfigSettingOptionSelected(option: any) : boolean {
    return option.isSelected;
  }

  isConfigEnabled() : boolean {
    return (this.partyData.isOwner && (this.partyData.partyType == PartyType.Friendly));
  }
}
