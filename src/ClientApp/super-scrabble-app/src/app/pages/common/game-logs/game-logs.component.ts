import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AngularFireRemoteConfig } from '@angular/fire/compat/remote-config';
import { Action } from 'src/app/models/action';

@Component({
  selector: 'app-game-logs',
  templateUrl: './game-logs.component.html',
  styleUrls: ['./game-logs.component.css']
})
export class GameLogsComponent implements OnInit {

  @Output('showWordMeaningOf') _showWordMeaningOf: EventEmitter<any> = new EventEmitter();
  @Input() gameLogs: Action[] = [];

  gameLogsLabel: string = "";

  constructor(private remoteConfig: AngularFireRemoteConfig) {
    this.loadRemoteConfigTexts();
  }

  private loadRemoteConfigTexts() {
    //AppConfig.isRemoteConfigFetched = false;
    this.remoteConfig.fetchAndActivate().then(hasActivatedTheFetch => {
      this.remoteConfig.getAll().then(all => {
        //AppConfig.isRemoteConfigFetched = true;
        this.gameLogsLabel = all["GameLogsLabel"].asString()!;
      })
    })
  }

  ngOnInit(): void {
  }

  showWordMeaningOf(value: string) {
    this._showWordMeaningOf.emit(value);
  }

}
