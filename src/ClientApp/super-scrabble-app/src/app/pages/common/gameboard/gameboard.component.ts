import { Component, Input, OnInit, Output, EventEmitter, NgZone, ElementRef, Renderer2 } from '@angular/core';
import { Cell } from 'src/app/models/cell';
import { CellViewData } from 'src/app/models/cellViewData';
import { Tile } from 'src/app/models/tile';
import { AppConfig } from 'src/app/app-config';
import { ViewportRuler } from '@angular/cdk/scrolling';

@Component({
  selector: 'app-gameboard',
  templateUrl: './gameboard.component.html',
  styleUrls: ['./gameboard.component.css']
})
export class GameboardComponent implements OnInit {

  @Input() board: Cell[][] = new Array();
  @Input() selectedPlayerTile: Tile | null = null;
  
  //TODO: maybe move this into a class
  @Input() updatedBoardCells: Array<{ cell: Cell, coordinates: { column: number, row: number } }> = new Array();
  @Output() openWildcardDialogEvent: EventEmitter<any> = new EventEmitter();
  @Output() removeTileFromPlayerTiles: EventEmitter<any> = new EventEmitter();
  @Output() addTileToPlayerTiles: EventEmitter<any> = new EventEmitter();
  
  cellViewDataByType: Map<number, CellViewData> = new Map();
  selectedBoardCell: Cell | null = null;
  
  //TODO: check whether this is ok
  private readonly viewportChange = this.viewportRuler
    .change(200)
    .subscribe(() => this.ngZone.run(() => this.onSizeChange()));

  constructor(private viewportRuler: ViewportRuler, private ngZone: NgZone,
              private elementRef: ElementRef, private renderer: Renderer2) {
      this.loadCellViewDataByType();
  }

  onSizeChange() : void {      
      let verticalCellsCount = this.board.length;
      let horizontalCellsCount = 0;
      
      //Finding the longest row
      for(let i = 0; i < verticalCellsCount; i++) {
          if(horizontalCellsCount > this.board[i].length) {
              horizontalCellsCount = this.board[i].length;
          }
      }
      
      let minNeededBoardWidth = AppConfig.BoardCellMinWidth * horizontalCellsCount;
      let minNeededBoardHeight = AppConfig.BoardCellMinHeight * verticalCellsCount;

      const screenWidth = this.elementRef.nativeElement.offsetWidth;
      const screenHeight = this.elementRef.nativeElement.offsetHeight;

      //TODO: move this into variable
      let cellWidth = '2.2rem';

      if((screenHeight > minNeededBoardHeight)
            && (screenWidth > minNeededBoardWidth)) {
          //TODO: move this into variable
          cellWidth = "1fr";
      }

      this.renderer.setStyle(this.elementRef.nativeElement, '--cell-width', cellWidth, 2);
      this.renderer.setStyle(this.elementRef.nativeElement, '--cell-height', cellWidth, 2);
  }

  ngOnInit(): void {
    this.onSizeChange();
  }

  loadCellViewDataByType() {
    const emptyCellValue = " ";
    this.cellViewDataByType = new Map([
      [0, new CellViewData("basic-cell", emptyCellValue)],
      [1, new CellViewData("center-cell", emptyCellValue)],
      [2, new CellViewData("x2-letter-cell", "x2")],
      [3, new CellViewData("x3-letter-cell", "x3")],
      [4, new CellViewData("x2-word-cell", "x2")],
      [5, new CellViewData("x3-word-cell", "x3")],
    ]);
  }

  getValueWhenEmptyByCellType(type: number) {
    return this.cellViewDataByType.get(type)?.valueWhenEmpty;
  }

  rightClickOnBoardCell(cell: Cell | any) {
    if(cell.tile && cell == this.selectedBoardCell) {
        this.addTileToPlayerTiles.emit(cell.tile);
        //this.playerTiles.push(cell.tile);
        this.removeCellFromUpdatedBoardCells(cell);
        this.selectedBoardCell = null;
        cell.tile = null;
    }
    return false;
  }

  doubleClickOnBoardCell(cell: Cell) {
    if((cell.tile?.letter == AppConfig.WildcardSymbol
        || cell.tile?.points == 0)
        && this.updatedBoardCells.find(item => item.cell == cell)) {
        this.openWildcardDialogEvent.emit({tile: cell.tile, writeWordInput: null});
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
            if(this.updatedBoardCells[i].cell == cell) {
                isNewCell = true;
                break;
            }
        }

        if(!isNewCell) {
            return;
        }

        let temp = cell.tile;
        cell.tile = this.selectedPlayerTile;
        this.addTileToPlayerTiles.emit(temp);
        //this.playerTiles.push(temp);
        this.removeTileFromPlayerTiles.emit(this.selectedPlayerTile);
        //this.removeTileFromPlayerTiles(this.selectedPlayerTile);
        this.selectedPlayerTile = null;
        return;
    }

    //place the selected player tile
    if(this.selectedPlayerTile) {
        if(!cell.tile) {
            cell.tile = this.selectedPlayerTile;
            this.removeTileFromPlayerTiles.emit(this.selectedPlayerTile);
            //this.removeTileFromPlayerTiles(this.selectedPlayerTile);
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
            if(this.updatedBoardCells[i].cell == cell
                || this.updatedBoardCells[i].cell == cell.tile) {
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

  removeCellFromUpdatedBoardCells(cell: Cell) {
      if(cell) {
        this.updatedBoardCells = this.updatedBoardCells.filter(item => item.cell !== cell);
      }
  }

  addCellToUpdatedBoardCells(cell: Cell) {
    if(cell && cell.tile) {
        for(let i = 0; i < this.updatedBoardCells.length; i++) {
            if(this.updatedBoardCells[i].cell == cell) {
                return;
            }
        }
        this.saveUpdatedBoardCellWithPosition(cell);
    }
  }

  saveUpdatedBoardCellWithPosition(cell: Cell) {
    for(let row = 0; row < this.board.length; row++) {
        for(let col = 0; col < this.board[row].length; col++) {
            if(this.board[row][col] == cell) {
                this.updatedBoardCells.push({cell: cell, coordinates: {row: row, column: col}})
                return;
            }       
        }
    }
  }

  getClassNameByCell(cell: Cell) {
      let result = " ";

      result += this.getClassNameIfPlayerIsNotOnTurn();
      result += " " + this.getClassNameByCellType(cell.type);
      result += " " + this.getClassNameIfCellIsTaken(cell);
      result += " " + this.getClassNameIfSelected(cell);
      result += " " + this.getClassNameIfNewCell(cell);

      return result;
  }

  //TODO: Think how to show that a cell is disabled
  getClassNameIfPlayerIsNotOnTurn() {
    /*if(!this.isCurrentPlayerOnTurn()) {
        return "basic-cell-disabled"
    }*/
    return "";
  }

  getClassNameIfNewCell(cell: Cell) {
      for(let i = 0; i < this.updatedBoardCells.length; i++) {
          if(this.updatedBoardCells[i].cell == cell) {
              //TODO: think how to show that a cell is new
              //return "border border-info";
          }
      }
      return "";
  }

  getClassNameByCellType(type: number) {
    return this.cellViewDataByType.get(type)?.className;
  }

  getClassNameIfCellIsTaken(cell: Cell) {
    if(cell.tile) {
        if(this.selectedBoardCell == cell) {
            return "selected-tile rounded-1";
        } else {
            return "tile-on-cell rounded-1";
        }
    }
    return "";
  }

  getClassNameIfSelected(object: Tile | Cell | any) {
    if(object instanceof Tile) {
        if(this.selectedPlayerTile && this.selectedPlayerTile == object) {
            return "selected-tile";
        }
    } else if(object instanceof Cell) {
        if(this.selectedBoardCell && this.selectedBoardCell == object) {
            return "selected-cell";
        }
    }
    return "";
  }
}