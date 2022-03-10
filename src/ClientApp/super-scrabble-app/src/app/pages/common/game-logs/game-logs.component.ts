import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AngularFireRemoteConfig } from '@angular/fire/compat/remote-config';
import { Log } from 'src/app/common/enums/log';
import { LogStatus } from 'src/app/common/enums/log-status';
import { Action } from 'src/app/models/action';

@Component({
  selector: 'app-game-logs',
  templateUrl: './game-logs.component.html',
  styleUrls: ['./game-logs.component.css']
})
export class GameLogsComponent implements OnInit {

  @Output('showWordMeaningOf') _showWordMeaningOf: EventEmitter<any> = new EventEmitter();
  @Input() gameLogs: Log[] = [];

  gameLogsLabel: string = "";
  gameLogsWriteWordText: string = "";
  gameLogsSkipTurnText: string = "";
  gameLogsLeaveGameText: string = "";
  gameLogsChangeTilesText: string = "";
  gameLogsNoLogsText: string = "";

  constructor(private remoteConfig: AngularFireRemoteConfig) {
    this.loadRemoteConfigTexts();
  }

  private loadRemoteConfigTexts() {
    //AppConfig.isRemoteConfigFetched = false;
    this.remoteConfig.fetchAndActivate().then(hasActivatedTheFetch => {
      this.remoteConfig.getAll().then(all => {
        //AppConfig.isRemoteConfigFetched = true;
        this.gameLogsLabel = all["GameLogsLabel"].asString()!;
        this.gameLogsWriteWordText = all["GameLogsWriteWordText"].asString()!;
        this.gameLogsSkipTurnText = all["GameLogsSkipTurnText"].asString()!;
        this.gameLogsLeaveGameText = all["GameLogsLeaveGameText"].asString()!;
        this.gameLogsChangeTilesText = all["GameLogsChangeTilesText"].asString()!;
        this.gameLogsNoLogsText = all["GameLogsNoLogsText"].asString()!;
      })
    })
  }

  ngOnInit(): void {
  }

  showWordMeaningOf(value: string) {
    this._showWordMeaningOf.emit(value);
  }

  getTextByGameLog(log: Log) {
    const status = log.status;
    switch(status) {
      case(LogStatus.WriteWord): {

        return this.gameLogsWriteWordText;
      }
      case(LogStatus.Leave): {
        return this.gameLogsLeaveGameText;
      }
      case(LogStatus.Skip): {
        return this.gameLogsSkipTurnText;
      }
      case(LogStatus.ChangeTiles): {
        let changedTilesCountAsText = log.changedTilesCount?.toString()!;
        return this.gameLogsChangeTilesText.replace("{0}", changedTilesCountAsText);
      }
    }
  }

  isChangeTilesStatus(status: LogStatus) {
    return status == LogStatus.ChangeTiles;
  }

  isWriteWordStatus(status: LogStatus) {
    return status == LogStatus.WriteWord;
  }

  isLogListEmpty() {
    return this.gameLogs.length == 0;
  }
}
