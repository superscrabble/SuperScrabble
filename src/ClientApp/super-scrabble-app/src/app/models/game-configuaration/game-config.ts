import { GameOption } from "./game-option";

export class GameConfig {
    title: string = "";
    gameOptions: GameOption[] = [];
    //TODO: change name
    inputPropName: string = "";
    //image: string
    //background color
    //selectedOption

    constructor(title: string) {
        this.title = title;
    }
}