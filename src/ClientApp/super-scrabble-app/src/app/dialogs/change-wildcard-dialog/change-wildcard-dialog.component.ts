import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { AppConfig } from 'src/app/app-config';
import { Tile } from 'src/app/models/tile';
import { SignalrService } from 'src/app/services/signalr.service';

export interface ChangeWildcardDialogData {
  tiles: Tile[];
  writeWordInput: any[];
  tile: Tile;
}

@Component({
  selector: 'app-change-wildcard-dialog',
  templateUrl: './change-wildcard-dialog.component.html',
  styleUrls: ['./change-wildcard-dialog.component.css']
})
export class ChangeWildcardDialogComponent implements OnInit {

  selectedTile: Tile | null = null;
  tiles: any[] = new Array();

  constructor(public dialogRef: MatDialogRef<ChangeWildcardDialogComponent>, 
              @Inject(MAT_DIALOG_DATA) public data: ChangeWildcardDialogData,
              public signalrService: SignalrService) {
                  this.tiles = data.tiles;
              }

  ngOnInit(): void {
      
  }

  getClassNameIfSelected(tile: Tile) {
      if(tile == this.selectedTile) {
          return "selected-tile";
      }
      return "";
  }

  clickOnTile(tile: Tile) {
      if(tile) {
          if(tile == this.selectedTile) {
              this.selectedTile = null;
              return;
          }
          this.selectedTile = tile;
      }
  }

  close() {
      if(this.selectedTile) {
          this.data.tile.letter = this.selectedTile.letter
      }
      if(this.data.writeWordInput) {
          //check whether all wildcards are not empty
          if(!this.checkForEmptyWildcards(this.data.writeWordInput)) {
              try {
                  this.signalrService.writeWord(this.data.writeWordInput);
              }
              catch (ex) {
                  console.log("ERROR");
                  console.log(ex);
              } 
          }
      }
  }

  checkForEmptyWildcards(tiles: any[]) : boolean {
      for(let i = 0; i < tiles.length; i++) {
          if(tiles[i].key.letter == AppConfig.WildcardSymbol) {
              return true;
          }
      }
      return false;
  }
}