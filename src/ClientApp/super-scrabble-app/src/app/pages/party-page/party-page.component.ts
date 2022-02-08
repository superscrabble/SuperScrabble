import { Component, OnInit } from '@angular/core';
import { TimerType } from 'src/app/common/enums/timer-type';
import { GameConfig } from 'src/app/models/game-configuaration/game-config';
import { SignalrService } from 'src/app/services/signalr.service';

class PartyData {
  invitationCode: string = "";
  gameModeName: string = "";
  gameConfig?: GameConfig;
  owner: string = "";
  members: string[] = [];
}

class MatchProp {
  name: string = "";
  values: {
    key: string;
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
  isOwner: boolean = false;
  matchProps: MatchProp[] = [];
  selectedMatchProps: Map<string, number> = new Map();

  constructor(private signalrService: SignalrService) {
    this.partyData.invitationCode = "DSDS121"
    this.partyData.members = ["Denis", "Gosho", "Misho", "Pesho"]
    //this.partyData.members = ["Denis"]
    this.partyData.owner = "Denis";
    this.isOwner = true;

    this.matchProps = [
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
    ]
  }

  ngOnInit(): void {
    this.signalrService.startConnection();

    const url = window.location.href;
    const params = url.split("/");
    let id = params[params.length - 1];

    this.signalrService.loadParty(id);

    this.signalrService.hubConnection?.on("ReceivePartyData", data => {
    })
  }

  selectMatchProp(key: string, value: number) {
    this.selectedMatchProps.set(key, value);
  }

  isPartyOwner(member: string) {
    return this.partyData.owner == member;
  }

  isPartyReady() : boolean {
    if(this.partyData.members.length >= 2) {
      return true;
    }
    return false;
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
