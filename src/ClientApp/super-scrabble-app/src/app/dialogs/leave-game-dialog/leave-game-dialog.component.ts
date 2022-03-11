import { Component, Inject, OnInit } from '@angular/core';
import { AngularFireRemoteConfig } from '@angular/fire/compat/remote-config';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ErrorDialogComponent } from '../error-dialog/error-dialog.component';

@Component({
  selector: 'app-leave-game-dialog',
  templateUrl: './leave-game-dialog.component.html',
  styleUrls: ['./leave-game-dialog.component.css']
})
export class LeaveGameDialogComponent implements OnInit {

  leavePopupText: string = "";
  leaveComfirmBtnLabel: string = "";
  leaveDenyBtnLabel: string = "";

  constructor(public dialogRef: MatDialogRef<ErrorDialogComponent>,
              private remoteConfig: AngularFireRemoteConfig) {
    this.loadRemoteConfigTexts();
  }

  private loadRemoteConfigTexts() {
    //AppConfig.isRemoteConfigFetched = false;
    this.remoteConfig.fetchAndActivate().then(hasActivatedTheFetch => {
      this.remoteConfig.getAll().then(all => {
        //AppConfig.isRemoteConfigFetched = true;
        this.leavePopupText = all["LeavePopupText"].asString()!;
        this.leaveComfirmBtnLabel = all["LeaveComfirmBtnLabel"].asString()!;
        this.leaveDenyBtnLabel = all["LeaveDenyBtnLabel"].asString()!;
      })
    })
  }

  ngOnInit(): void {
  }

}