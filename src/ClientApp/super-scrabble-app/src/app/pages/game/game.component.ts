import { Component, OnInit, Inject, NgZone, Pipe, PipeTransform, EventEmitter, ElementRef, ViewChild } from '@angular/core';
import { SignalrService } from 'src/app/services/signalr.service';
import { Tile } from 'src/app/models/tile';
import { Cell } from 'src/app/models/cell';
import { CellViewData } from 'src/app/models/cellViewData';
import { HubConnection, HubConnectionState } from '@microsoft/signalr';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { Action } from 'src/app/models/action';
import { AppConfig } from 'src/app/app-config';
import { ChangeWildcardDialogComponent } from '../common/dialogs/change-wildcard-dialog/change-wildcard-dialog.component';
import { SettingsDialogComponent } from '../common/dialogs/settings-dialog/settings-dialog.component';
import { LeaveGameDialogComponent } from '../common/dialogs/leave-game-dialog/leave-game-dialog.component';
import { ErrorDialogComponent, ErrorDialogData } from '../common/dialogs/error-dialog/error-dialog.component';
import { GameContentDialogComponent } from '../common/dialogs/game-content-dialog/game-content-dialog.component';
import { ExchangeTilesDialogComponent } from '../common/dialogs/exchange-tiles-dialog/exchange-tiles-dialog.component';
import { GameService } from 'src/app/services/game.service';
import { CdkDragDrop, CdkDragEnter, moveItemInArray, transferArrayItem } from "@angular/cdk/drag-drop";
import { GameboardComponent } from '../common/gameboard/gameboard.component';
import { Team } from 'src/app/models/team';
import { Player } from 'src/app/models/player';
import { LoadingScreenService } from 'src/app/services/loading-screen.service';

@Pipe({
    name: "formatTime"
})
export class FormatTimePipe implements PipeTransform {
    transform(value: number): string {
    const minutes: number = Math.floor(value / 60);
    return (
        ("00" + minutes).slice(-2) +
        ":" +
        ("00" + Math.floor(value - minutes * 60)).slice(-2)
    );
    }
}

//FIXME
class Teammate {
    userName: string = "";
    tiles: Tile[] = [];
}

@Component({
selector: 'app-game',
templateUrl: './game.component.html',
styleUrls: ['./game.component.css']
})
export class GameComponent implements OnInit {

    //TODO: check which properties are not used
    //TODO: create BoardConfigurationProvider which will provide e.x special cells
    //TODO: create GameService for Join, Leave, ExchangeTiles, WriteWord, SkipTurn
    board: Cell[][] = new Array();
    playerTiles: Tile[] = new Array();
    teammates: Teammate[] = [];
    cellViewDataByType: Map<number, CellViewData> = new Map();
    remainingTilesCount: number = 90;
    teams: Team[] = new Array();
    selectedPlayerTile: Tile | null = null;
    updatedBoardCells: any[] = new Array();
    selectedBoardCell: Cell | null = null;
    playerNameOnTurn: string = "";
    currentUserName: string = "";
    showExchangeField: boolean = false;
    selectedExchangeTiles: Tile[] = new Array();
    isTileExchangePossible: boolean = true;
    wildcardOptions: Tile[] = new Array();
    userNamesOfPlayersWhoHaveLeftTheGame: string[] = [];
    turnRemainingTime: number = 100;
    gameTimeAsString: string = "01:30";
    gameLogs: Action[] = [];
    playerOnTurn: Player = new Player("", 0);
    //@ViewChild('boardComponent', {static: false}) boardComponent: GameboardComponent | undefined;

    constructor(
        private gameService: GameService,
        private signalrService: SignalrService,
        private router: Router,
        public dialog: MatDialog,
        private elementRef: ElementRef,
        private loadingScreenService: LoadingScreenService) {}

    ngOnInit(): void {
        this.loadingScreenService.showLoadingScreen();
        this.signalrService.startConnection();

        const url = window.location.href;
        const params = url.split("/");
        let id = params[params.length - 1];

        if(this.signalrService.hubConnection
            && this.signalrService.hubConnection.state == HubConnectionState.Connected) {
                this.attachGameListeners();
                this.signalrService.loadGame(id);
                this.signalrService.getAllWildcardOptions();
        } else {
            //TODO: Handle slow connection/loading -> showing loading screen
            this.signalrService.hubConnectionStartPromise?.then( () => {
                this.attachGameListeners();
                this.signalrService.loadGame(id);
                this.signalrService.getAllWildcardOptions();
            })
        }

        //this.loadCellViewDataByType();
        this.loadMockLogs();
        this.loadMockData(); //TODO: Remove this in production
        this.wildcardOptions = [
            {
                "letter": "Ь",
                "points": 10
            },
            {
                "letter": "Щ",
                "points": 10
            },
            {
            "letter": "Щ",
            "points": 10
            },
            {
            "letter": "Ш",
            "points": 2
            },
            {
                "letter": "А",
                "points": 1
            },
            {
                "letter": "З",
                "points": 4
            },
            {
                "letter": "Т",
                "points": 1
            },
            {
            "letter": "Т",
            "points": 1
            },
        ];
    }

    attachGameListeners() {
        this.signalrService.hubConnection?.on("UpdateGameState", data => {      
            console.log(data);
            this.loadBoard(data.commonGameState.board);
            this.loadPlayerTiles(data.tiles);
            this.remainingTilesCount = data.commonGameState.remainingTilesCount;
            this.playerNameOnTurn = data.commonGameState.playerOnTurnUserName;
            this.currentUserName = data.myUserName; //Can be moved into localStorage
            this.isTileExchangePossible = data.commonGameState.isTileExchangePossible;
            this.loadScoreBoard(data.commonGameState.teams)
            //TODO: remove updatedBoardCells from board, if there are some
            this.updatedBoardCells = [];

            if(data.commonGameState.userNamesOfPlayersWhoHaveLeftTheGame) {
                this.userNamesOfPlayersWhoHaveLeftTheGame = data.commonGameState.userNamesOfPlayersWhoHaveLeftTheGame;
            }

            this.teams.forEach(team => {
                team.players.forEach(player => {
                    if(player.userName == this.playerNameOnTurn) {
                        this.playerOnTurn = player;
                        return;
                    }
                })
            })

            if(data.commonGameState.remainingSecondsByUserNames) {
                console.log("Seconds remaining")
                console.log(data.commonGameState.remainingSecondsByUserNames);
                this.parseRemainingSeconds(data.commonGameState.remainingSecondsByUserNames);
                console.log(this.teams[0].players[0].remainingSeconds);
            }

            if(data.commonGameState.isGameOver == true) {
                this.router.navigate([this.router.url + "/summary"]);
            }
            this.loadingScreenService.stopShowingLoadingScreen();
        })
    
        this.signalrService.hubConnection?.on("InvalidWriteWordInput", data => {
            console.log("Invalid Write Word Input");
            console.log(data);
            //TODO: Return all wildcards to their normal state
            let dialogData: ErrorDialogData = { message: Object.values(data.errorsByCodes).toString(), unexistingWords: null };
            //TODO: Change || to &&
            if(data.unexistingWords || data.unexistingWords.length > 0) {
                dialogData.unexistingWords = data.unexistingWords;
            }
            this.dialog.open(ErrorDialogComponent, { data: dialogData});
            /*for(let i = 0; i < this.updatedBoardCells.length; i++) {
                this.playerTiles.push(this.updatedBoardCells[i].key.tile)
                this.board[this.updatedBoardCells[i].value.row][this.updatedBoardCells[i].value.column].tile = null;
            }*/
            this.selectedBoardCell = null;
            //this.updatedBoardCells = [];
        })

        this.signalrService.hubConnection?.on("InvalidExchangeTilesInput", data => {
            console.log(data);
            this.dialog.open(ErrorDialogComponent, { data: { message: Object.values(data.errorsByCodes)}});
            this.showExchangeField = false;
            this.selectedExchangeTiles = [];
        })

        this.signalrService.hubConnection?.on("ImpossibleToSkipTurn", data => {
            console.log(data);
            this.dialog.open(ErrorDialogComponent, { data: { message: Object.values(data.errorsByCodes)}});
        })

        this.signalrService.hubConnection?.on("ReceiveAllWildcardOptions", data => {
            console.log("Wildcard Options: ");
            console.log(data);
            this.wildcardOptions = data;
        })

        this.signalrService.hubConnection?.on("UpdateGameTimer", data => {
            const fillString: string = "0";
            const maxLength: number = 2;
            let minutes: string = data.minutes.toString().padStart(maxLength, fillString);
            let seconds: string = data.seconds.toString().padStart(maxLength, fillString);
            this.gameTimeAsString = `${minutes}:${seconds}`;
            this.playerOnTurn.remainingSeconds = data.minutes * 60 + data.seconds;
        });

        this.signalrService.hubConnection?.on("UserEnteredGameFromAnotherConnectionId", () => {
            this.router.navigateByUrl("/");
        });
    }

    loadBoard(board: any): void {
        this.board = new Array(board.height).fill(false).map(() => new Array(board.width));
        
        for(let i = 0; i < board.cells.length; i++) {
            let cell = board.cells[i];
            let tile = cell.tile ? new Tile(cell.tile.letter, cell.tile.points) : null;
            this.board[cell.position.row][cell.position.column] = new Cell(cell.type, tile);
        }
    }

    loadPlayerTiles(playerTiles: any): void {
        console.log(playerTiles);
        this.playerTiles = new Array(playerTiles.length);
        for(let i = 0; i < playerTiles.length; i++) {
        let playerTile = playerTiles[i];
        this.playerTiles[i] = new Tile(playerTile.letter, playerTile.points);
        }
    }

    loadScoreBoard(teams: any): void {
        console.log("Beginning");
        console.log(teams)
        this.teams = [];
        for(let i = 0; i < teams.length; i++) {
            this.teams.push(this.loadTeam(teams[i].players));
        }
        this.teams.sort((a : Team, b : Team) => {
            return b.points - a.points;
        });
        console.log("TEAMS");
        console.log(this.teams);
    }

    loadTeam(players: any): Team {
        let team: Team = new Team();
        for(let i = 0; i < players.length; i++) {
            team.players.push(new Player(players[i].userName, players[i].points));
        }

        team.calculatePoints();
        team.players.sort((a : Player, b : Player) => {
            return b.points - a.points;
        })

        return team;
    }

    parseRemainingSeconds(secondsRemainingByUserNames: any) {
        this.teams.forEach( team => {
            team.players.forEach(player => {
                player.remainingSeconds = secondsRemainingByUserNames[player.userName];
            })
        })
    }

    /*
    getClassNameIfNewCell(cell: Cell) {
        for(let i = 0; i < this.updatedBoardCells.length; i++) {
            if(this.updatedBoardCells[i].key == cell) {
                //TODO: think how to show that a cell is new
                //return "border border-info";
            }
        }
        return "";
    }

    getClassNameByCellType(type: number) {
        return this.cellViewDataByType.get(type)?.className;
    }*/

    //Useless
    removeTileFromBoard(tile: Tile) {
        console.log("Remove tile from Board")
        //this.boardComponent?.removeTileFromBoard(tile);
    }

    /*drop(event: CdkDragDrop<Tile[]>) {
        console.log("DROPPING IN PLAYER TILES")
        console.log(event.previousIndex + " " + event.previousContainer.data + " " + event.currentIndex);
        if(event.previousContainer === event.container)
            moveItemInArray(this.playerTiles, event.previousIndex, event.currentIndex);
        else {
            transferArrayItem(event.previousContainer.data, event.container.data, event.previousIndex, event.currentIndex);
        }
    }

    customPlayerTiles: Tile[] = [];

    customDrop(event: CdkDragDrop<Tile[]>) {
        console.log("In Custom Drop")
        console.log(event.previousContainer);
        console.log(event.previousContainer.data)
        //this.customPlayerTiles.push(event.item.data);
        //this.customPlayerTiles.push(new Tile("", 2));
        transferArrayItem(event.previousContainer.data,
                            this.customPlayerTiles,
                            event.previousIndex,
                            this.customPlayerTiles.length - 1);
        console.log(this.customPlayerTiles);
    }

    show: boolean = false;

    onDropEntered(event: CdkDragEnter<Tile[]>) {
        if(document.getElementById("playerTilePreview")) {
            //document.getElementById("playerTilePreview")!.style.display = 'none'
            this.show = true;
        }
    }

    showEmptyPlaceholder() {
        return this.show;
    }

    createPlayerTilePreview() {
        return document.createElement('div');
    }*/

    isCurrentPlayerOnTurn() : boolean {
        return this.currentUserName == this.playerNameOnTurn;
    }

    /*clickOnPlayerTile(playerTile: Tile | any) {
        if(!this.isCurrentPlayerOnTurn()) return;

        if(playerTile != null) {
            if(playerTile == this.selectedPlayerTile) {
                this.selectedPlayerTile = null;
                return;
            }   
            //Check whether the player has the tile
            for(let i = 0; i < this.playerTiles.length; i++) {
            if(this.playerTiles[i] == playerTile) {
                this.selectedPlayerTile = playerTile;
                return;
            }
            }
        }
    }

    doubleClickOnPlayerTile(playerTile: Tile) {
        if((playerTile.letter == AppConfig.WildcardSymbol
            || playerTile.points == 0)
            && this.playerTiles.find(item => item == playerTile)) {
            this.dialog.open(ChangeWildcardDialogComponent, { data: { tiles: this.wildcardOptions, 
                tile: playerTile, writeWordInput: null}});
        }
    }*/

    leaveGame() {
        let dialogRef = this.dialog.open(LeaveGameDialogComponent)
        dialogRef.afterClosed().subscribe(result => {
            if(result) {
                this.signalrService.leaveGame();
                this.router.navigate(["/"]);
            }
        })
    }

    //TODO: declare type for writeWordInput
    openWildcardDialogEvent(input: {tile: Tile, writeWordInput: any[]}) : void {
        console.log("Opening Chnage Wildcard Dialog");
        console.log(input);
        console.log(this.wildcardOptions);
        this.dialog.open(ChangeWildcardDialogComponent, { data: { tiles: this.wildcardOptions, 
            tile: input.tile, writeWordInput: input.writeWordInput}});
    }

    clickExchangeBtn() {
        this.showExchangeField = !this.showExchangeField
    }

    clickOnExchangeTile(tile: Tile) {
        if(tile) {
            //Deselecting
            for(let i = 0; i < this.selectedExchangeTiles.length; i++) {
                if(this.selectedExchangeTiles[i] == tile) {
                    this.selectedExchangeTiles = this.selectedExchangeTiles.filter(item => item !== tile)
                    return;
                }
            }
            this.selectedExchangeTiles.push(tile);
        }
    }

    skipTurn() {
        this.signalrService.skipTurn();
    }

    getClassNameIfSelectedExchangeTile(tile: Tile) {
        for(let i = 0; i < this.selectedExchangeTiles.length; i++) {
            if(this.selectedExchangeTiles[i] == tile) {
                return "selected-tile";
            }
        }
        return "";
    }

    exchangeSelectedTiles() {
        this.signalrService.exchangeTiles(this.selectedExchangeTiles);
        this.showExchangeField = false;
        this.selectedExchangeTiles = [];
    }

    //TODO: rename here and in the gameboard component to onTileOutside
    addTileToPlayerTiles(tile: Tile) : void {
        this.playerTiles.push(tile);
    }

    //TODO: rename here and in the gameboard component to onTilePlaced
    removeTileFromPlayerTiles(playerTile: Tile) {
        if(playerTile) {
            this.playerTiles = this.playerTiles.filter(item => item !== playerTile);
        }
    }

    /*getClassNameIfSelected(object: Tile | Cell | any) {
        if(object instanceof Tile) {
            if(this.selectedPlayerTile && this.selectedPlayerTile == object) {
                return "selected-tile";
            }
        } else if(object instanceof Cell) {
            if(this.selectedBoardCell && this.selectedBoardCell == object) {
                return "selected-cell";
            }
        }
        return "";
    }*/

    showWordMeaningOf(word: string) : void {
        console.log("Show word meaning")
    }

    checkForWildcards(tiles: Tile[]) : boolean {
        for(let i = 0; i < tiles.length; i++) {
            if(tiles[i].letter == AppConfig.WildcardSymbol) {
                return true;
            }
        }
        return false;
    }

    updatedBoardCellsChange(cells: Array<{ cell: Cell, coordinates: { column: number, row: number } }>) {
        console.log("ON UPDATED BOARD CELLS CHANGE")
        console.log(cells);
        this.updatedBoardCells = cells;
    }

    writeWord() : void {     
        if(this.updatedBoardCells.length <= 0) {
            return;
        }

        console.log("Before Anything in Writing Word");
        console.log(this.updatedBoardCells);
        
        //Check for null tiles
        this.updatedBoardCells = this.updatedBoardCells.filter(item => item.cell.tile !== null);
        console.log("WRITING WORD")
        console.log(this.updatedBoardCells);
        let writeWordInput = this.updatedBoardCells.map(item => ({key: item.cell.tile, value: item.coordinates}))

        if(!this.checkForWildcards(writeWordInput.map(item => (item.key)))) {
            try {
                //this.signalrService.writeWord(writeWordInput);
                this.gameService.writeWord(writeWordInput);
            }
            catch (ex) {
                console.log("ERROR");
                console.log(ex);
            }
            return;
        }

        for(let i = writeWordInput.length - 1; i >= 0; i--) {
            if(writeWordInput[i].key.letter == AppConfig.WildcardSymbol) {    
                this.dialog.open(ChangeWildcardDialogComponent, { data: { tiles: this.wildcardOptions, 
                        tile: this.updatedBoardCells[i].key.tile, writeWordInput: writeWordInput}});
            }
        }
    }

    openSettings() {
        this.dialog.open(SettingsDialogComponent, { data: {
            openLeaveGameDialog: this.leaveGame
        }});
    }

    openGameContentMenu() {
        this.dialog.open(GameContentDialogComponent, {
            data: {
                teams: this.teams,
                userNamesOfPlayersWhoHaveLeftTheGame: this.userNamesOfPlayersWhoHaveLeftTheGame,
                currentUserName: this.currentUserName,
                gameLogs: this.gameLogs,
                isDuoGame: this.isDuoGame(),
                showWordMeaningOf: this.showWordMeaningOf
            }
        });
    }

    openExchangeTilesDialog() {
        this.dialog.open(ExchangeTilesDialogComponent, {
            data: {
                playerTiles: this.playerTiles
            }
        })
    }

    areThereNewPlacedTiles() : boolean {
        return this.updatedBoardCells.length > 0;
    }

    isDuoGame() : boolean {
        //TODO: check if GameMode is duo
        return false;
    }

    loadMockLogs() {
        for(let i = 0; i < 10; i++) {
            this.gameLogs.push(new Action("Иван написа ", "здраве"));
        }
    }

    loadMockData(): void {
        let data = 
        {
        "remainingTilesCount": 88,
        "teams": [
            {
                "players": [
                    {
                        "userName": "Gosho2",
                        "points": 20
                    }
                ]
            },
            {
                "players": [
                    {
                        "userName": "Gosho1Gosho1Go",
                        "points": 1000
                    },
                    /*{
                        "userName": "Gosho1",
                        "points": 20
                    },
                    {
                        "userName": "Gosho1",
                        "points": 30
                    }*/
                ]
            },
            {
                "players": [
                    {
                        "userName": "Gosho1Gosho2",
                        "points": 990
                    }
                ]
            },
            {
                "players": [
                    {
                        "userName": "Gosho1",
                        "points": 10
                    }
                ]
            }
        ],
        "pointsByUserNames": [
            {
                "key": "Gosho2",
                "value": 0
            },
            {
                "key": "Gosho",
                "value": 0
            },
            {
                "key": "Gosho5",
                "value": 0
            },
            {
                "key": "Gosho6",
                "value": 0
            },
        ],
        "board": {
            "cells": [
                {
                    "position": {
                        "row": 0,
                        "column": 0
                    },
                    "type": 5,
                    "tile": null
                },
                {
                    "position": {
                        "row": 0,
                        "column": 1
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 0,
                        "column": 2
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 0,
                        "column": 3
                    },
                    "type": 2,
                    "tile": null
                },
                {
                    "position": {
                        "row": 0,
                        "column": 4
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 0,
                        "column": 5
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 0,
                        "column": 6
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 0,
                        "column": 7
                    },
                    "type": 5,
                    "tile": null
                },
                {
                    "position": {
                        "row": 0,
                        "column": 8
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 0,
                        "column": 9
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 0,
                        "column": 10
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 0,
                        "column": 11
                    },
                    "type": 2,
                    "tile": null
                },
                {
                    "position": {
                        "row": 0,
                        "column": 12
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 0,
                        "column": 13
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 0,
                        "column": 14
                    },
                    "type": 5,
                    "tile": null
                },
                {
                    "position": {
                        "row": 1,
                        "column": 0
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 1,
                        "column": 1
                    },
                    "type": 4,
                    "tile": null
                },
                {
                    "position": {
                        "row": 1,
                        "column": 2
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 1,
                        "column": 3
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 1,
                        "column": 4
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 1,
                        "column": 5
                    },
                    "type": 3,
                    "tile": null
                },
                {
                    "position": {
                        "row": 1,
                        "column": 6
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 1,
                        "column": 7
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 1,
                        "column": 8
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 1,
                        "column": 9
                    },
                    "type": 3,
                    "tile": null
                },
                {
                    "position": {
                        "row": 1,
                        "column": 10
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 1,
                        "column": 11
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 1,
                        "column": 12
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 1,
                        "column": 13
                    },
                    "type": 4,
                    "tile": null
                },
                {
                    "position": {
                        "row": 1,
                        "column": 14
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 2,
                        "column": 0
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 2,
                        "column": 1
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 2,
                        "column": 2
                    },
                    "type": 4,
                    "tile": null
                },
                {
                    "position": {
                        "row": 2,
                        "column": 3
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 2,
                        "column": 4
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 2,
                        "column": 5
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 2,
                        "column": 6
                    },
                    "type": 2,
                    "tile": null
                },
                {
                    "position": {
                        "row": 2,
                        "column": 7
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 2,
                        "column": 8
                    },
                    "type": 2,
                    "tile": null
                },
                {
                    "position": {
                        "row": 2,
                        "column": 9
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 2,
                        "column": 10
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 2,
                        "column": 11
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 2,
                        "column": 12
                    },
                    "type": 4,
                    "tile": null
                },
                {
                    "position": {
                        "row": 2,
                        "column": 13
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 2,
                        "column": 14
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 3,
                        "column": 0
                    },
                    "type": 2,
                    "tile": null
                },
                {
                    "position": {
                        "row": 3,
                        "column": 1
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 3,
                        "column": 2
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 3,
                        "column": 3
                    },
                    "type": 4,
                    "tile": null
                },
                {
                    "position": {
                        "row": 3,
                        "column": 4
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 3,
                        "column": 5
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 3,
                        "column": 6
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 3,
                        "column": 7
                    },
                    "type": 2,
                    "tile": null
                },
                {
                    "position": {
                        "row": 3,
                        "column": 8
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 3,
                        "column": 9
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 3,
                        "column": 10
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 3,
                        "column": 11
                    },
                    "type": 4,
                    "tile": null
                },
                {
                    "position": {
                        "row": 3,
                        "column": 12
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 3,
                        "column": 13
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 3,
                        "column": 14
                    },
                    "type": 2,
                    "tile": null
                },
                {
                    "position": {
                        "row": 4,
                        "column": 0
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 4,
                        "column": 1
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 4,
                        "column": 2
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 4,
                        "column": 3
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 4,
                        "column": 4
                    },
                    "type": 4,
                    "tile": null
                },
                {
                    "position": {
                        "row": 4,
                        "column": 5
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 4,
                        "column": 6
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 4,
                        "column": 7
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 4,
                        "column": 8
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 4,
                        "column": 9
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 4,
                        "column": 10
                    },
                    "type": 4,
                    "tile": null
                },
                {
                    "position": {
                        "row": 4,
                        "column": 11
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 4,
                        "column": 12
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 4,
                        "column": 13
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 4,
                        "column": 14
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 5,
                        "column": 0
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 5,
                        "column": 1
                    },
                    "type": 3,
                    "tile": null
                },
                {
                    "position": {
                        "row": 5,
                        "column": 2
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 5,
                        "column": 3
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 5,
                        "column": 4
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 5,
                        "column": 5
                    },
                    "type": 3,
                    "tile": null
                },
                {
                    "position": {
                        "row": 5,
                        "column": 6
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 5,
                        "column": 7
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 5,
                        "column": 8
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 5,
                        "column": 9
                    },
                    "type": 3,
                    "tile": null
                },
                {
                    "position": {
                        "row": 5,
                        "column": 10
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 5,
                        "column": 11
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 5,
                        "column": 12
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 5,
                        "column": 13
                    },
                    "type": 3,
                    "tile": null
                },
                {
                    "position": {
                        "row": 5,
                        "column": 14
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 6,
                        "column": 0
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 6,
                        "column": 1
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 6,
                        "column": 2
                    },
                    "type": 2,
                    "tile": null
                },
                {
                    "position": {
                        "row": 6,
                        "column": 3
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 6,
                        "column": 4
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 6,
                        "column": 5
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 6,
                        "column": 6
                    },
                    "type": 2,
                    "tile": null
                },
                {
                    "position": {
                        "row": 6,
                        "column": 7
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 6,
                        "column": 8
                    },
                    "type": 2,
                    "tile": null
                },
                {
                    "position": {
                        "row": 6,
                        "column": 9
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 6,
                        "column": 10
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 6,
                        "column": 11
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 6,
                        "column": 12
                    },
                    "type": 2,
                    "tile": null
                },
                {
                    "position": {
                        "row": 6,
                        "column": 13
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 6,
                        "column": 14
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 7,
                        "column": 0
                    },
                    "type": 5,
                    "tile": null
                },
                {
                    "position": {
                        "row": 7,
                        "column": 1
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 7,
                        "column": 2
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 7,
                        "column": 3
                    },
                    "type": 2,
                    "tile": null
                },
                {
                    "position": {
                        "row": 7,
                        "column": 4
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 7,
                        "column": 5
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 7,
                        "column": 6
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 7,
                        "column": 7
                    },
                    "type": 1,
                    "tile": null
                },
                {
                    "position": {
                        "row": 7,
                        "column": 8
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 7,
                        "column": 9
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 7,
                        "column": 10
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 7,
                        "column": 11
                    },
                    "type": 2,
                    "tile": null
                },
                {
                    "position": {
                        "row": 7,
                        "column": 12
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 7,
                        "column": 13
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 7,
                        "column": 14
                    },
                    "type": 5,
                    "tile": null
                },
                {
                    "position": {
                        "row": 8,
                        "column": 0
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 8,
                        "column": 1
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 8,
                        "column": 2
                    },
                    "type": 2,
                    "tile": null
                },
                {
                    "position": {
                        "row": 8,
                        "column": 3
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 8,
                        "column": 4
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 8,
                        "column": 5
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 8,
                        "column": 6
                    },
                    "type": 2,
                    "tile": null
                },
                {
                    "position": {
                        "row": 8,
                        "column": 7
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 8,
                        "column": 8
                    },
                    "type": 2,
                    "tile": null
                },
                {
                    "position": {
                        "row": 8,
                        "column": 9
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 8,
                        "column": 10
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 8,
                        "column": 11
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 8,
                        "column": 12
                    },
                    "type": 2,
                    "tile": null
                },
                {
                    "position": {
                        "row": 8,
                        "column": 13
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 8,
                        "column": 14
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 9,
                        "column": 0
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 9,
                        "column": 1
                    },
                    "type": 3,
                    "tile": null
                },
                {
                    "position": {
                        "row": 9,
                        "column": 2
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 9,
                        "column": 3
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 9,
                        "column": 4
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 9,
                        "column": 5
                    },
                    "type": 3,
                    "tile": null
                },
                {
                    "position": {
                        "row": 9,
                        "column": 6
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 9,
                        "column": 7
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 9,
                        "column": 8
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 9,
                        "column": 9
                    },
                    "type": 3,
                    "tile": null
                },
                {
                    "position": {
                        "row": 9,
                        "column": 10
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 9,
                        "column": 11
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 9,
                        "column": 12
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 9,
                        "column": 13
                    },
                    "type": 3,
                    "tile": null
                },
                {
                    "position": {
                        "row": 9,
                        "column": 14
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 10,
                        "column": 0
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 10,
                        "column": 1
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 10,
                        "column": 2
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 10,
                        "column": 3
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 10,
                        "column": 4
                    },
                    "type": 4,
                    "tile": null
                },
                {
                    "position": {
                        "row": 10,
                        "column": 5
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 10,
                        "column": 6
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 10,
                        "column": 7
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 10,
                        "column": 8
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 10,
                        "column": 9
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 10,
                        "column": 10
                    },
                    "type": 4,
                    "tile": null
                },
                {
                    "position": {
                        "row": 10,
                        "column": 11
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 10,
                        "column": 12
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 10,
                        "column": 13
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 10,
                        "column": 14
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 11,
                        "column": 0
                    },
                    "type": 2,
                    "tile": null
                },
                {
                    "position": {
                        "row": 11,
                        "column": 1
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 11,
                        "column": 2
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 11,
                        "column": 3
                    },
                    "type": 4,
                    "tile": null
                },
                {
                    "position": {
                        "row": 11,
                        "column": 4
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 11,
                        "column": 5
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 11,
                        "column": 6
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 11,
                        "column": 7
                    },
                    "type": 2,
                    "tile": null
                },
                {
                    "position": {
                        "row": 11,
                        "column": 8
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 11,
                        "column": 9
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 11,
                        "column": 10
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 11,
                        "column": 11
                    },
                    "type": 4,
                    "tile": null
                },
                {
                    "position": {
                        "row": 11,
                        "column": 12
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 11,
                        "column": 13
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 11,
                        "column": 14
                    },
                    "type": 2,
                    "tile": null
                },
                {
                    "position": {
                        "row": 12,
                        "column": 0
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 12,
                        "column": 1
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 12,
                        "column": 2
                    },
                    "type": 4,
                    "tile": null
                },
                {
                    "position": {
                        "row": 12,
                        "column": 3
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 12,
                        "column": 4
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 12,
                        "column": 5
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 12,
                        "column": 6
                    },
                    "type": 2,
                    "tile": null
                },
                {
                    "position": {
                        "row": 12,
                        "column": 7
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 12,
                        "column": 8
                    },
                    "type": 2,
                    "tile": null
                },
                {
                    "position": {
                        "row": 12,
                        "column": 9
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 12,
                        "column": 10
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 12,
                        "column": 11
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 12,
                        "column": 12
                    },
                    "type": 4,
                    "tile": null
                },
                {
                    "position": {
                        "row": 12,
                        "column": 13
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 12,
                        "column": 14
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 13,
                        "column": 0
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 13,
                        "column": 1
                    },
                    "type": 4,
                    "tile": null
                },
                {
                    "position": {
                        "row": 13,
                        "column": 2
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 13,
                        "column": 3
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 13,
                        "column": 4
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 13,
                        "column": 5
                    },
                    "type": 3,
                    "tile": null
                },
                {
                    "position": {
                        "row": 13,
                        "column": 6
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 13,
                        "column": 7
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 13,
                        "column": 8
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 13,
                        "column": 9
                    },
                    "type": 3,
                    "tile": null
                },
                {
                    "position": {
                        "row": 13,
                        "column": 10
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 13,
                        "column": 11
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 13,
                        "column": 12
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 13,
                        "column": 13
                    },
                    "type": 4,
                    "tile": null
                },
                {
                    "position": {
                        "row": 13,
                        "column": 14
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 14,
                        "column": 0
                    },
                    "type": 5,
                    "tile": null
                },
                {
                    "position": {
                        "row": 14,
                        "column": 1
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 14,
                        "column": 2
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 14,
                        "column": 3
                    },
                    "type": 2,
                    "tile": null
                },
                {
                    "position": {
                        "row": 14,
                        "column": 4
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 14,
                        "column": 5
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 14,
                        "column": 6
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 14,
                        "column": 7
                    },
                    "type": 5,
                    "tile": null
                },
                {
                    "position": {
                        "row": 14,
                        "column": 8
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 14,
                        "column": 9
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 14,
                        "column": 10
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 14,
                        "column": 11
                    },
                    "type": 2,
                    "tile": null
                },
                {
                    "position": {
                        "row": 14,
                        "column": 12
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 14,
                        "column": 13
                    },
                    "type": 0,
                    "tile": null
                },
                {
                    "position": {
                        "row": 14,
                        "column": 14
                    },
                    "type": 5,
                    "tile": null
                }
            ],
            "width": 15,
            "height": 15
        }
        }
        let tiles =
        [
        {
            "letter": "Ь",
            "points": 10
        },
        {
            "letter": "Щ",
            "points": 10
        },
        {
            "letter": "Щ",
            "points": 10
        },
        {
            "letter": "Ш",
            "points": 2
        },
        {
            "letter": "А",
            "points": 1
        },
        {
            "letter": "З",
            "points": 4
        },
        {
            "letter": "Т",
            "points": 1
        },
        {
            "letter": "Т",
            "points": 1
        },
        {
            "letter": AppConfig.WildcardSymbol,
            "points": 0
        },
    ]

        this.loadBoard(data.board);
        this.loadPlayerTiles(tiles);
        this.remainingTilesCount = data.remainingTilesCount;
        this.loadScoreBoard(data.teams)
        console.log("Tiles Count: " + this.remainingTilesCount)
        
        this.parseRemainingSeconds([
            {
                "key": "Gosho",
                "value": 60
            },
            {
                "key": "Alex",
                "value": 60
            }
        ]);
        /*this.teammates = [
            {
                userName: "Gosho",
                tiles: this.playerTiles
            }
        ]*/
    }
}