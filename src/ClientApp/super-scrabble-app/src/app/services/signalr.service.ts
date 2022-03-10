import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import * as signalR from "@microsoft/signalr";
import { Utilities } from 'src/app/common/utilities';
import { AppConfig } from '../app-config';
import { GameMode } from '../common/enums/game-mode';
import { PartyType } from '../common/enums/party-type';
import { MatchProps } from '../models/game-configuaration/match-props';
import { Tile } from '../models/tile';
import { ErrorHandler } from './error-handler'
import { LoadingScreenService } from './loading-screen.service';

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

  constructor(private utilities: Utilities, private router: Router, private errorHandler: ErrorHandler, private loadingScreenService: LoadingScreenService) { }

  //FIXME: change the access modifier
  public hubConnection?: signalR.HubConnection;
  public hubConnectionStartPromise: Promise<void> | null = null;

  public startConnection = () => {
    if(this.hubConnection?.state == signalR.HubConnectionState.Connected) {
      return;
    }

    this.hubConnection = new signalR.HubConnectionBuilder()
                            .withUrl(AppConfig.ServerUrl + AppConfig.ServerPort + '/gamehub',
                            { accessTokenFactory: () => this.utilities.getAccessToken()})
                            .configureLogging(signalR.LogLevel.Critical)
                            .configureLogging(new CustomLogger(this.errorHandler))
                            .build();

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

  //TODO: find a way to unsubscribe a listener
  public addStartGameListeners = () => {
    this.hubConnection?.on('StartGame', (data) => {
      this.router.navigateByUrl("/games/" + data);
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

  public joinRoom(gameMode: GameMode) {
    console.log(this.hubConnection?.state)
    console.log("Join Room")
    if(this.hubConnection?.state == signalR.HubConnectionState.Connected) {
      this.hubConnection?.invoke("JoinRoom", gameMode)
    }
  }

  public joinRoomWithProps(props: MatchProps) {
    if(this.hubConnection?.state == signalR.HubConnectionState.Connected) {
      console.log("Join room with props")
      console.log(props)
      this.hubConnection?.invoke("JoinRoom", props);
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
