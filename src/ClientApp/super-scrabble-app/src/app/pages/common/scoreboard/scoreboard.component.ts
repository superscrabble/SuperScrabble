import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-scoreboard',
  templateUrl: './scoreboard.component.html',
  styleUrls: ['./scoreboard.component.css']
})
export class ScoreboardComponent implements OnInit {
  @Input() teams: any[] = [];
  @Input() currentUserName: string = "";
  @Input() userNamesOfPlayersWhoHaveLeftTheGame: string[] = [];

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
