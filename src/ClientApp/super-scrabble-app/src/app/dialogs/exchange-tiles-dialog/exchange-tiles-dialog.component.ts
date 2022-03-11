import { Component, Inject, OnInit } from '@angular/core';
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

  constructor(@Inject(MAT_DIALOG_DATA) public data: ExchangeTileDialogData) { }

  ngOnInit(): void {
  }

}
