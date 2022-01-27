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
  @Output() openWildcardDialogEvent: EventEmitter<any> = new EventEmitter();
  @Input() isEnabled: boolean = false;

  constructor() { }

  ngOnInit(): void {
  }

  doubleClickOnPlayerTile(playerTile: Tile) {
    if((playerTile.letter == AppConfig.WildcardSymbol
        || playerTile.points == 0)
        && this.playerTiles.find(item => item == playerTile)) {
          this.openWildcardDialogEvent.emit({tile: playerTile, writeWordInput: null});
    }
  }

  drop(event: CdkDragDrop<Tile[]>) {
    console.log("DROPPING IN PLAYER TILES")
    console.log(event.previousIndex + " " + event.previousContainer.data + " " + event.currentIndex);
    if(event.previousContainer === event.container)
        moveItemInArray(this.playerTiles, event.previousIndex, event.currentIndex);
    else {
        transferArrayItem(event.previousContainer.data, event.container.data, event.previousIndex, event.currentIndex);
    }
  }
}
