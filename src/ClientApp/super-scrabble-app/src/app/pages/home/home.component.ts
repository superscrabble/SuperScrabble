import { Component, OnInit } from '@angular/core';
import { Utilities } from 'src/app/common/utilities';
import { SignalrService } from 'src/app/services/signalr.service';
import { HubConnectionState } from '@microsoft/signalr';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { JoinPartyWithCodeDialogComponent } from '../../dialogs/join-party-with-code-dialog/join-party-with-code-dialog.component';
import { MatchmakingService } from 'src/app/services/matchmaking.service';
import { PartyType } from 'src/app/models/enums/party-type';
import { GameMode } from 'src/app/models/enums/game-mode';
import { LoadingScreenService } from 'src/app/services/loading-screen.service';
import { AngularFireRemoteConfig } from '@angular/fire/compat/remote-config';

class GameModeButton {
  text: string = "";
  action: Function = () => {};
}

class GameModeInfo {
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

  messages: string[] = new Array();

  isStartGameButtonEnabled: boolean = false;
  isSearchingForGame: boolean = false;
  currentGameGroupName: string | null = null;
  gameModes: GameModeInfo[] = [];

  invitationCode: string = "";
  receivedInvitationCode: string = "";
  invitationCodeFieldVisible: boolean = false;

  welcomeMessageText: string = "";
  chooseGamemodeMessageText: string = "";

  constructor(private signalrService: SignalrService, private utilities: Utilities,
              private router: Router, private dialog: MatDialog, private matchmakingService: MatchmakingService,
              private loadingScreenService: LoadingScreenService,
              private remoteConfig: AngularFireRemoteConfig) {
      /*this.gameModes = [
        {
          name: "Дуел",
          description: "Играй стандартен Скрабъл срещу друг играч!",
          buttons: [
            {
              text: "Играй",
              action: () => {
                this.joinRoom(GameMode.Duel);
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
                this.matchmakingService.joinRandomDuoGame();
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
              action: () => {
                this.matchmakingService.createParty(PartyType.Duo);
      */
              
      this.loadRemoteConfigTexts();
  }

  private loadRemoteConfigTexts() {
    //AppConfig.isRemoteConfigFetched = false;
    this.remoteConfig.fetchAndActivate().then(hasActivatedTheFetch => {
      this.remoteConfig.getAll().then(all => {
        //AppConfig.isRemoteConfigFetched = true;
        this.welcomeMessageText = all["WelcomeMessage"].asString()!;
        this.chooseGamemodeMessageText = all["ChooseGamemodeMessage"].asString()!;
        this.gameModes = [
          {
            name: all["DuelGamemodeLabel"].asString()!,
            description: all["DuelGamemodeDesc"].asString()!,
            buttons: [
              {
                text: all["PlayBtnText"].asString()!,
                action: () => {
                  this.joinRoom(GameMode.Duel);
                }
              }
            ]
          },
          {
            name: all["DuoGamemodeLabel"].asString()!,
            description: all["DuoGamemodeDesc"].asString()!,
            buttons: [
              {
                text: all["PlayWithRandomBtnText"].asString()!,
                action: () => {
                  this.matchmakingService.joinRandomDuoGame();
                }
              },
              {
                text: all["JoinWithCodeBtnText"].asString()!,
                action: () => {
                  this.dialog.open(JoinPartyWithCodeDialogComponent);
                }
              },
              {
                text: all["CreatePartyBtnText"].asString()!,
                action: () => {
                  this.matchmakingService.createParty(PartyType.Duo);
                }
              }
            ]
          },
          {
            name: all["FriendlyGamemodeLabel"].asString()!,
            description: all["FriendlyGamemodeDesc"].asString()!,
            buttons: [
              {
                text: all["JoinWithCodeBtnText"].asString()!,
                action: () => {
                  this.dialog.open(JoinPartyWithCodeDialogComponent);
                }
              },
              {
                text: all["CreatePartyBtnText"].asString()!,
                action: () => {
                  this.matchmakingService.createParty(PartyType.Friendly);
                }
              }
            ]
          },
          {
            name: all["ChessScrabbleGamemodeLabel"].asString()!,
            description: all["ChessScrabbleGamemodeDesc"].asString()!,
            buttons: [
              {
                text: all["PlayBtnText"].asString()!,
                action: () => {
                  this.joinRoom(GameMode.ChessScrabble);
                }
              }
            ]
          },
          {
            name: all["ClassicGamemodeLabel"].asString()!,
            description: all["ClassicGamemodeDesc"].asString()!,
            buttons: [
              {
                text: all["PlayBtnText"].asString()!,
                action: () => {
                  this.joinRoom(GameMode.Classic);
                }
              }
            ]
          }
        ]

        for(let i = 0; i < this.gameModes.length % 3; i++) {
          this.gameModes.push({
            name: "FillingCard",
            description: "",
            buttons: [
              {
                text: all["PlayBtnText"].asString()!,
                action: () => {
                  
                }
              }
            ]
          })
        }
      })
    })
  }

  getClassNameByGameModeName(name: string) {
    if(name == "FillingCard") {
      return "invisible-card";
    }
    return "";
  }

  ngOnInit(): void {
    this.loadingScreenService.showLoadingScreen();
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
    this.loadingScreenService.stopShowingLoadingScreen();
  }

  attachListeners() : void {
    this.signalrService.hubConnection?.on("UserAlreadyInsideGame", data => {
      console.log("UserAlreadyInsideGame");
      this.currentGameGroupName = data;
    });

    this.signalrService.hubConnection?.on("Error", data => {
      alert(data);
    });

    this.signalrService.hubConnection?.on("PartyCreated", data => {
      console.log("Party Created")
      this.router.navigate(["party/" + data]);
    });

    this.signalrService.hubConnection?.on("PlayerJoined", data => {
      console.log("Party Joined")
      this.router.navigate(["party/" + data]);
    });

    this.signalrService.hubConnection?.on("ReceiveFriendlyGameCode", code => {
      this.receivedInvitationCode = code;
    });
    
    this.signalrService.hubConnection?.on("EnableFriendlyGameStart", () => {
      this.isStartGameButtonEnabled = true;
    });
  }

  joinRoom(gameMode: GameMode) {
    this.matchmakingService.joinRoom(gameMode);
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

  flip(id: string) {
    if(document.querySelector("#" + id)?.classList.contains('flipped')) {
      document.querySelector("#" + id)?.classList.remove('flipped')
      return;
    }
    document.querySelector("#" + id)?.classList.add('flipped')
  }
}
