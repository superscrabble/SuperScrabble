import { Component, OnInit } from '@angular/core';
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

  constructor(private signalrService: SignalrService) {}

  ngOnInit(): void {
    //verify connection presence
    this.signalrService.startConnection();
    //this.signalrService.addInGameListeners();

    const url = window.location.href;
    const params = url.split("/");
    let id = params[params.length - 1];

    this.signalrService.hubConnection?.on("UpdateGameState", (data) => {      
      this.loadBoard(data.commonGameState.board);
    })

    this.signalrService.loadGame(id);
    this.loadCellViewDataByType();

    this.playerTiles = new Array(5).fill(new Tile('A', 2));
  }

  loadBoard(board: any): void {
    this.board = new Array(board.height).fill(false).map(() => new Array(board.width));
    
    for(let i = 0; i < board.cells.length; i++) {
      let cell = board.cells[i];
      let tile = cell.tile ? new Tile(cell.tile.letter, cell.tile.points) : null;
      this.board[cell.position.row][cell.position.column] = new Cell(cell.type, tile);
    }
  }

  loadCellViewDataByType() {
    this.cellViewDataByType = new Map([
      [0, new CellViewData("basic-cell", " ")],
      [1, new CellViewData("center-cell", "")],
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
}
