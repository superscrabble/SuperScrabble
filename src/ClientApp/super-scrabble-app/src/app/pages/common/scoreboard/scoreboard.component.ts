import { Component, OnInit, Input, Pipe, PipeTransform } from '@angular/core';
import { AngularFireRemoteConfig } from '@angular/fire/compat/remote-config';
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

  scoreboardTimeLabel: string = "";
  scoreboardPlayerLabel: string = "";
  scoreboardPointsLabel: string = "";
  scoreboardPointsAbreviation: string = "";
  scoreboardLeftPlayerText: string = "";
  scoreboardCurrentPlayerText: string = "";

  constructor(private remoteConfig: AngularFireRemoteConfig) {
    this.loadRemoteConfigTexts();
  }

  private loadRemoteConfigTexts() {
    //AppConfig.isRemoteConfigFetched = false;
    this.remoteConfig.fetchAndActivate().then(hasActivatedTheFetch => {
      this.remoteConfig.getAll().then(all => {
        //AppConfig.isRemoteConfigFetched = true;
        this.scoreboardTimeLabel = all["ScoreboardTimeLabel"].asString()!;
        this.scoreboardPlayerLabel = all["ScoreboardPlayerLabel"].asString()!;
        this.scoreboardPointsLabel = all["ScoreboardPointsLabel"].asString()!;
        this.scoreboardPointsAbreviation = all["ScoreboardPointsAbreviation"].asString()!;
        this.scoreboardLeftPlayerText = all["ScoreboardLeftPlayerText"].asString()!;
        this.scoreboardCurrentPlayerText = all["ScoreboardCurrentPlayerText"].asString()!;
      })
    })
}

  ngOnInit(): void {
  }

  modifyCurrentUserName(playerName: string) {
    let result = playerName;
    if(playerName == this.currentUserName) {
      result += " (" + this.scoreboardCurrentPlayerText + ")"; 
      return result;
    } 
    
    if(this.userNamesOfPlayersWhoHaveLeftTheGame.find(x => x == playerName)) {
        result += " (" + this.scoreboardLeftPlayerText + ")";
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
