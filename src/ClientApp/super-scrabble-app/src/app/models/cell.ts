import { Tile } from 'src/app/models/tile';

export class Cell {
    type: string;
    tile: Tile;

    constructor(type: string, tile: Tile) {
        this.type = type;
        this.tile = tile;
    }
}