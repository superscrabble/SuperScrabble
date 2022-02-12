import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Tile } from 'src/app/models/tile';
import { CdkDragDrop, CdkDragEnter, moveItemInArray, transferArrayItem } from "@angular/cdk/drag-drop";
import { AppConfig } from 'src/app/app-config';

@Component({
  selector: 'app-player-rack',
  templateUrl: './player-rack.component.html',
  styleUrls: ['./player-rack.component.scss']
})
export class PlayerRackComponent implements OnInit {

  @Input() playerTiles: Tile[] = [];
  @Input() isEnabled: boolean = false;
  @Output() openWildcardDialogEvent: EventEmitter<any> = new EventEmitter();
  @Output() removeTileFromBoard: EventEmitter<any> = new EventEmitter();
  selectedPlayerTile: Tile = this.playerTiles[0];

  constructor() { }

  ngOnInit(): void {
  }

  doubleClickOnPlayerTile(playerTile: Tile) {
    if((playerTile.letter == AppConfig.WildcardSymbol
        || playerTile.points == 0)
        && this.playerTiles.find(item => item == playerTile)
        && this.isEnabled) {
          this.openWildcardDialogEvent.emit({tile: playerTile, writeWordInput: null});
    }
  }

  drop(event: CdkDragDrop<Tile[]>) {
    console.log("DROPPING IN PLAYER TILES")
    console.log(event.previousIndex + " " + event.previousContainer.data + " " + event.currentIndex);

    if(!this.isEnabled) {
      return;
    }

    if(event.previousContainer === event.container)
        moveItemInArray(this.playerTiles, event.previousIndex, event.currentIndex);
    else {
        if(event.previousContainer.data) {
          transferArrayItem(event.previousContainer.data, event.container.data, event.previousIndex, event.currentIndex);
        } else {
          //Remove the tile from the board
          this.removeTileFromBoard.emit(event.item.data);
          this.playerTiles.push(event.item.data);
        }
    }
  }

  getClassOfTile(object: Tile) {
    let result = "";
    if(this.selectedPlayerTile && this.selectedPlayerTile == object) {
      result += "selected-tile";
    }
    if(this.isEnabled) {
      result += " cursor-pointer";
    }
    return result;
  }
}
