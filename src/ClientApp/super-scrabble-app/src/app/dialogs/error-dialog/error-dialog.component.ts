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
  errors: Map<string, string> = new Map();

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

        this.errors.set("UnexistingWords", all["UnexistingWords"].asString()!);
        this.errors.set("InvalidInputTilesCount", all["InvalidInputTilesCount"].asString()!);
        this.errors.set("FirstWordMustGoThroughTheBoardCenter", all["FirstWordMustGoThroughTheBoardCenter"].asString()!);
        this.errors.set("GapsBetweenInputTilesNotAllowed", all["GapsBetweenInputTilesNotAllowed"].asString()!);
        this.errors.set("ImpossibleTileExchange", all["ImpossibleTileExchange"].asString()!);
        this.errors.set("InputTilesPositionsCollide", all["InputTilesPositionsCollide"].asString()!);
        this.errors.set("NewTilesNotConnectedToTheOldOnes", all["NewTilesNotConnectedToTheOldOnes"].asString()!);
        this.errors.set("TilePositionAlreadyTaken", all["TilePositionAlreadyTaken"].asString()!);
        this.errors.set("TilesNotOnTheSameLine", all["TilesNotOnTheSameLine"].asString()!);
      })
    })
  }

  convertMessage(message: string) {
    if(this.errors.has(message)) {
      return this.errors.get(message);
    }
    return "";
  }
}