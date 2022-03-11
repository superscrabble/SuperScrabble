import { Component, Inject, OnInit } from '@angular/core';
import { AngularFireRemoteConfig } from '@angular/fire/compat/remote-config';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Tile } from 'src/app/models/tile';

export interface ExchangeTileDialogData {
  playerTiles: Tile[];
}

@Component({
  selector: 'app-exchange-tiles-dialog',
  templateUrl: './exchange-tiles-dialog.component.html',
  styleUrls: ['./exchange-tiles-dialog.component.css']
})
export class ExchangeTilesDialogComponent implements OnInit {

  exchangeTilesTitle: string = "";

  constructor(@Inject(MAT_DIALOG_DATA) public data: ExchangeTileDialogData,
              private remoteConfig: AngularFireRemoteConfig) {
    this.loadRemoteConfigTexts();
  }

  ngOnInit(): void {
  }

  private loadRemoteConfigTexts() {
    //AppConfig.isRemoteConfigFetched = false;
    this.remoteConfig.fetchAndActivate().then(hasActivatedTheFetch => {
      this.remoteConfig.getAll().then(all => {
        //AppConfig.isRemoteConfigFetched = true;
        this.exchangeTilesTitle = all["ExchangeTilesTitle"].asString()!;
      })
    })
  }
}
