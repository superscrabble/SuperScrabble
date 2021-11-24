import { Injectable } from '@angular/core';
import * as signalR from "@microsoft/signalr";
import { Utilities } from 'src/app/common/utilities';

@Injectable({
  providedIn: 'root'
})
export class SignalrService {

  constructor(private utilities: Utilities) { }

  private hubConnection?: signalR.HubConnection;

  public startConnection = () => {
    

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

  public addTransferGameHubDataListener = () => {
    this.hubConnection?.on('StartGame', () => {
      console.log("Game started");
    });

    this.hubConnection?.on('WaitingForMorePlayers', (data) =>   {
      console.log("Waiting for " + data + " more players!");
    });

    this.hubConnection?.on("UpdateGameState", (data) => {
      console.log(data);
    });
  }

  public joinRoom() {
    console.log(this.hubConnection?.state)
    console.log("Join Room")
    this.hubConnection?.invoke("JoinRoom");
  }

  public leaveQueue() {
    console.log("Leave Room")
    this.hubConnection?.invoke("LeaveQueue");
  }
}
