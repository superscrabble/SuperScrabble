import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { AngularFireRemoteConfig } from '@angular/fire/compat/remote-config';

export interface ErrorDialogData {
  message: string;
  unexistingWords: string[] | null;
}

@Component({
  selector: 'app-error-dialog',
  templateUrl: './error-dialog.component.html',
  styleUrls: ['./error-dialog.component.css']
})
export class ErrorDialogComponent implements OnInit {

  settingsTitle: string = "";
  errorDialogOkButton: string = "";
  unexistingWords: string = "";

  constructor(public dialogRef: MatDialogRef<ErrorDialogComponent>, 
    @Inject(MAT_DIALOG_DATA) public data: ErrorDialogData, private remoteConfig: AngularFireRemoteConfig) {
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
        this.errorDialogOkButton = all["ErrorDialogOkButton"].asString()!;
        this.unexistingWords = all["UnexistingWords"].asString()!;
      })
    })
  }

  convertMessage(message: string) {
    switch(message) {
      case("UnexistingWords"): {
        return this.unexistingWords;
        break;
      }
    }
    return "";
  }
}