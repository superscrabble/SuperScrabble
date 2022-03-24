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
import { ChangeWildcardDialogComponent } from '../../dialogs/change-wildcard-dialog/change-wildcard-dialog.component';
import { SettingsDialogComponent } from '../../dialogs/settings-dialog/settings-dialog.component';
import { LeaveGameDialogComponent } from '../../dialogs/leave-game-dialog/leave-game-dialog.component';
import { ErrorDialogComponent, ErrorDialogData } from '../../dialogs/error-dialog/error-dialog.component';
import { GameContentDialogComponent } from '../../dialogs/game-content-dialog/game-content-dialog.component';
import { ExchangeTilesDialogComponent } from '../../dialogs/exchange-tiles-dialog/exchange-tiles-dialog.component';
import { GameService } from 'src/app/services/game.service';
import { CdkDragDrop, CdkDragEnter, moveItemInArray, transferArrayItem } from "@angular/cdk/drag-drop";
import { Team } from 'src/app/models/team';
import { Player } from 'src/app/models/player';
import { LoadingScreenService } from 'src/app/services/loading-screen.service';
import { AngularFireRemoteConfig } from '@angular/fire/compat/remote-config';
import { Log } from 'src/app/models/enums/log';
import { ToastrService } from 'ngx-toastr';

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
    //this one
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
    gameLogs: Log[] = [];
    playerOnTurn: Player = new Player("", 0);
    //@ViewChild('boardComponent', {static: false}) boardComponent: GameboardComponent | undefined;

    isNewGame: boolean = false;
    timerMaxSeconds: number = 90;

    leaveGameBtnLabel: string = "";
    skipTurnBtnLabel: string = "";
    changeLetterBtnLabel: string = "";
    changeLetterSecondBtnLabel: string = "";
    writeWordBtnLabel: string = "";
    youAreOnTurnText: string = "";
    onTurnIsText: string = "";
    leftLettersText: string = "";
    noSuchGameText: string = "";
    
    constructor(
        private gameService: GameService,
        private signalrService: SignalrService,
        private router: Router,
        public dialog: MatDialog,
        private elementRef: ElementRef,
        private loadingScreenService: LoadingScreenService,
        private remoteConfig: AngularFireRemoteConfig,
        private toastr: ToastrService) {
        this.loadRemoteConfigTexts();
    }

    private loadRemoteConfigTexts() {
        //AppConfig.isRemoteConfigFetched = false;
        this.remoteConfig.fetchAndActivate().then(hasActivatedTheFetch => {
            this.remoteConfig.getAll().then(all => {
            //AppConfig.isRemoteConfigFetched = true;
                this.leaveGameBtnLabel = all["LeaveGameBtnLabel"].asString()!;
                this.skipTurnBtnLabel = all["SkipTurnBtnLabel"].asString()!;
                this.changeLetterBtnLabel = all["ChangeLetterBtnLabel"].asString()!;
                this.changeLetterSecondBtnLabel = all["ChangeLetterSecondBtnLabel"].asString()!;
                this.writeWordBtnLabel = all["WriteWordBtnLabel"].asString()!;
                this.youAreOnTurnText = all["YouAreOnTurnText"].asString()!;
                this.onTurnIsText = all["OnTurnIsText"].asString()!;
                this.leftLettersText = all["LeftLettersText"].asString()!;
                this.noSuchGameText = all["NoSuchGameText"].asString()!;
            })
        })
    }

    ngOnInit(): void {
        //this.loadingScreenService.showLoadingScreen();
        this.isNewGame = true;
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

        this.isNewGame = false;
    }

    attachGameListeners() {
        this.signalrService.hubConnection?.on("UpdateGameState", data => {      
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

            this.timerMaxSeconds = data.commonGameState.maxTimerSeconds;

            if(data.commonGameState.logs) {
                let logs: Log[] = data.commonGameState.logs;
                // Game logs are reversed in order to show the latest ones on the list bottom
                this.gameLogs = logs.reverse();
            }

            if(data.commonGameState.remainingSecondsByUserNames) {
                this.parseRemainingSeconds(data.commonGameState.remainingSecondsByUserNames);
            }

            if(data.commonGameState.isGameOver == true) {
                this.router.navigate([this.router.url + "/summary"]);
            }
            this.loadingScreenService.stopShowingLoadingScreen();
        })
    
        this.signalrService.hubConnection?.on("InvalidWriteWordInput", data => {
            console.log("Invalid Write Word Input");
            //TODO: Return all wildcards to their normal state
            let dialogData: ErrorDialogData = { message: Object.values(data.errorsByCodes).toString(), unexistingWords: null };
            //TODO: Change || to &&
            if(data.unexistingWords && data.unexistingWords.length > 0) {
                dialogData.unexistingWords = data.unexistingWords;
            }
            this.dialog.open(ErrorDialogComponent, { data: dialogData});

            this.selectedBoardCell = null;
        })

        this.signalrService.hubConnection?.on("InvalidExchangeTilesInput", data => {
            this.dialog.open(ErrorDialogComponent, { data: { message: Object.values(data.errorsByCodes)}});
            this.showExchangeField = false;
            this.selectedExchangeTiles = [];
        })

        this.signalrService.hubConnection?.on("ImpossibleToSkipTurn", data => {
            this.dialog.open(ErrorDialogComponent, { data: { message: Object.values(data.errorsByCodes)}});
        })

        this.signalrService.hubConnection?.on("ReceiveAllWildcardOptions", data => {
            this.wildcardOptions = data;
        })

        this.signalrService.hubConnection?.on("UpdateGameTimer", data => {
            const fillString: string = "0";
            const maxLength: number = 2;
            let minutes: string = data.minutes.toString().padStart(maxLength, fillString);
            let seconds: string = data.seconds.toString().padStart(maxLength, fillString);
            this.gameTimeAsString = `${minutes}:${seconds}`;
            this.turnRemainingTime = data.minutes * 60 + data.seconds;
            this.playerOnTurn.remainingSeconds = data.minutes * 60 + data.seconds;
        });

        this.signalrService.hubConnection?.on("UserEnteredGameFromAnotherConnectionId", () => {
            this.router.navigateByUrl("/");
        });
        
        this.signalrService.hubConnection?.on("NoSuchGame", () => {
            this.remoteConfig.fetchAndActivate().then(hasActivatedTheFetch => {
                this.remoteConfig.getAll().then(all => {
                    this.noSuchGameText = all["NoSuchGameText"].asString()!;
                    this.toastr.error(this.noSuchGameText, '', {
                        progressBar: true,
                        closeButton: true,
                        progressAnimation: 'increasing',
                        tapToDismiss: true,
                    });
                    this.router.navigateByUrl("/");
                })
            })
        })
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
        this.playerTiles = new Array(playerTiles.length);
        for(let i = 0; i < playerTiles.length; i++) {
        let playerTile = playerTiles[i];
        this.playerTiles[i] = new Tile(playerTile.letter, playerTile.points);
        }
    }

    loadScoreBoard(teams: any): void {
        this.teams = [];
        for(let i = 0; i < teams.length; i++) {
            this.teams.push(this.loadTeam(teams[i].players));
        }
        this.teams.sort((a : Team, b : Team) => {
            return b.points - a.points;
        });
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

    //Useless
    removeTileFromBoard(tile: Tile) {
        //this.boardComponent?.removeTileFromBoard(tile);
    }

    isCurrentPlayerOnTurn() : boolean {
        return this.currentUserName == this.playerNameOnTurn;
    }

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
        this.updatedBoardCells = cells;
    }

    writeWord() : void {     
        if(this.updatedBoardCells.length <= 0) {
            return;
        }
        
        //Check for null tiles
        this.updatedBoardCells = this.updatedBoardCells.filter(item => item.cell.tile !== null);
        
        //Map current updatedBoardCells, so back-end's format
        let writeWordInput = this.updatedBoardCells.map(item => ({key: item.cell.tile, value: item.coordinates}))

        if(!this.checkForWildcards(writeWordInput.map(item => (item.key)))) {
            try {
                //this.signalrService.writeWord(writeWordInput);
                this.gameService.writeWord(writeWordInput);
            }
            catch (ex) {
                console.log("ERROR");
                //console.log(ex);
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
}