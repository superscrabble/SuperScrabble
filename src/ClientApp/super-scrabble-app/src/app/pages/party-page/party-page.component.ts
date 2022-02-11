import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { HubConnectionState } from '@microsoft/signalr';
import { PartyType } from 'src/app/common/enums/party-type';
import { TimerType } from 'src/app/common/enums/timer-type';
import { GameConfig } from 'src/app/models/game-configuaration/game-config';
import { MatchmakingService } from 'src/app/services/matchmaking.service';
import { SignalrService } from 'src/app/services/signalr.service';

class PartyData {
  invitationCode: string = "";
  partyType: PartyType = PartyType.Duo;
  gameConfig?: GameConfig;
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
  //matchProps: ConfigSetting[] = [];
  selectedMatchProps: Map<string, number> = new Map();
  partyTypeString: string = "";
  partyId: string = "";

  constructor(private signalrService: SignalrService, private matchmakingService: MatchmakingService,
              private router: Router) {
    this.partyData.invitationCode = "DSDS121"
    this.partyData.members = ["Denis", "Gosho", "Misho", "Pesho"]
    //this.partyData.members = ["Denis"]
    this.partyData.owner = "Denis";

    /*this.matchProps = [
      {
        name: "TimerType",
        values: [
          {
            key: "Standard",
            value: 1
          },
          {
            key: "Chess",
            value: 2
          }
        ]
      },
      {
        name: "TimerDifficulty",
        values: [
          {
            key: "Slow",
            value: 1
          },
          {
            key: "Standard",
            value: 2
          },
          {
            key: "Fast",
            value: 3
          }
        ]
      },
      {
        name: "BoardType",
        values: [
          {
            key: "Standard",
            value: 1
          },
          {
            key: "Circle",
            value: 2
          }
        ]
      }
    ]*/
  }

  ngOnInit(): void {
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
      console.log("Receive Party DatA")
      console.log(data);
      this.parsePartyData(data);
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
      console.log("UpdateFriendPartyConfigSettings")
      console.log(data);
      this.partyData.configSettings = data;
    })
  }

  parsePartyData(rawServerData: any) {
    this.partyData.invitationCode = rawServerData.invitationCode;
    this.partyData.isOwner = rawServerData.isOwner;
    this.partyData.owner = rawServerData.owner;
    this.partyData.members = rawServerData.members;
    this.partyData.partyType = rawServerData.partyType;
    this.partyData.configSettings = rawServerData.configSettings;

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
    console.log(this.selectedMatchProps);
    this.matchmakingService.StartGameFromParty(this.partyId);
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
