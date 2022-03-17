import { Component, EventEmitter, Inject, OnInit, Output } from '@angular/core';
import { AngularFireRemoteConfig } from '@angular/fire/compat/remote-config';
import { MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';

export interface SettingsDialogData {
  openLeaveGameDialog: Function;
}

@Component({
  selector: 'app-settings-dialog',
  templateUrl: './settings-dialog.component.html',
  styleUrls: ['./settings-dialog.component.css']
})
export class SettingsDialogComponent implements OnInit {

  openLeaveGameDialog: Function;
  
  settingsTitle: string = "";
  leaveGameBtnLabel: string = "";


  //TODO: think how to remove MatDialog so that openLeaveGameDialog will work
  constructor(@Inject(MAT_DIALOG_DATA) public data: SettingsDialogData, private dialog: MatDialog,
              private remoteConfig: AngularFireRemoteConfig) {
    this.openLeaveGameDialog = data.openLeaveGameDialog;
    this.loadRemoteConfigTexts();
  }

  ngOnInit(): void {
  }

  private loadRemoteConfigTexts() {
    //AppConfig.isRemoteConfigFetched = false;
    this.remoteConfig.fetchAndActivate().then(hasActivatedTheFetch => {
      this.remoteConfig.getAll().then(all => {
        //AppConfig.isRemoteConfigFetched = true;
        this.settingsTitle = all["SettingsTitle"].asString()!;
        this.leaveGameBtnLabel = all["LeaveGameBtnLabel"].asString()!;
      })
    })
  }
}
