import { Component, OnInit, Input, Pipe, PipeTransform } from '@angular/core';
import { Player } from 'src/app/models/player';
import { Team } from 'src/app/models/team';

@Pipe({
  name: "formatTime"
})
export class FormatTimePipe implements PipeTransform {
  transform(value: number): string {
    const minutes: number = Math.floor(value / 60);
    return (
        ("00" + minutes).slice(-2) +
        ":" +
        ("00" + Math.floor(value - minutes * 60)).slice(-2)
    );
  }
}

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

  getClassByRowIndex(i: number) {
    if(i % 2 != 0) {
      return "odd-row";
    }
    return "even-row";
  }

  doAllPlayersHaveTimer() : boolean {
    return true;
  }
}
