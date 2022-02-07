export class Player {
    userName: string = "";
    points: number = 0;

    constructor(userName: string, points: number) {
        this.userName = userName;
        this.points = points;
    }
}