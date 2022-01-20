import { GameOption } from "./game-option";

export class GameConfig {
    title: string = "";
    gameOptions: GameOption[] = [];
    //image: string
    //background color

    constructor(title: string) {
        this.title = title;
    }
}