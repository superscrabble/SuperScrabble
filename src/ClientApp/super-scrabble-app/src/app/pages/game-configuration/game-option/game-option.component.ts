import { Component, Input, OnInit } from '@angular/core';
import { GameOption } from 'src/app/models/game-configuaration/game-option';
import { MatchmakingService } from 'src/app/services/matchmaking.service';

@Component({
  selector: 'app-game-option',
  templateUrl: './game-option.component.html',
  styleUrls: ['./game-option.component.css']
})
export class GameOptionComponent implements OnInit {

  @Input() gameOption: GameOption = new GameOption("", "", 0);
  @Input() isAboutTeamCount: boolean = false;

  constructor(private matchmakingService: MatchmakingService) {
  }

  ngOnInit(): void {
  }

  //TODO: rename this
  getClassIfSelected() {
    if(this.gameOption.isSelected) {
      return "selected-game-option"
    }
    return "game-option"
  }

  isTeamGame() : boolean {
    return this.matchmakingService.isTeamGame();
  }

  openDescriptionDialog(): void {

  }
}
