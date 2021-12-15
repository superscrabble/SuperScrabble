import { Component, HostListener, OnInit } from '@angular/core';
import { SignalrService } from 'src/app/services/signalr.service';
import { Tile } from 'src/app/models/tile';
import { Cell } from 'src/app/models/cell';
import { CellViewData } from 'src/app/models/cellViewData';

@Component({
  selector: 'app-game',
  templateUrl: './game.component.html',
  styleUrls: ['./game.component.css']
})
export class GameComponent implements OnInit {

  board: Cell[][] = new Array();
  playerTiles: Tile[] = new Array();
  cellViewDataByType: Map<number, CellViewData> = new Map();
  remainingTilesCount: number = 0;
  pointsByUserNames: Map<string, number> = new Map();
  selectedPlayerTile: Tile | null = null;
  updatedBoardCells: any[] = new Array();
  selectedBoardCell: Cell | null = null;

  constructor(private signalrService: SignalrService) {}

  ngOnInit(): void {
    //verify connection presence
    this.signalrService.startConnection();
    //this.signalrService.addInGameListeners();

    const url = window.location.href;
    const params = url.split("/");
    let id = params[params.length - 1];

    //TODO: fix this to be called after hubConnection is Connected
    this.signalrService.hubConnection?.on("UpdateGameState", data => {      
      console.log(data);
      this.loadBoard(data.commonGameState.board);
      this.loadPlayerTiles(data.tiles);
      this.remainingTilesCount = data.commonGameState.remainingTilesCount;
      this.loadScoreBoard(data.commonGameState.pointsByUserNames)
      console.log("Tiles Count: " + this.remainingTilesCount)
    })

    this.signalrService.hubConnection?.on("InvalidWriteWordInput", data => {
        console.log(data);
    })

    this.signalrService.loadGame(id);
    this.loadCellViewDataByType();
    this.loadMockData();
  }

  loadBoard(board: any): void {
    this.board = new Array(board.height).fill(false).map(() => new Array(board.width));
    
    for(let i = 0; i < board.cells.length; i++) {
      let cell = board.cells[i];
      let tile = cell.tile ? new Tile(cell.tile.letter, cell.tile.points) : null;
      this.board[cell.position.row][cell.position.column] = new Cell(cell.type, tile);
    }
  }

  loadPlayerTiles(playerTiles: any): void {
    console.log(playerTiles);
    this.playerTiles = new Array(playerTiles.length);
    for(let i = 0; i < playerTiles.length; i++) {
      let playerTile = playerTiles[i];
      this.playerTiles[i] = new Tile(playerTile.letter, playerTile.points);
    }
  }

  loadScoreBoard(pointsByUserNames: any): void {
    this.pointsByUserNames.clear();

    for(let i = 0; i < pointsByUserNames.length; i++) {
      this.pointsByUserNames.set(pointsByUserNames[i].key, pointsByUserNames[i].value);
    }

    this.pointsByUserNames = new Map([...this.pointsByUserNames.entries()].sort((a, b) => b[1] - a[1]));
    console.log(this.pointsByUserNames);
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

  getClassNameByCellType(type: number) {
    return this.cellViewDataByType.get(type)?.className;
  }

  getValueWhenEmptyByCellType(type: number) {
    return this.cellViewDataByType.get(type)?.valueWhenEmpty;
  }

  clickOnPlayerTile(playerTile: Tile | any) { //either with index
      if(playerTile != null) {  
        //Check whether the player has the tile
        for(let i = 0; i < this.playerTiles.length; i++) {
          if(this.playerTiles[i] == playerTile) {
            this.selectedPlayerTile = playerTile;
          }
        }
      }
  }

  clickOnBoardCell(cell: Cell | any) {
    if(!cell) {
        return;
    }

    //swap the selected player tile and the selected board cell
    if(cell.tile && this.selectedPlayerTile) {
        let temp = cell.tile;
        cell.tile = this.selectedPlayerTile;
        //check whether the player had the tile in the current round
        this.playerTiles.push(temp);
        this.playerTiles = this.playerTiles.filter(item => item !== this.selectedPlayerTile);
        this.selectedPlayerTile = null;
        return;
    }

    //place the selected player tile
    if(this.selectedPlayerTile) {
        if(!cell.tile) {
            cell.tile = this.selectedPlayerTile;
            this.playerTiles = this.playerTiles.filter(item => item !== this.selectedPlayerTile);
            this.saveUpdatedBoardCellWithPosition(cell);
            this.selectedPlayerTile = null;
            return;
        }
    }

    //select a cell
    if(cell.tile) {
        // TODO: check whether the cell is on the board
        this.selectedBoardCell = cell;
        return;
    }

    //move a tile from a selected cell to another cell
    if(this.selectedBoardCell) {
        let tempTile = this.selectedBoardCell.tile;
        this.selectedBoardCell.tile = cell.tile;
        cell.tile = tempTile;

        this.updatedBoardCells = this.updatedBoardCells.filter(item => item.key !== this.selectedBoardCell);
        this.saveUpdatedBoardCellWithPosition(cell);
        this.selectedBoardCell = null;
        return;
    }
  }

  saveUpdatedBoardCellWithPosition(cell: Cell) {
    for(let row = 0; row < this.board.length; row++) {
        for(let col = 0; col < this.board[row].length; col++) {
            if(this.board[row][col] == cell) {
                this.updatedBoardCells.push({key: cell, value: {row: row, column: col}})
                return;
            }       
        }
    }
  }

  rightClickOnBoardCell(cell: Cell | any) {
    if(cell.tile && cell == this.selectedBoardCell) {
        this.playerTiles.push(cell.tile);
        this.selectedBoardCell = null;
        cell.tile = null;
    }
    return false;
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

  writeWord() : void {
    if(this.updatedBoardCells.length > 0) {
        this.updatedBoardCells = this.updatedBoardCells.map(item => ({key: item.key.tile, value: item.value}))
        this.signalrService.writeWord(this.updatedBoardCells);

        this.updatedBoardCells = new Array();
    }
  }  

  loadMockData(): void {
    let data = 
    {
      "remainingTilesCount": 88,
      "pointsByUserNames": [
          {
              "key": "Gosho2",
              "value": 0
          },
          {
              "key": "Gosho",
              "value": 0
          },
          {
            "key": "Gosho5",
            "value": 0
          },
          {
            "key": "Gosho6",
            "value": 0
          },
      ],
      "board": {
          "cells": [
              {
                  "position": {
                      "row": 0,
                      "column": 0
                  },
                  "type": 5,
                  "tile": null
              },
              {
                  "position": {
                      "row": 0,
                      "column": 1
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 0,
                      "column": 2
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 0,
                      "column": 3
                  },
                  "type": 2,
                  "tile": null
              },
              {
                  "position": {
                      "row": 0,
                      "column": 4
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 0,
                      "column": 5
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 0,
                      "column": 6
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 0,
                      "column": 7
                  },
                  "type": 5,
                  "tile": null
              },
              {
                  "position": {
                      "row": 0,
                      "column": 8
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 0,
                      "column": 9
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 0,
                      "column": 10
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 0,
                      "column": 11
                  },
                  "type": 2,
                  "tile": null
              },
              {
                  "position": {
                      "row": 0,
                      "column": 12
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 0,
                      "column": 13
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 0,
                      "column": 14
                  },
                  "type": 5,
                  "tile": null
              },
              {
                  "position": {
                      "row": 1,
                      "column": 0
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 1,
                      "column": 1
                  },
                  "type": 4,
                  "tile": null
              },
              {
                  "position": {
                      "row": 1,
                      "column": 2
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 1,
                      "column": 3
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 1,
                      "column": 4
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 1,
                      "column": 5
                  },
                  "type": 3,
                  "tile": null
              },
              {
                  "position": {
                      "row": 1,
                      "column": 6
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 1,
                      "column": 7
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 1,
                      "column": 8
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 1,
                      "column": 9
                  },
                  "type": 3,
                  "tile": null
              },
              {
                  "position": {
                      "row": 1,
                      "column": 10
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 1,
                      "column": 11
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 1,
                      "column": 12
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 1,
                      "column": 13
                  },
                  "type": 4,
                  "tile": null
              },
              {
                  "position": {
                      "row": 1,
                      "column": 14
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 2,
                      "column": 0
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 2,
                      "column": 1
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 2,
                      "column": 2
                  },
                  "type": 4,
                  "tile": null
              },
              {
                  "position": {
                      "row": 2,
                      "column": 3
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 2,
                      "column": 4
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 2,
                      "column": 5
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 2,
                      "column": 6
                  },
                  "type": 2,
                  "tile": null
              },
              {
                  "position": {
                      "row": 2,
                      "column": 7
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 2,
                      "column": 8
                  },
                  "type": 2,
                  "tile": null
              },
              {
                  "position": {
                      "row": 2,
                      "column": 9
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 2,
                      "column": 10
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 2,
                      "column": 11
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 2,
                      "column": 12
                  },
                  "type": 4,
                  "tile": null
              },
              {
                  "position": {
                      "row": 2,
                      "column": 13
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 2,
                      "column": 14
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 3,
                      "column": 0
                  },
                  "type": 2,
                  "tile": null
              },
              {
                  "position": {
                      "row": 3,
                      "column": 1
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 3,
                      "column": 2
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 3,
                      "column": 3
                  },
                  "type": 4,
                  "tile": null
              },
              {
                  "position": {
                      "row": 3,
                      "column": 4
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 3,
                      "column": 5
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 3,
                      "column": 6
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 3,
                      "column": 7
                  },
                  "type": 2,
                  "tile": null
              },
              {
                  "position": {
                      "row": 3,
                      "column": 8
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 3,
                      "column": 9
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 3,
                      "column": 10
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 3,
                      "column": 11
                  },
                  "type": 4,
                  "tile": null
              },
              {
                  "position": {
                      "row": 3,
                      "column": 12
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 3,
                      "column": 13
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 3,
                      "column": 14
                  },
                  "type": 2,
                  "tile": null
              },
              {
                  "position": {
                      "row": 4,
                      "column": 0
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 4,
                      "column": 1
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 4,
                      "column": 2
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 4,
                      "column": 3
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 4,
                      "column": 4
                  },
                  "type": 4,
                  "tile": null
              },
              {
                  "position": {
                      "row": 4,
                      "column": 5
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 4,
                      "column": 6
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 4,
                      "column": 7
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 4,
                      "column": 8
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 4,
                      "column": 9
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 4,
                      "column": 10
                  },
                  "type": 4,
                  "tile": null
              },
              {
                  "position": {
                      "row": 4,
                      "column": 11
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 4,
                      "column": 12
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 4,
                      "column": 13
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 4,
                      "column": 14
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 5,
                      "column": 0
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 5,
                      "column": 1
                  },
                  "type": 3,
                  "tile": null
              },
              {
                  "position": {
                      "row": 5,
                      "column": 2
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 5,
                      "column": 3
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 5,
                      "column": 4
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 5,
                      "column": 5
                  },
                  "type": 3,
                  "tile": null
              },
              {
                  "position": {
                      "row": 5,
                      "column": 6
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 5,
                      "column": 7
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 5,
                      "column": 8
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 5,
                      "column": 9
                  },
                  "type": 3,
                  "tile": null
              },
              {
                  "position": {
                      "row": 5,
                      "column": 10
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 5,
                      "column": 11
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 5,
                      "column": 12
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 5,
                      "column": 13
                  },
                  "type": 3,
                  "tile": null
              },
              {
                  "position": {
                      "row": 5,
                      "column": 14
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 6,
                      "column": 0
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 6,
                      "column": 1
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 6,
                      "column": 2
                  },
                  "type": 2,
                  "tile": null
              },
              {
                  "position": {
                      "row": 6,
                      "column": 3
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 6,
                      "column": 4
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 6,
                      "column": 5
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 6,
                      "column": 6
                  },
                  "type": 2,
                  "tile": null
              },
              {
                  "position": {
                      "row": 6,
                      "column": 7
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 6,
                      "column": 8
                  },
                  "type": 2,
                  "tile": null
              },
              {
                  "position": {
                      "row": 6,
                      "column": 9
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 6,
                      "column": 10
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 6,
                      "column": 11
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 6,
                      "column": 12
                  },
                  "type": 2,
                  "tile": null
              },
              {
                  "position": {
                      "row": 6,
                      "column": 13
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 6,
                      "column": 14
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 7,
                      "column": 0
                  },
                  "type": 5,
                  "tile": null
              },
              {
                  "position": {
                      "row": 7,
                      "column": 1
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 7,
                      "column": 2
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 7,
                      "column": 3
                  },
                  "type": 2,
                  "tile": null
              },
              {
                  "position": {
                      "row": 7,
                      "column": 4
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 7,
                      "column": 5
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 7,
                      "column": 6
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 7,
                      "column": 7
                  },
                  "type": 1,
                  "tile": null
              },
              {
                  "position": {
                      "row": 7,
                      "column": 8
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 7,
                      "column": 9
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 7,
                      "column": 10
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 7,
                      "column": 11
                  },
                  "type": 2,
                  "tile": null
              },
              {
                  "position": {
                      "row": 7,
                      "column": 12
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 7,
                      "column": 13
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 7,
                      "column": 14
                  },
                  "type": 5,
                  "tile": null
              },
              {
                  "position": {
                      "row": 8,
                      "column": 0
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 8,
                      "column": 1
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 8,
                      "column": 2
                  },
                  "type": 2,
                  "tile": null
              },
              {
                  "position": {
                      "row": 8,
                      "column": 3
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 8,
                      "column": 4
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 8,
                      "column": 5
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 8,
                      "column": 6
                  },
                  "type": 2,
                  "tile": null
              },
              {
                  "position": {
                      "row": 8,
                      "column": 7
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 8,
                      "column": 8
                  },
                  "type": 2,
                  "tile": null
              },
              {
                  "position": {
                      "row": 8,
                      "column": 9
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 8,
                      "column": 10
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 8,
                      "column": 11
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 8,
                      "column": 12
                  },
                  "type": 2,
                  "tile": null
              },
              {
                  "position": {
                      "row": 8,
                      "column": 13
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 8,
                      "column": 14
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 9,
                      "column": 0
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 9,
                      "column": 1
                  },
                  "type": 3,
                  "tile": null
              },
              {
                  "position": {
                      "row": 9,
                      "column": 2
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 9,
                      "column": 3
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 9,
                      "column": 4
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 9,
                      "column": 5
                  },
                  "type": 3,
                  "tile": null
              },
              {
                  "position": {
                      "row": 9,
                      "column": 6
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 9,
                      "column": 7
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 9,
                      "column": 8
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 9,
                      "column": 9
                  },
                  "type": 3,
                  "tile": null
              },
              {
                  "position": {
                      "row": 9,
                      "column": 10
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 9,
                      "column": 11
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 9,
                      "column": 12
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 9,
                      "column": 13
                  },
                  "type": 3,
                  "tile": null
              },
              {
                  "position": {
                      "row": 9,
                      "column": 14
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 10,
                      "column": 0
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 10,
                      "column": 1
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 10,
                      "column": 2
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 10,
                      "column": 3
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 10,
                      "column": 4
                  },
                  "type": 4,
                  "tile": null
              },
              {
                  "position": {
                      "row": 10,
                      "column": 5
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 10,
                      "column": 6
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 10,
                      "column": 7
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 10,
                      "column": 8
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 10,
                      "column": 9
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 10,
                      "column": 10
                  },
                  "type": 4,
                  "tile": null
              },
              {
                  "position": {
                      "row": 10,
                      "column": 11
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 10,
                      "column": 12
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 10,
                      "column": 13
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 10,
                      "column": 14
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 11,
                      "column": 0
                  },
                  "type": 2,
                  "tile": null
              },
              {
                  "position": {
                      "row": 11,
                      "column": 1
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 11,
                      "column": 2
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 11,
                      "column": 3
                  },
                  "type": 4,
                  "tile": null
              },
              {
                  "position": {
                      "row": 11,
                      "column": 4
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 11,
                      "column": 5
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 11,
                      "column": 6
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 11,
                      "column": 7
                  },
                  "type": 2,
                  "tile": null
              },
              {
                  "position": {
                      "row": 11,
                      "column": 8
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 11,
                      "column": 9
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 11,
                      "column": 10
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 11,
                      "column": 11
                  },
                  "type": 4,
                  "tile": null
              },
              {
                  "position": {
                      "row": 11,
                      "column": 12
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 11,
                      "column": 13
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 11,
                      "column": 14
                  },
                  "type": 2,
                  "tile": null
              },
              {
                  "position": {
                      "row": 12,
                      "column": 0
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 12,
                      "column": 1
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 12,
                      "column": 2
                  },
                  "type": 4,
                  "tile": null
              },
              {
                  "position": {
                      "row": 12,
                      "column": 3
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 12,
                      "column": 4
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 12,
                      "column": 5
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 12,
                      "column": 6
                  },
                  "type": 2,
                  "tile": null
              },
              {
                  "position": {
                      "row": 12,
                      "column": 7
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 12,
                      "column": 8
                  },
                  "type": 2,
                  "tile": null
              },
              {
                  "position": {
                      "row": 12,
                      "column": 9
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 12,
                      "column": 10
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 12,
                      "column": 11
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 12,
                      "column": 12
                  },
                  "type": 4,
                  "tile": null
              },
              {
                  "position": {
                      "row": 12,
                      "column": 13
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 12,
                      "column": 14
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 13,
                      "column": 0
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 13,
                      "column": 1
                  },
                  "type": 4,
                  "tile": null
              },
              {
                  "position": {
                      "row": 13,
                      "column": 2
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 13,
                      "column": 3
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 13,
                      "column": 4
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 13,
                      "column": 5
                  },
                  "type": 3,
                  "tile": null
              },
              {
                  "position": {
                      "row": 13,
                      "column": 6
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 13,
                      "column": 7
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 13,
                      "column": 8
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 13,
                      "column": 9
                  },
                  "type": 3,
                  "tile": null
              },
              {
                  "position": {
                      "row": 13,
                      "column": 10
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 13,
                      "column": 11
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 13,
                      "column": 12
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 13,
                      "column": 13
                  },
                  "type": 4,
                  "tile": null
              },
              {
                  "position": {
                      "row": 13,
                      "column": 14
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 14,
                      "column": 0
                  },
                  "type": 5,
                  "tile": null
              },
              {
                  "position": {
                      "row": 14,
                      "column": 1
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 14,
                      "column": 2
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 14,
                      "column": 3
                  },
                  "type": 2,
                  "tile": null
              },
              {
                  "position": {
                      "row": 14,
                      "column": 4
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 14,
                      "column": 5
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 14,
                      "column": 6
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 14,
                      "column": 7
                  },
                  "type": 5,
                  "tile": null
              },
              {
                  "position": {
                      "row": 14,
                      "column": 8
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 14,
                      "column": 9
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 14,
                      "column": 10
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 14,
                      "column": 11
                  },
                  "type": 2,
                  "tile": null
              },
              {
                  "position": {
                      "row": 14,
                      "column": 12
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 14,
                      "column": 13
                  },
                  "type": 0,
                  "tile": null
              },
              {
                  "position": {
                      "row": 14,
                      "column": 14
                  },
                  "type": 5,
                  "tile": null
              }
          ],
          "width": 15,
          "height": 15
      }
    }
    let tiles =
    [
      {
          "letter": "ь",
          "points": 10
      },
      {
          "letter": "Е",
          "points": 1
      },
      {
        "letter": "Е",
        "points": 2
      },
      {
          "letter": "И",
          "points": 1
      },
      {
          "letter": "З",
          "points": 4
      },
      {
          "letter": "Т",
          "points": 1
      },
      {
          "letter": "К",
          "points": 2
      },
      {
          "letter": "П",
          "points": 1
      }
  ]

    this.loadBoard(data.board);
    this.loadPlayerTiles(tiles);
    this.remainingTilesCount = data.remainingTilesCount;
    this.loadScoreBoard(data.pointsByUserNames)
    console.log("Tiles Count: " + this.remainingTilesCount)
  }
}
