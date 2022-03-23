import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Tile } from 'src/app/models/tile';
import { CdkDragDrop, CdkDragEnter, CdkDragMove, moveItemInArray, transferArrayItem } from "@angular/cdk/drag-drop";
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
    if(!this.isEnabled) {
      return;
    }

    if(event.previousContainer === event.container)
      moveItemInArray(this.playerTiles, event.previousIndex, event.currentIndex);
    else {
      if(event.previousContainer.data) {
        //this may work wrongly
        transferArrayItem(event.previousContainer.data, event.container.data, event.previousIndex, event.currentIndex);
      } else {
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

  isInsideCurrentDropList() {
    return this.isInside;
  }

  isInside: boolean = true;
  placeholderClass: string = "";

  onDropListExited() {
    //this.isInside = false;
  }

  onDropListEntered(event: CdkDragEnter<any>) {
    this.isInside = true;
    this.placeholderClass = "d-flex";
  }

  onDragEntered(event: CdkDragEnter<any>) {
    if(event.item.dropContainer === event.container) {
      this.isInside = true;
      this.placeholderClass = "mx-2 border tile cursor-default rounded-1 p-2 text-center {{getClassOfTile(playerTile)}} d-flex";
    } else {
      this.isInside = false;
      this.placeholderClass = "mx-2 border tile cursor-default rounded-1 p-2 text-center {{getClassOfTile(playerTile)}} d-none";
    }
  }

  onDragDropped(event: CdkDragDrop<any>) {
    this.isInside = true;
    this.placeholderClass = "mx-2 border tile cursor-default rounded-1 p-2 text-center {{getClassOfTile(playerTile)}} d-flex";
  }

  onDragMoved(event: CdkDragMove<any>) {
    let value = this.placeholderClass;
    this.placeholderClass = "";
    this.placeholderClass = value;
  }

  getClass() {
    if(!this.isInside) {
      return "d-none";
    } else {
      return "";
    }
  }
}
