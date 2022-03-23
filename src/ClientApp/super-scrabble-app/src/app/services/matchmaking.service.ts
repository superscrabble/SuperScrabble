import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { GameMode } from '../models/enums/game-mode';
import { PartyType } from '../models/enums/party-type';
import { SignalrService } from './signalr.service';

@Injectable({
  providedIn: 'root'
})
export class MatchmakingService {

  constructor(private signalrService: SignalrService) {}

  joinRoom(gameMode: GameMode): void {
    this.signalrService.joinRoom(gameMode);
  }

  createParty(type: PartyType) {
    this.signalrService.createParty(type);
  }

  joinParty(code: string) {
    this.signalrService.joinParty(code);
  }

  joinRandomDuoGame() {
    this.signalrService.joinRandomDuoGame();
  }

  startGameFromParty(partyId: string) {
    this.signalrService.startGameFromParty(partyId);
  }
}
