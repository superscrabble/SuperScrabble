import { Component, OnInit } from '@angular/core';
import { Utilities } from 'src/app/common/utilities';
import { SignalrService } from 'src/app/services/signalr.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  isSearchingForGame: boolean = false;

  constructor(private signalrService: SignalrService, private utilities: Utilities) { }

  ngOnInit(): void {
    this.signalrService.startConnection();
    this.signalrService.addTransferGameHubDataListener();
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
}
