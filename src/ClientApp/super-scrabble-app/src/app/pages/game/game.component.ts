import { Component, OnInit, Inject, NgZone } from '@angular/core';
import { SignalrService } from 'src/app/services/signalr.service';
import { Tile } from 'src/app/models/tile';
import { Cell } from 'src/app/models/cell';
import { CellViewData } from 'src/app/models/cellViewData';
import { HubConnection, HubConnectionState } from '@microsoft/signalr';
import { Router } from '@angular/router';
import {MatDialog, MatDialogRef, MatDialogState, MAT_DIALOG_DATA} from '@angular/material/dialog';

export interface ErrorDialogData {
    message: string;
    unexistingWords: string[] | null;
}

export interface ChangeWildcardDialogData {
    tiles: Tile[];
    writeWordInput: any[];
    tile: Tile;
}

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
  pointsByUserNames: any[] = new Array();
  selectedPlayerTile: Tile | null = null;
  updatedBoardCells: any[] = new Array();
  selectedBoardCell: Cell | null = null;
  playerNameOnTurn: string = "";
  currentUserName: string = "";
  showExchangeField: boolean = false;
  selectedExchangeTiles: Tile[] = new Array();
  isTileExchangePossible: boolean = true;
  wildcardOptions: Tile[] = new Array();
  //TODO: move this constant in a global file
  WILDCARD_SYMBOL: string = "\u0000";
  userNamesOfPlayersWhoHaveLeftTheGame: string[] = [];

  constructor(
      private signalrService: SignalrService,
      private router: Router,
      public dialog: MatDialog) {}

  ngOnInit(): void {
    this.signalrService.startConnection();

    const url = window.location.href;
    const params = url.split("/");
    let id = params[params.length - 1];

    if(this.signalrService.hubConnection
        && this.signalrService.hubConnection.state == HubConnectionState.Connected) {
            this.attachGameListeners();
            this.signalrService.loadGame(id);
            this.signalrService.getAllWildcardOptions();
    } else {
        //TODO: Handle slow connection/loading -> showing loading screen
        this.signalrService.hubConnectionStartPromise?.then( () => {
            this.attachGameListeners();
            this.signalrService.loadGame(id);
            this.signalrService.getAllWildcardOptions();
        })
    }

    this.loadCellViewDataByType();
    this.loadMockData(); //TODO: Remove this in production
  }

  attachGameListeners() {
    this.signalrService.hubConnection?.on("UpdateGameState", data => {      
        console.log(data);
        this.loadBoard(data.commonGameState.board);
        this.loadPlayerTiles(data.tiles);
        this.remainingTilesCount = data.commonGameState.remainingTilesCount;
        this.playerNameOnTurn = data.commonGameState.playerOnTurnUserName;
        this.currentUserName = data.myUserName; //Can be moved into localStorage
        this.isTileExchangePossible = data.commonGameState.isTileExchangePossible;
        this.loadScoreBoard(data.commonGameState.pointsByUserNames)
        this.updatedBoardCells = [];

        if(data.commonGameState.userNamesOfPlayersWhoHaveLeftTheGame) {
            this.userNamesOfPlayersWhoHaveLeftTheGame = data.commonGameState.userNamesOfPlayersWhoHaveLeftTheGame;
        }

        if(data.commonGameState.isGameOver == true) {
            this.router.navigate([this.router.url + "/summary"]);
        }
    })
  
    this.signalrService.hubConnection?.on("InvalidWriteWordInput", data => {
        console.log("Invalid Write Word Input");
        console.log(data);
        //TODO: Return all wildcards to their normal state
        let dialogData: ErrorDialogData = { message: Object.values(data.errorsByCodes).toString(), unexistingWords: null };
        if(data.unexistingWords || data.unexistingWords.length > 0) {
            dialogData.unexistingWords = data.unexistingWords;
        }
        this.dialog.open(ErrorDialog, { data: dialogData});
        for(let i = 0; i < this.updatedBoardCells.length; i++) {
            this.playerTiles.push(this.updatedBoardCells[i].key.tile)
            this.board[this.updatedBoardCells[i].value.row][this.updatedBoardCells[i].value.column].tile = null;
        }
        this.selectedBoardCell = null;
        this.updatedBoardCells = [];
    })

    this.signalrService.hubConnection?.on("InvalidExchangeTilesInput", data => {
        console.log(data);
        this.dialog.open(ErrorDialog, { data: { message: Object.values(data.errorsByCodes)}});
        this.showExchangeField = false;
        this.selectedExchangeTiles = [];
    })

    this.signalrService.hubConnection?.on("ImpossibleToSkipTurn", data => {
        console.log(data);
        this.dialog.open(ErrorDialog, { data: { message: Object.values(data.errorsByCodes)}});
    })

    this.signalrService.hubConnection?.on("ReceiveAllWildcardOptions", data => {
        console.log("Wildcard Options: ");
        console.log(data);
        this.wildcardOptions = data;
    })
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
    this.pointsByUserNames = [];
    for(let i = 0; i < pointsByUserNames.length; i++) {
      this.pointsByUserNames.push({key: pointsByUserNames[i].key, value: pointsByUserNames[i].value});
    }
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

  getClassNameByCell(cell: Cell) {
      let result = " ";

      result += this.getClassNameByCellType(cell.type);
      result += " " + this.getClassNameIfCellIsTaken(cell);
      result += " " + this.getClassNameIfSelected(cell);
      result += " " + this.getClassNameIfNewCell(cell);

      return result;
  }

  getClassNameIfNewCell(cell: Cell) {
      for(let i = 0; i < this.updatedBoardCells.length; i++) {
          if(this.updatedBoardCells[i].key == cell) {
              //TODO: think how to show that a cell is new
              //return "border border-info";
          }
      }
      return "";
  }

  getClassNameByCellType(type: number) {
    return this.cellViewDataByType.get(type)?.className;
  }
  
  getClassNameWhetherPlayerIsOnTurn(playerName: string) {
      return playerName == this.playerNameOnTurn ? "player-on-turn" : "";
  }

  modifyCurrentUserName(playerName: string) {
      let result = playerName;
      if(playerName == this.currentUserName) {
        result += " (????)"; 
        return result;
      } 
      
      if(this.userNamesOfPlayersWhoHaveLeftTheGame.find(x => x == playerName)) {
          result += " (????????????????)";
          return result;
      }

      return result;
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

  getValueWhenEmptyByCellType(type: number) {
    return this.cellViewDataByType.get(type)?.valueWhenEmpty;
  }

  isCurrentPlayerOnTurn() : boolean {
      return this.currentUserName == this.playerNameOnTurn;
  }

  clickOnPlayerTile(playerTile: Tile | any) {
      if(playerTile != null) {
        if(playerTile == this.selectedPlayerTile) {
            this.selectedPlayerTile = null;
            return;
        }
        //Check whether the player has the tile
        for(let i = 0; i < this.playerTiles.length; i++) {
          if(this.playerTiles[i] == playerTile) {
            this.selectedPlayerTile = playerTile;
            return;
          }
        }
      }
  }

  doubleClickOnPlayerTile(playerTile: Tile) {
    if((playerTile.letter == this.WILDCARD_SYMBOL
        || playerTile.points == 0)
        && this.playerTiles.find(item => item == playerTile)) {
        this.dialog.open(ChangeWildcardDialog, { data: { tiles: this.wildcardOptions, 
            tile: playerTile, writeWordInput: null}});
    }
  }

  leaveGame() {
      let dialogRef = this.dialog.open(LeaveGameDialog)
      dialogRef.afterClosed().subscribe(result => {
        if(result) {
            this.signalrService.leaveGame();
            this.router.navigate(["/"]);
        }
      })
  }

  clickExchangeBtn() {
    this.showExchangeField = !this.showExchangeField
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

  skipTurn() {
    this.signalrService.skipTurn();
  }

  getClassNameIfSelectedExchangeTile(tile: Tile) {
    for(let i = 0; i < this.selectedExchangeTiles.length; i++) {
        if(this.selectedExchangeTiles[i] == tile) {
            return "selected-tile";
        }
    }
    return "";
  }

  exchangeSelectedTiles() {
    this.signalrService.exchangeTiles(this.selectedExchangeTiles);
    this.showExchangeField = false;
    this.selectedExchangeTiles = [];
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

  doubleClickOnBoardCell(cell: Cell) {
    if((cell.tile?.letter == this.WILDCARD_SYMBOL
        || cell.tile?.points == 0)
        && this.updatedBoardCells.find(item => item.key == cell)) {
        this.dialog.open(ChangeWildcardDialog, { data: { tiles: this.wildcardOptions, 
            tile: cell.tile, writeWordInput: null}});
    }
  }  

  removeTileFromPlayerTiles(playerTile: Tile) {
      if(playerTile) {
        this.playerTiles = this.playerTiles.filter(item => item !== playerTile);
      }
  }

  removeCellFromUpdatedBoardCells(cell: Cell) {
      if(cell) {
        this.updatedBoardCells = this.updatedBoardCells.filter(item => item.key !== cell);
      }
  }

  addCellToUpdatedBoardCells(cell: Cell) {
    if(cell && cell.tile) {
        for(let i = 0; i < this.updatedBoardCells.length; i++) {
            if(this.updatedBoardCells[i] == cell) {
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
                this.updatedBoardCells.push({key: cell, value: {row: row, column: col}})
                return;
            }       
        }
    }
  }

  rightClickOnBoardCell(cell: Cell | any) {
    if(cell.tile && cell == this.selectedBoardCell) {
        this.playerTiles.push(cell.tile);
        this.removeCellFromUpdatedBoardCells(cell);
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
    if(this.updatedBoardCells.length <= 0) {
        return;
    }
    
    //Check for null tiles
    this.updatedBoardCells = this.updatedBoardCells.filter(item => item.key.tile !== null);
    console.log("WRITING WORD")
    console.log(this.updatedBoardCells);
    let writeWordInput = this.updatedBoardCells.map(item => ({key: item.key.tile, value: item.value}))

    if(!this.checkForWildcards(writeWordInput.map(item => (item.key)))) {
        try {
            this.signalrService.writeWord(writeWordInput);
        }
        catch (ex) {
            console.log("ERROR");
            console.log(ex);
        }
        return;
    }

    for(let i = writeWordInput.length - 1; i >= 0; i--) {
        if(writeWordInput[i].key.letter == this.WILDCARD_SYMBOL) {    
            this.dialog.open(ChangeWildcardDialog, { data: { tiles: this.wildcardOptions, 
                    tile: this.updatedBoardCells[i].key.tile, writeWordInput: writeWordInput}});
        }
    }
  }

  checkForWildcards(tiles: Tile[]) : boolean {
    for(let i = 0; i < tiles.length; i++) {
        if(tiles[i].letter == this.WILDCARD_SYMBOL) {
            return true;
        }
    }
    return false;
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
          "letter": "??",
          "points": 10
      },
      {
          "letter": "??",
          "points": 10
      },
      {
        "letter": "??",
        "points": 10
    },
      {
        "letter": "??",
        "points": 2
      },
      {
          "letter": "??",
          "points": 1
      },
      {
          "letter": "??",
          "points": 4
      },
      {
          "letter": "??",
          "points": 1
      },
  ]

    this.loadBoard(data.board);
    this.loadPlayerTiles(tiles);
    this.remainingTilesCount = data.remainingTilesCount;
    this.loadScoreBoard(data.pointsByUserNames)
    console.log("Tiles Count: " + this.remainingTilesCount)
  }
}

@Component({
    selector: 'error-dialog',
    templateUrl: 'error-dialog.html',
  })
export class ErrorDialog {

    constructor(public dialogRef: MatDialogRef<ErrorDialog>, 
                @Inject(MAT_DIALOG_DATA) public data: ErrorDialogData) {}
}

@Component({
    selector: 'leave-game-dialog',
    templateUrl: 'leave-game-dialog.html',
    styleUrls: ['./game.component.css']
  })
export class LeaveGameDialog {

    constructor(public dialogRef: MatDialogRef<ErrorDialog>, 
                @Inject(MAT_DIALOG_DATA) public data: ErrorDialogData) {}
}

@Component({
    selector: 'change-wildcard-dialog',
    templateUrl: 'change-wildcard-dialog.html',
    styleUrls: ['./game.component.css']
  })
export class ChangeWildcardDialog {

    selectedTile: Tile | null = null;
    tiles: any[] = new Array();
    WILDCARD_SYMBOL: string = "\u0000";

    constructor(public dialogRef: MatDialogRef<ChangeWildcardDialog>, 
                @Inject(MAT_DIALOG_DATA) public data: ChangeWildcardDialogData,
                public signalrService: SignalrService) {
                    this.tiles = data.tiles;
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
            if(tiles[i].key.letter == this.WILDCARD_SYMBOL) {
                return true;
            }
        }
        return false;
    }
}