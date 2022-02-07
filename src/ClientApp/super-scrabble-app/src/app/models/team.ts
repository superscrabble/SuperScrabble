import { Player } from "./player";

export class Team {
    players: Player[] = [];
    points: number = 0;

    calculatePoints() {
        this.points = 0;
        this.players.forEach(player => {
            this.points += player.points;
        });
    }
}