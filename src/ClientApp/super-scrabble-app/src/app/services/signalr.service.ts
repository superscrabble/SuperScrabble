import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import * as signalR from "@microsoft/signalr";
import { Utilities } from 'src/app/common/utilities';
import { MatchProps } from '../models/game-configuaration/match-props';
import { Tile } from '../models/tile';

@Injectable({
  providedIn: 'root'
})
export class SignalrService {

  constructor(private utilities: Utilities, private router: Router) { }

  //FIXME: change the access modifier
  public hubConnection?: signalR.HubConnection;
  public hubConnectionStartPromise: Promise<void> | null = null;

  public startConnection = () => {
    if(this.hubConnection?.state == signalR.HubConnectionState.Connected) {
      return;
    }

    this.hubConnection = new signalR.HubConnectionBuilder()
                            .withUrl('https://localhost:7168/gamehub',  { accessTokenFactory: () => this.utilities.getAccessToken()})
                            .build();

    this.hubConnectionStartPromise = this.hubConnection.start();

    this.hubConnectionStartPromise.catch(err => console.log('Error while starting connection: ' + err));
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

  public joinRoom() {
    console.log(this.hubConnection?.state)
    console.log("Join Room")
    if(this.hubConnection?.state == signalR.HubConnectionState.Connected) {
      this.hubConnection?.invoke("JoinRoom");
    }
  }

  public joinRoomWithProps(props: MatchProps) {
    if(this.hubConnection?.state == signalR.HubConnectionState.Connected) {
      console.log("Jpin room with props")
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
}
