import { Component, OnInit } from '@angular/core';
import { Utilities } from 'src/app/common/utilities';
import { SignalrService } from 'src/app/services/signalr.service';
import { HubConnectionState } from '@microsoft/signalr';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { JoinPartyWithCodeDialogComponent } from '../game-configuration/dialogs/join-party-with-code-dialog/join-party-with-code-dialog.component';

class GameModeButton {
  text: string = "";
  action: Function = () => {};
}

class GameMode {
  name: string = "";
  description?: string;
  buttons: GameModeButton[] = [];
}

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  isSearchingForGame: boolean = false;
  currentGameGroupName: string | null = null;
  gameModes: GameMode[] = [];

  invitationCode: string = "";

  constructor(private signalrService: SignalrService, private utilities: Utilities,
              private router: Router, private dialog: MatDialog) {
      this.gameModes = [
        {
          name: "Дуел",
          description: "Играй стандартен Скрабъл срещу друг играч!",
          buttons: [
            {
              text: "Играй",
              action: () => {
                
              }
            }
          ]
        },
        {
          name: "Двама на двама",
          description: "Играй в отбор с твой приятел или случаен играч срещу друг отбор!",
          buttons: [
            {
              text: "Играй със случаен играч",
              action: () => {
                
              }
            },
            {
              text: "Влез с код",
              action: () => {
                this.dialog.open(JoinPartyWithCodeDialogComponent);
              }
            },
            {
              text: "Създай отбор",
              action: () => {}
            }
          ]
        },
        {
          name: "Приятелска игра",
          description: "Играй с приятели!",
          buttons: [
            {
              text: "Влез с код",
              action: () => {
                this.dialog.open(JoinPartyWithCodeDialogComponent);
              }
            },
            {
              text: "Създай игра",
              action: () => {}
            }
          ]
        },
        {
          name: "Шах-Скрабъл",
          description: "Играй Скрабъл с таймер, който отмерва времето както в шах, срещу друг играч!",
          buttons: [
            {
              text: "Играй",
              action: () => {}
            }
          ]
        },
        {
          name: "Класически Скрабъл",
          description: "Играй Скрабъл с други трима играчи!",
          buttons: [
            {
              text: "Играй",
              action: () => {}
            }
          ]
        }
      ]
  }

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
      console.error(data);
    });

    this.signalrService.hubConnection?.on("ReceiveFriendlyGameCode", data => {
      console.log(data);
    });
  }

  joinRoom() {
    this.signalrService.joinRoom();
    this.isSearchingForGame = true;
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

  flip(id: string) {
    if(document.querySelector("#" + id)?.classList.contains('flipped')) {
      document.querySelector("#" + id)?.classList.remove('flipped')
      return;
    }
    document.querySelector("#" + id)?.classList.add('flipped')
  }
}
