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
    console.log("DROPPING IN PLAYER TILES")
    console.log(event.previousIndex + " " + event.previousContainer.data + " " + event.currentIndex);
    console.log(event);

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
    //document.querySelector('#tilePlaceholder')?.classList.remove('d-none');
    //document.querySelector('#tilePlaceholder')?.classList.add('d-flex');
    //event.container.element.nativeElement.classList.remove("d-none")
  }

  onDragEntered(event: CdkDragEnter<any>) {
    console.log("DRAG ENTERED")
    console.log(event);
    if(event.item.dropContainer === event.container) {
      console.log("Inside")
      this.isInside = true;
      //event.container.element.nativeElement.classList.remove("d-none")
      this.placeholderClass = "d-flex";
      console.log(event.item.getPlaceholderElement())
      //document.querySelector('#tilePlaceholder')?.classList.remove('d-none');
      //document.querySelector('#tilePlaceholder')?.classList.add('d-flex');
    } else {
      console.log("Outside")
      this.isInside = false;
      //event.container.element.nativeElement.classList.add("d-none")
      //document.querySelector('#tilePlaceholder')?.classList.remove('d-flex');
      //document.querySelector('#tilePlaceholder')?.classList.add('d-none');
      this.placeholderClass = "d-none";
    }
  }

  onDragDropped(event: CdkDragDrop<any>) {
    this.isInside = true;
    this.placeholderClass = "d-flex";
    console.log("Dropped")
    console.log(event.dropPoint)    
    //document.querySelector('#tilePlaceholder')?.classList.remove('d-none');
    //document.querySelector('#tilePlaceholder')?.classList.add('d-flex');
    //event.container.element.nativeElement.classList.remove("d-none")
  }

  onDragMoved(event: CdkDragMove<any>) {
    let value = this.placeholderClass;
    this.placeholderClass = "";
    this.placeholderClass = value;
    //console.log("DRAG MOVED");
    //console.log(this.placeholderClass);
  }

  getClass() {
    if(!this.isInside) {
      return "d-none";
    } else {
      return "";
    }
  }
}
