import { Component, Input, OnInit } from '@angular/core';
import { Cell } from 'src/app/models/cell';
import { CellViewData } from 'src/app/models/cellViewData';
import { Tile } from 'src/app/models/tile';
import { AppConfig } from 'src/app/app-config';

@Component({
  selector: 'app-gameboard',
  templateUrl: './gameboard.component.html',
  styleUrls: ['./gameboard.component.css']
})
export class GameboardComponent implements OnInit {

  @Input() board: Cell[][] = new Array();
  cellViewDataByType: Map<number, CellViewData> = new Map();
  updatedBoardCells: any[] = new Array();

  constructor() { }

  ngOnInit(): void {
  }

  getValueWhenEmptyByCellType(type: number) {
    return this.cellViewDataByType.get(type)?.valueWhenEmpty;
  }

  doubleClickOnBoardCell(cell: Cell) {
    if((cell.tile?.letter == AppConfig.WildcardSymbol
        || cell.tile?.points == 0)
        && this.updatedBoardCells.find(item => item.key == cell)) {
        this.dialog.open(ChangeWildcardDialog, { data: { tiles: this.wildcardOptions, 
            tile: cell.tile, writeWordInput: null}});
    }
  }  

  clickOnBoardCell(cell: Cell | any) {
    if(!cell) {
        return;
    }

    console.log("CLICK ON BOARD CELL")
    console.log(this.updatedBoardCells);

    //swap the selected player tile and the selected board cell
    if(cell.tile && this.selectedPlayerTile) {
        let isNewCell = false;
        for(let i = 0; i < this.updatedBoardCells.length; i++) {
            if(this.updatedBoardCells[i].key == cell) {
                isNewCell = true;
                break;
            }
        }

        if(!isNewCell) {
            return;
        }

        let temp = cell.tile;
        cell.tile = this.selectedPlayerTile;
        this.playerTiles.push(temp);
        this.removeTileFromPlayerTiles(this.selectedPlayerTile);
        this.selectedPlayerTile = null;
        return;
    }

    //place the selected player tile
    if(this.selectedPlayerTile) {
        if(!cell.tile) {
            cell.tile = this.selectedPlayerTile;
            this.removeTileFromPlayerTiles(this.selectedPlayerTile);
            this.addCellToUpdatedBoardCells(cell);
            this.selectedPlayerTile = null;
            return;
        }
    }

    //select a cell
    if(cell.tile) {
        if(cell == this.selectedBoardCell) {
            this.selectedBoardCell = null;
            return;
        }

        for(let i = 0; i < this.updatedBoardCells.length; i++) {
            //TODO: check for posible bugs with the second condition
            if(this.updatedBoardCells[i].key == cell
                || this.updatedBoardCells[i].key == cell.tile) {
                this.selectedBoardCell = cell;
                return;
            }
        }
        return;
    }

    //move a tile from a selected cell to another cell
    if(this.selectedBoardCell) {
        let tempTile = this.selectedBoardCell.tile;
        this.selectedBoardCell.tile = cell.tile;
        cell.tile = tempTile;

        this.removeCellFromUpdatedBoardCells(this.selectedBoardCell);
        this.addCellToUpdatedBoardCells(cell);
        this.selectedBoardCell = null;

        console.log("UPDATED BOARD CELLS: ");
        console.log(this.updatedBoardCells);
        return;
    }
  }
}
