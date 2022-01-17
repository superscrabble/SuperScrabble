import { Component, Input, OnInit } from '@angular/core';
import { Tile } from 'src/app/models/tile';

@Component({
  selector: 'app-exchange-tiles',
  templateUrl: './exchange-tiles.component.html',
  styleUrls: ['./exchange-tiles.component.css']
})
export class ExchangeTilesComponent implements OnInit {

  @Input() playerTiles: Tile[] = [];

  selectedExchangeTiles: Tile[] = new Array();

  constructor() { }

  ngOnInit(): void {
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
