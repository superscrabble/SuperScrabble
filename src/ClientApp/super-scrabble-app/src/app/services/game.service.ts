import { Injectable } from '@angular/core';
import { Tile } from '../models/tile';
import { SignalrService } from './signalr.service';

@Injectable({
  providedIn: 'root'
})
export class GameService {

  constructor(private signalrService: SignalrService) { }

  joinRoom() {

  }

  leaveQueue() {

  }

  loadGame() {

  }

  public writeWord(cells: any[]) {
    this.signalrService.writeWord(cells);
  }

  skipTurn() {
    this.signalrService.skipTurn();
  }

  exchangeTiles(tiles: Tile[]) {
    this.signalrService.exchangeTiles(tiles);
  }
}
