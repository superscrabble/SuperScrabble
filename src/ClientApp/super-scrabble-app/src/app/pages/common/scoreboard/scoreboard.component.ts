import { Component, OnInit, Input } from '@angular/core';
import { Player } from 'src/app/models/player';
import { Team } from 'src/app/models/team';

@Component({
  selector: 'app-scoreboard',
  templateUrl: './scoreboard.component.html',
  styleUrls: ['./scoreboard.component.css']
})
export class ScoreboardComponent implements OnInit {
  @Input() teams: Team[] = [];
  @Input() currentUserName: string = "";
  @Input() userNamesOfPlayersWhoHaveLeftTheGame: string[] = [];
  @Input() isDuoGame: boolean = true;

  constructor() { }

  ngOnInit(): void {
  }

  modifyCurrentUserName(playerName: string) {
    let result = playerName;
    if(playerName == this.currentUserName) {
      result += " (аз)"; 
      return result;
    } 
    
    if(this.userNamesOfPlayersWhoHaveLeftTheGame.find(x => x == playerName)) {
        result += " (напуснал)";
        return result;
    }

    return result;
}

}
