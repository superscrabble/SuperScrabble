export class CellViewData {
    className: string;
    valueWhenEmpty: string;
    hint?: string;

    constructor(className: string, valueWhenEmpty: string) {
        this.className = className;
        this.valueWhenEmpty = valueWhenEmpty;
    }
}