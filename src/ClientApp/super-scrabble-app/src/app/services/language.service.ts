import { Injectable } from '@angular/core';
import { AngularFireRemoteConfig } from '@angular/fire/compat/remote-config';

export type AppLanguage = 'bg' | 'en';

// English texts for every Firebase Remote Config key the app uses.
// Bulgarian stays in Remote Config (the current source of truth); when English
// is selected these values override the remote ones. Keys missing here fall
// back to the remote (Bulgarian) value.
const EN_TEXTS: { [key: string]: string } = {
  // Navbar
  AppName: 'SuperScrabble',
  NavHomeLabel: 'Home',
  NavLoginLabel: 'Log in',
  NavRegisterLabel: 'Register',
  NavLogOutLabel: 'Log out',

  // Home
  WelcomeMessage: 'Welcome to SuperScrabble!',
  ChooseGamemodeMessage: 'Choose a game mode',
  ContinueYourGameText: 'Continue your game',
  DuelGamemodeLabel: 'Duel',
  DuelGamemodeDesc: 'Face another player one on one.',
  DuoGamemodeLabel: 'Duo',
  DuoGamemodeDesc: 'Team up with a friend and take on another duo.',
  FriendlyGamemodeLabel: 'Friendly game',
  FriendlyGamemodeDesc: 'Create a private game and invite friends with a code.',
  ChessScrabbleGamemodeLabel: 'Chess Scrabble',
  ChessScrabbleGamemodeDesc: 'Every player has a chess-style time budget for the whole game.',
  ClassicGamemodeLabel: 'Classic',
  ClassicGamemodeDesc: 'The classic Scrabble experience.',
  PlayBtnText: 'Play',
  PlayWithRandomBtnText: 'Play with a random partner',
  JoinWithCodeBtnText: 'Join with a code',
  CreatePartyBtnText: 'Create a party',

  // Login / register
  LoginPageTitle: 'Log in',
  LoginPageUsername: 'Username',
  LoginPagePassword: 'Password',
  LoginBtnText: 'Log in',
  NoSuchUser: 'Wrong username or password.',
  RegisterPageTitle: 'Register',
  RegisterPageUsername: 'Username',
  RegisterPageEmail: 'Email',
  RegisterPagePassword: 'Password',
  RegisterPageRepeatPassword: 'Repeat password',
  RegisterBtnText: 'Register',
  UnauthorizedErrorMessage: 'Your session has expired. Please log in again.',

  // Matchmaking / party
  WaitingInQueuePopUp: 'Searching for opponents…',
  StopSearchingButtonText: 'Cancel',
  PartyTypeDuoText: 'Duo party',
  PartyTypeFriendlyText: 'Friendly game',
  CodeLabelText: 'Invitation code:',
  CopyBtnText: 'Copy',
  TimerTypeLabel: 'Timer type',
  TimerTypeStandardText: 'Per turn',
  TimerTypeChessText: 'Chess clock',
  TimerDifficultyLabel: 'Time',
  PlayersLabel: 'Players',
  StartGameBtnText: 'Start game',
  LeaveBtnText: 'Leave',
  EnterCodeText: 'Enter an invitation code',
  EnterCodeBtnText: 'Join',
  DenialBtnText: 'Cancel',

  // Game page
  LeaveGameBtnLabel: 'Leave game',
  SkipTurnBtnLabel: 'Skip turn',
  ChangeLetterBtnLabel: 'Exchange tiles',
  ChangeLetterSecondBtnLabel: 'Exchange selected',
  WriteWordBtnLabel: 'Play word',
  YouAreOnTurnText: 'Your turn',
  OnTurnIsText: 'Now playing:',
  LeftLettersText: 'Tiles left',
  NoSuchGameText: 'This game no longer exists.',
  ScoreboardLabel: 'Scoreboard',
  WordInfoLabel: 'Word meanings',
  SettingsTitle: 'Settings',
  basicCellHint: '',
  GameboardCenterCellHint: 'The first word must go through the center square.',

  // Scoreboard
  ScoreboardTimeLabel: 'Time',
  ScoreboardPlayerLabel: 'Player',
  ScoreboardPointsLabel: 'Points',
  ScoreboardPointsAbreviation: 'pts',
  // Rendered as " (<text>)" after the player name.
  ScoreboardLeftPlayerText: 'left',
  ScoreboardCurrentPlayerText: 'you',

  // Game log
  GameLogsLabel: 'Game log',
  GameLogsWriteWordText: 'played:',
  GameLogsSkipTurnText: 'skipped their turn',
  GameLogsLeaveGameText: 'left the game',
  GameLogsChangeTilesText: 'exchanged tiles',
  GameLogsNoLogsText: 'No moves yet',

  // Game summary
  SummaryWinText: 'Victory!',
  SummaryDefeatText: 'Defeat',
  SummaryDrawText: 'Draw',
  SummaryPageCaption: 'Final standings',
  SummaryScoreboardPlayerLabel: 'Player',
  SummaryScoreboardPointsLabel: 'Points',

  // Dialogs
  ErrorDialogTitle: 'Invalid move',
  ErrorDialogOkButton: 'OK',
  UnexistingWords: 'Some of the words do not exist in the dictionary.',
  InvalidInputTilesCount: 'Invalid number of placed tiles.',
  FirstWordMustGoThroughTheBoardCenter: 'The first word must go through the center of the board.',
  GapsBetweenInputTilesNotAllowed: 'Gaps between the placed tiles are not allowed.',
  ImpossibleTileExchange: 'Exchanging tiles is not possible right now.',
  InputTilesPositionsCollide: 'The placed tiles collide with each other.',
  NewTilesNotConnectedToTheOldOnes: 'New tiles must connect to the tiles already on the board.',
  TilePositionAlreadyTaken: 'One of those squares is already taken.',
  TilesNotOnTheSameLine: 'All tiles must be placed on a single line.',
  PlayerNotOnTurn: 'It is not your turn.',
  ChangeWildcardTitle: 'Choose a letter for the blank tile',
  ChangeWildcardOkButton: 'OK',
  ExchangeTilesTitle: 'Select tiles to exchange',
  LeavePopupText: 'Are you sure you want to leave the game?',
  LeaveComfirmBtnLabel: 'Leave',
  LeaveDenyBtnLabel: 'Stay',
};

// Texts that were hardcoded in templates and have no Remote Config key.
const LOCAL_TEXTS: { [lang in AppLanguage]: { [key: string]: string } } = {
  bg: {
    TeammateTilesLabel: 'Плочки на',
    TotalPointsLabel: 'Общо точки',
  },
  en: {
    TeammateTilesLabel: 'Tiles of',
    TotalPointsLabel: 'Team points',
  },
};

/**
 * Drop-in replacement for AngularFireRemoteConfig where texts are consumed:
 * exposes the same fetchAndActivate()/getAll() surface. Bulgarian (default)
 * returns the remote values unchanged; English overlays EN_TEXTS on top.
 */
@Injectable({
  providedIn: 'root'
})
export class LanguageService {

  private static readonly StorageKey = 'appLanguage';

  constructor(private remoteConfig: AngularFireRemoteConfig) { }

  get language(): AppLanguage {
    return localStorage.getItem(LanguageService.StorageKey) === 'en' ? 'en' : 'bg';
  }

  toggleLanguage(): void {
    const next: AppLanguage = this.language === 'bg' ? 'en' : 'bg';
    localStorage.setItem(LanguageService.StorageKey, next);
    // Texts are loaded once in component constructors, so reload to re-render everything.
    window.location.reload();
  }

  fetchAndActivate(): Promise<boolean> {
    return this.remoteConfig.fetchAndActivate();
  }

  getAll(): Promise<any> {
    return this.remoteConfig.getAll().then((all: any) => {
      if (this.language === 'bg') {
        return all;
      }

      return new Proxy(all, {
        get: (target: any, key: PropertyKey) => {
          if (typeof key === 'string' && key in EN_TEXTS) {
            return { asString: () => EN_TEXTS[key] };
          }
          return target[key];
        }
      });
    });
  }

  getLocalText(key: string): string {
    const texts = LOCAL_TEXTS[this.language];
    return texts[key] !== undefined ? texts[key] : '';
  }
}
