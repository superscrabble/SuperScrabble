import { Component, OnInit } from '@angular/core';
import { GameConfig } from 'src/app/models/game-configuaration/game-config';
import { SignalrService } from 'src/app/services/signalr.service';

class PartyData {
  invitationCode: string = "";
  gameModeName: string = "";
  gameConfig?: GameConfig;
  owner: string = "";
  members: string[] = [];
}

@Component({
  selector: 'app-party-page',
  templateUrl: './party-page.component.html',
  styleUrls: ['./party-page.component.scss']
})
export class PartyPageComponent implements OnInit {
  
  partyData: PartyData = new PartyData();
  isOwner: boolean = false;
  
  constructor(private signalrService: SignalrService) {
    this.partyData.invitationCode = "DSDS121"
    this.partyData.members = ["Denis", "Gosho", "Misho", "Pesho"]
    //this.partyData.members = ["Denis"]
    this.partyData.owner = "Denis";
    this.isOwner = true;
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

  isPartyOwner(member: string) {
    return this.partyData.owner == member;
  }

  isPartyReady() : boolean {
    if(this.partyData.members.length >= 2) {
      return true;
    }
    return false;
  }
}
