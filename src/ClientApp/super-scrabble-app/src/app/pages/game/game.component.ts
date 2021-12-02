import { Component, OnInit } from '@angular/core';
import { SignalrService } from 'src/app/services/signalr.service';
import { Tile } from 'src/app/models/tile';
import { Cell } from 'src/app/models/cell';

@Component({
  selector: 'app-game',
  templateUrl: './game.component.html',
  styleUrls: ['./game.component.css']
})
export class GameComponent implements OnInit {

  board: Cell[][] = new Array();
  playerTiles: Tile[] = new Array();

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

    this.playerTiles = new Array(5).fill(new Tile('A', 2));
  }

  loadBoard(board: any): void {
    this.board = new Array(board.height).fill(false).map(() => new Array(board.width));
    
    for(let i = 0; i < board.cells.length; i++) {
      let cell = board.cells[i];
      this.board[cell.position.row][cell.position.column] = new Cell(cell.type.toString(), new Tile('G', 2))
    }
  }
}
