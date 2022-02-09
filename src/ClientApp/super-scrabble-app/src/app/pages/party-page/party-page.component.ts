import { Component, OnInit } from '@angular/core';
import { HubConnectionState } from '@microsoft/signalr';
import { PartyType } from 'src/app/common/enums/party-type';
import { TimerType } from 'src/app/common/enums/timer-type';
import { GameConfig } from 'src/app/models/game-configuaration/game-config';
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

  constructor(private signalrService: SignalrService) {
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


    if(this.signalrService.hubConnection
      && this.signalrService.hubConnection.state == HubConnectionState.Connected) {
        this.signalrService.loadParty(id);
        this.attachListeners();
    } else {
        //TODO: Handle slow connection/loading -> showing loading screen
        this.signalrService.hubConnectionStartPromise?.then( () => {
          this.signalrService.loadParty(id);
          this.attachListeners();
        })
    }
  }

  attachListeners() : void {
    this.signalrService.hubConnection?.on("ReceivePartyData", data => {
      console.log("Receive Party DatA")
      console.log(data);
      this.parsePartyData(data);
    })

    this.signalrService.hubConnection?.on("EnableFriendlyGameStart", () => {
      this.isPartyReady = true;
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

  selectMatchProp(key: string, value: number) {
    this.selectedMatchProps.set(key, value);
  }

  isPartyOwner(member: string) {
    return this.partyData.owner == member;
  }

  startGame() {
    console.log(this.selectedMatchProps);
  }

  isMatchPropOptionSelected(name: string, value: number) : boolean {
    if(this.selectedMatchProps.has(name)) {
      return this.selectedMatchProps.get(name) == value;
    }
    return false;
  }
}
