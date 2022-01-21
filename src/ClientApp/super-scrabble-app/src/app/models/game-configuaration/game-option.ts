export class GameOption {
    //FIXME: title = value; remove hint
    title: string = "";
    description: string = "";
    value: number = 0;
    hint: string = "";
    backgroundColorHex: string = "";
    //image: string

    constructor(title: string, description: string, value: number, hint: string) {
        this.title = title;
        this.description = description;
        this.value = value;
        this.hint = hint;
    }
}