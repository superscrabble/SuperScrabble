import { Tile } from 'src/app/models/tile';

export class Cell {
    type: number;
    tile: Tile | null;

    constructor(type: number, tile: Tile | null) {
        this.type = type;
        this.tile = tile;
    }
}