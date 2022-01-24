import { GameOption } from "./game-option";

export class GameConfig {
    title: string = "";
    gameOptions: GameOption[] = [];
    //TODO: change name
    inputPropName: string = "";
    //FIXME: change with enum
    isAboutTeamCount: boolean = false;
    
    //image: string
    //background color
    //selectedOption
    onSelectingOption: Function = () => {};
    
    constructor(title: string, gameOptions: GameOption[], onSelectinOption: Function) {
        this.title = title;
        this.gameOptions = gameOptions;
        this.onSelectingOption = onSelectinOption;
    }

    public selectOption(option: GameOption) : void {
        this.onSelectingOption(option);
    }
}