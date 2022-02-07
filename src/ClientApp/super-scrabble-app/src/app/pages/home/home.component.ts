import { Component, OnInit } from '@angular/core';
import { Utilities } from 'src/app/common/utilities';
import { SignalrService } from 'src/app/services/signalr.service';
import { HubConnectionState } from '@microsoft/signalr';
import { Router } from '@angular/router';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  messages: string[] = new Array();

  isStartGameButtonEnabled: boolean = false;
  isSearchingForGame: boolean = false;
  currentGameGroupName: string | null = null;

  invitationCode: string = "";
  receivedInvitationCode: string = "";
  invitationCodeFieldVisible: boolean = false;

  constructor(private signalrService: SignalrService, private utilities: Utilities,
              private router: Router) { }

  ngOnInit(): void {
    this.signalrService.startConnection();
    this.signalrService.addStartGameListeners();

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
    this.signalrService.hubConnection?.on("UserAlreadyInsideGame", data => {
      this.currentGameGroupName = data;
    });

    this.signalrService.hubConnection?.on("Error", data => {
      alert(data);
    });

    this.signalrService.hubConnection?.on("ReceiveFriendlyGameCode", code => {
      this.receivedInvitationCode = code;
    });
    
    this.signalrService.hubConnection?.on("EnableFriendlyGameStart", () => {
      this.isStartGameButtonEnabled = true;
    });
    this.signalrService.hubConnection?.on("PlayerJoinedLobby", joinedUserName => {
      this.messages.push(`${joinedUserName} се присиедини към лобито`);
    });
  }

  joinRoom() {
    this.signalrService.joinRoom();
    this.isSearchingForGame = true;
  }

  isReceivedInvitationCodeVisible() {
    return this.receivedInvitationCode != "";
  }

  leaveQueue() {
    this.signalrService.leaveQueue();
    this.isSearchingForGame = false;
  }

  hasAccessToken() {
    return this.utilities.hasAccessToken();
  }

  redirectToCurrentGame() {
    this.router.navigate(["games/" + this.currentGameGroupName]);
  }

  createFriendlyGame() {
    const input = {
      timerType: 1,
      timerDifficulty: 2
    };
    this.signalrService.hubConnection?.invoke("CreateFriendlyGame", input);
  }

  joinFriendlyGame() {
    this.signalrService.hubConnection?.invoke("JoinFriendlyGame", this.invitationCode);
  }

  startFriendlyGame() {
    this.signalrService.hubConnection?.invoke("StartFriendlyGame", this.receivedInvitationCode);
  }

  showInvitationCodeField() {
    if (this.invitationCode.length > 0) {
      this.joinFriendlyGame();
    }
    this.invitationCodeFieldVisible = true;
  }
}
