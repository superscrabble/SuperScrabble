import { Component, OnInit } from '@angular/core';
import { MatchmakingService } from 'src/app/services/matchmaking.service';

@Component({
  selector: 'app-game-configuration-menu',
  templateUrl: './game-configuration-menu.component.html',
  styleUrls: ['./game-configuration-menu.component.css']
})
export class GameConfigurationMenuComponent implements OnInit {

  constructor(private matchmakingService: MatchmakingService) {}

  ngOnInit(): void {
  }

  getCurrentConfig() {
    return this.matchmakingService.getCurrentConfig();
  }

  isBackButtonEnabled() {
    return !this.matchmakingService.isFirstConfig();
  }

  onChosenOption($event: any) {
    let chosenValue = $event.chosenValue;
    this.matchmakingService.chooseOption(chosenValue);
  }

  previousConfig() {
    this.matchmakingService.previousConfig();
  }

  isStartButtonEnabled() {
    return this.matchmakingService.isConfigReady();
  }

  joinRoom() {

  }
}
