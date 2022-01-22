export class GameOption {
    //FIXME: title = value; remove hint
    title: string = "";
    description: string = "";
    value: number = 0;
    backgroundColorHex: string = "";
    isSelected: boolean = false;
    //image: string

    constructor(title: string, description: string, value: number) {
        this.title = title;
        this.description = description;
        this.value = value;
    }
}