import { Component, OnInit } from '@angular/core';
import { SignalrService } from 'src/app/services/signalr.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  constructor(private signalrService: SignalrService) { }

  ngOnInit(): void {
    this.signalrService.startConnection();
    this.signalrService.addTransferGameHubDataListener();
  }

  joinRoom() {
    this.signalrService.joinRoom();
  }

}
