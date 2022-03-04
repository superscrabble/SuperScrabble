import { Component, OnInit } from '@angular/core';
import { AngularFireRemoteConfig } from '@angular/fire/compat/remote-config';
import { MatDialogRef } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { HubConnectionState } from '@microsoft/signalr';
import { MatchmakingService } from 'src/app/services/matchmaking.service';
import { SignalrService } from 'src/app/services/signalr.service';

@Component({
  selector: 'app-join-party-with-code-dialog',
  templateUrl: './join-party-with-code-dialog.component.html',
  styleUrls: ['./join-party-with-code-dialog.component.scss']
})
export class JoinPartyWithCodeDialogComponent implements OnInit {

  code: string = "";

  enterCodeText: string = "";
  enterCodeBtnText: string = "";
  denialBtnText: string = "";

  constructor(public dialogRef: MatDialogRef<JoinPartyWithCodeDialogComponent>,
              private matchmakingService: MatchmakingService,
              private signalrService: SignalrService,
              private router: Router, private remoteConfig: AngularFireRemoteConfig) {
    this.loadRemoteConfigTexts();
  }

  private loadRemoteConfigTexts() {
    //AppConfig.isRemoteConfigFetched = false;
    this.remoteConfig.fetchAndActivate().then(hasActivatedTheFetch => {
      this.remoteConfig.getAll().then(all => {
        //AppConfig.isRemoteConfigFetched = true;
        this.enterCodeText = all["EnterCodeText"].asString()!;
        this.enterCodeBtnText = all["EnterCodeBtnText"].asString()!;
        this.denialBtnText = all["DenialBtnText"].asString()!;
      })
    })
  }

  ngOnInit(): void {
    console.log("ConnectING")
    this.signalrService.startConnection();

    if(this.signalrService.hubConnection
      && this.signalrService.hubConnection.state == HubConnectionState.Connected) {
        this.attachListeners();
    } else {
        //TODO: Handle slow connection/loading -> showing loading screen
        this.signalrService.hubConnectionStartPromise?.then( () => {
          this.attachListeners();
        })
    }
  }

  attachListeners() : void {
    this.signalrService.hubConnection?.on("PartyJoined", data => {
      console.log("Party Joined")
      this.router.navigate(["party/" + data]);
    });
  }

  joinParty() {
    this.matchmakingService.joinParty(this.code);
    console.log("Join Party");
    this.dialogRef.close();
  }
}
