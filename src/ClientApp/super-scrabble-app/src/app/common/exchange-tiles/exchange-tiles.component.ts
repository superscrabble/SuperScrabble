import { Component, Input, OnInit } from '@angular/core';
import { AngularFireRemoteConfig } from '@angular/fire/compat/remote-config';
import { Tile } from 'src/app/models/tile';

@Component({
  selector: 'app-exchange-tiles',
  templateUrl: './exchange-tiles.component.html',
  styleUrls: ['./exchange-tiles.component.css']
})
export class ExchangeTilesComponent implements OnInit {

  @Input() playerTiles: Tile[] = [];

  selectedExchangeTiles: Tile[] = new Array();
  changeLetterSecondBtnLabel: string = "";

  constructor(private remoteConfig: AngularFireRemoteConfig) {
    this.loadRemoteConfigTexts();
  }

  ngOnInit(): void {
  }

  private loadRemoteConfigTexts() {
    //AppConfig.isRemoteConfigFetched = false;
    this.remoteConfig.fetchAndActivate().then(hasActivatedTheFetch => {
      this.remoteConfig.getAll().then(all => {
        //AppConfig.isRemoteConfigFetched = true;
        this.changeLetterSecondBtnLabel = all["ChangeLetterSecondBtnLabel"].asString()!;
      })
    })
  }

  getClassNameIfSelectedExchangeTile(tile: Tile) {
    for(let i = 0; i < this.selectedExchangeTiles.length; i++) {
        if(this.selectedExchangeTiles[i] == tile) {
            return "selected-tile";
        }
    }
    return "";
  }

  clickOnExchangeTile(tile: Tile) {
    if(tile) {
        //Deselecting
        for(let i = 0; i < this.selectedExchangeTiles.length; i++) {
            if(this.selectedExchangeTiles[i] == tile) {
                this.selectedExchangeTiles = this.selectedExchangeTiles.filter(item => item !== tile)
                return;
            }
        }
        this.selectedExchangeTiles.push(tile);
    }
  }

  exchangeSelectedTiles() {
    //this.signalrService.exchangeTiles(this.selectedExchangeTiles);
    //this.showExchangeField = false;
    this.selectedExchangeTiles = [];
  }
}
