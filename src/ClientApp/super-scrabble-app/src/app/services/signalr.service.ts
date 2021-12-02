import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import * as signalR from "@microsoft/signalr";
import { Utilities } from 'src/app/common/utilities';

@Injectable({
  providedIn: 'root'
})
export class SignalrService {

  constructor(private utilities: Utilities, private router: Router) { }

  //FIXME: change the access modifier
  public hubConnection?: signalR.HubConnection;

  public startConnection = () => {
    if(this.hubConnection?.state == signalR.HubConnectionState.Connected) {
      return;
    }

    this.hubConnection = new signalR.HubConnectionBuilder()
                            .withUrl('https://localhost:5001/gamehub',  { accessTokenFactory: () => this.utilities.getAccessToken()})
                            .build();

    console.log("Before start connection")
    this.hubConnection
      .start()
      .then(() => {
        console.log('Connection started')
      })
      .catch(err => console.log('Error while starting connection: ' + err))
    console.log("After start connection")
  }

  //TODO: find a way to unsubscribe a listener
  public addStartGameListeners = () => {
    this.hubConnection?.on('StartGame', (data) => {
      console.log("Game started");
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

  public leaveQueue() {
    console.log("Leave Room")
    if(this.hubConnection?.state == signalR.HubConnectionState.Connected) {
      this.hubConnection?.invoke("LeaveQueue");
    }
  }

  public loadGame(groupName: string) {
    if(this.hubConnection?.state == signalR.HubConnectionState.Connected) {
      this.hubConnection?.invoke("LoadGame", groupName)  ;
    }
  }
}
