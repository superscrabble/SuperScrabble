import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import * as signalR from "@microsoft/signalr";
import { Utilities } from 'src/app/common/utilities';
import { environment } from 'src/environments/environment';
import { AppConfig } from '../app-config';
import { GameMode } from '../models/enums/game-mode';
import { PartyType } from '../models/enums/party-type';
//import { MatchProps } from '../models/game-configuaration/match-props';
import { Tile } from '../models/tile';
import { ErrorHandler } from './error-handler'
import { LanguageService } from './language.service';
import { LoadingScreenService } from './loading-screen.service';
import { ActiveToast, ToastrService } from 'ngx-toastr';

class CustomLogger implements signalR.ILogger {
  constructor(private errorHandler: ErrorHandler) {}

  log(logLevel: signalR.LogLevel, message: string): void {
    let statusCode: number;
    
    //TODO: catch when server is down

    let statusCodeSplit = message.split("Status code");
    if(statusCodeSplit.length > 1) {
      statusCode = parseInt(statusCodeSplit[1].replace("'", ""))
      console.log(statusCode);
      this.errorHandler.handle(statusCode);
    }
  }
}

@Injectable({
  providedIn: 'root'
})
export class SignalrService {

  constructor(private utilities: Utilities, private router: Router, private errorHandler: ErrorHandler,
              private loadingScreenService: LoadingScreenService, private dialog: MatDialog,
              private toastr: ToastrService, private languageService: LanguageService) { }

  //FIXME: change the access modifier
  public hubConnection?: signalR.HubConnection;
  public hubConnectionStartPromise: Promise<void> | null = null;

  private reconnectingToast: ActiveToast<any> | null = null;

  public startConnection = () => {
    if(this.hubConnection?.state == signalR.HubConnectionState.Connected) {
      return;
    }

    this.hubConnection = new signalR.HubConnectionBuilder()
                            .withUrl(environment.serverUrl + '/gamehub',
                            { accessTokenFactory: () => this.utilities.getAccessToken()})
                            .withAutomaticReconnect([0, 2000, 5000, 10000, 15000, 30000])
                            .configureLogging(signalR.LogLevel.Critical)
                            .configureLogging(new CustomLogger(this.errorHandler))
                            .build();

    this.addReconnectionListeners(this.hubConnection);

    this.hubConnectionStartPromise = this.hubConnection.start().then(() => {
      this.loadingScreenService.stopShowingLoadingScreen();
    }, (err) => {
      console.log('ERROR IN CATCH: ' + err);      
    });

    this.hubConnectionStartPromise.catch(err => {
      console.log('Error while starting connection: ' + err.toString())
    });
    //TODO: assure that everything about connection is working
    /*console.log("Before start connection")
    this.hubConnection
      .start()
      .then(() => {
        console.log('Connection started')
      })
      .catch(err => console.log('Error while starting connection: ' + err))*/
    console.log("After start connection")
  }

  private addReconnectionListeners(hubConnection: signalR.HubConnection) {
    hubConnection.onreconnecting(() => {
      this.reconnectingToast = this.toastr.warning(
        this.languageService.getLocalText("ReconnectingText"), '',
        { disableTimeOut: true, closeButton: false });
    });

    hubConnection.onreconnected(() => {
      this.clearReconnectingToast();
      this.toastr.success(this.languageService.getLocalText("ReconnectedText"));
      this.rejoinAfterReconnect();
    });

    // Fires when the automatic retries are exhausted (or the connection
    // closed without reconnect); at this point only a page reload helps.
    hubConnection.onclose(() => {
      this.clearReconnectingToast();
      this.loadingScreenService.stopShowingLoadingScreen();
      this.toastr.error(
        this.languageService.getLocalText("ConnectionLostText"), '',
        { disableTimeOut: true });
    });
  }

  private clearReconnectingToast() {
    if (this.reconnectingToast != null) {
      this.toastr.clear(this.reconnectingToast.toastId);
      this.reconnectingToast = null;
    }
  }

  // Reconnecting creates a new connection id, so the server must re-associate
  // this player/member with it. LoadGame/LoadParty do that and push fresh state.
  private rejoinAfterReconnect() {
    const url = this.router.url;

    if (url.startsWith("/games/")) {
      const gameId = url.split("/")[2];
      if (gameId) {
        this.loadGame(gameId);
      }
    } else if (url.startsWith("/party/")) {
      const partyId = url.split("/")[2];
      if (partyId) {
        this.loadParty(partyId);
      }
    }
  }

  //TODO: find a way to unsubscribe a listener
  public addStartGameListeners = () => {
    this.hubConnection?.on('StartGame', (data) => {
      console.log("GAME START")
      this.router.navigateByUrl("/games/" + data);
      this.dialog.closeAll();
    });

    this.hubConnection?.on('WaitingForMorePlayers', (data) =>   {
      console.log("Waiting for " + data + " more players!");
    });
  }

  //TODO: find a way to unsubscribe a listener
  public addInGameListeners = () => {
    this.hubConnection?.on("UpdateGameState", (data) => {
      console.log(data);
    });
  }

  public stopSearching() {
    this.hubConnection?.invoke("StopSearching");
  }

  public joinRoom(gameMode: GameMode) {
    console.log(this.hubConnection?.state)
    console.log("Join Room")
    if(this.hubConnection?.state == signalR.HubConnectionState.Connected) {
      this.hubConnection?.invoke("JoinRoom", gameMode)
    }
  }

  public leaveQueue() {
    console.log("Leave Room")
    if(this.hubConnection?.state == signalR.HubConnectionState.Connected) {
      this.hubConnection?.invoke("LeaveQueue");
    }
  }

  public loadGame(groupName: string) {
    if(this.hubConnection?.state == signalR.HubConnectionState.Connected) {
      this.hubConnection?.invoke("LoadGame", groupName);
    }
  }

  public writeWord(cells: any[]) {
    if(cells.length > 0) {
      let input = {
        positionsByTiles: cells
      }
      this.hubConnection?.invoke("WriteWord", input);
    }
  }

  public exchangeTiles(tiles: Tile[]) {
    if(tiles.length > 0) {
      let input = {
        tilesToExchange: tiles
      }
      this.hubConnection?.invoke("ExchangeTiles", input);
    }
  }

  public skipTurn() {
    this.hubConnection?.invoke("SkipTurn");
  }

  public getAllWildcardOptions() {
    this.hubConnection?.invoke("GetAllWildcardOptions");
  }

  public leaveGame() {
    this.hubConnection?.invoke("LeaveGame");
  }

  public loadParty(id: string) {
    this.hubConnection?.invoke("LoadParty", id);
  }

  public createParty(type: PartyType) {
    this.hubConnection?.invoke("CreateParty", type);
  }

  public joinParty(code: string) {
    this.hubConnection?.invoke("JoinParty", code);
  }

  public joinRandomDuoGame() {
    this.hubConnection?.invoke("JoinRandomDuo");
  }

  public leaveParty(partyId: string) {
    console.log("Party id" + partyId)
    this.hubConnection?.invoke("LeaveParty", partyId);
  }

  public startGameFromParty(partyId: string) {
    this.hubConnection?.invoke("StartGameFromParty", partyId);
  }

  public setFriendPartyConfiguration(config: any, partyId: string) {
    //TODO: set config type
    if(this.hubConnection?.state == signalR.HubConnectionState.Connected) {
      this.hubConnection?.invoke("SetFriendPartyConfiguration", config, partyId);
    } else {
      console.log("Disconnected");
    }

  }
}
