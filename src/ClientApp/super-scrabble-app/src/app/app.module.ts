import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';

import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { RegisterFormComponent } from './pages/register-form/register-form.component';
import { LoginFormComponent } from './pages/login-form/login-form.component';
import { HomeComponent } from './pages/home/home.component';
import { NavbarComponent } from './pages/navbar/navbar.component';
import { FormatTimePipe } from './pages/game/game.component';
import { Utilities } from './common/utilities';
import { GameComponent } from './pages/game/game.component';
import { GameSummaryComponent } from './pages/game-summary/game-summary.component';
import { MatDialogModule } from '@angular/material/dialog';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'; 
import { CommonModule } from '@angular/common';
import { ScoreboardComponent } from './pages/common/scoreboard/scoreboard.component';
import { WordInfoComponent } from './pages/common/word-info/word-info.component';
import { GameLogsComponent } from './pages/common/game-logs/game-logs.component';
import { GameboardComponent } from './pages/common/gameboard/gameboard.component';
import { ErrorDialogComponent } from './pages/common/dialogs/error-dialog/error-dialog.component';
import { SettingsDialogComponent } from './pages/common/dialogs/settings-dialog/settings-dialog.component';
import { ChangeWildcardDialogComponent } from './pages/common/dialogs/change-wildcard-dialog/change-wildcard-dialog.component';
import { LeaveGameDialogComponent } from './pages/common/dialogs/leave-game-dialog/leave-game-dialog.component';
import { GameContentDialogComponent } from './pages/common/dialogs/game-content-dialog/game-content-dialog.component';
import { ExchangeTilesDialogComponent } from './pages/common/dialogs/exchange-tiles-dialog/exchange-tiles-dialog.component';
import { ExchangeTilesComponent } from './pages/common/exchange-tiles/exchange-tiles.component';
import { GameConfigurationComponent } from './pages/game-configuration/game-configuration/game-configuration.component';
import { GameOptionComponent } from './pages/game-configuration/game-option/game-option.component';
import { GameConfigurationMenuComponent } from './pages/game-configuration/game-configuration-menu/game-configuration-menu.component';
import { GameInviteFriendsDialogComponent } from './pages/game-configuration/dialogs/game-invite-friends-dialog/game-invite-friends-dialog.component';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { ScrollingModule } from '@angular/cdk/scrolling';
import { ClipboardModule } from '@angular/cdk/clipboard';
import { MatTooltipModule } from '@angular/material/tooltip';
import { PlayerRackComponent } from './pages/common/player-rack/player-rack.component';
import { JoinPartyWithCodeDialogComponent } from './pages/game-configuration/dialogs/join-party-with-code-dialog/join-party-with-code-dialog.component';
import { PartyPageComponent } from './pages/party-page/party-page.component';
import { MatchesDashboardComponent } from './pages/common/matches-dashboard/matches-dashboard.component';
import { MatchComponent } from './pages/common/match/match.component';
import { LoadingScreenComponent } from './pages/loading-screen/loading-screen.component';
import { ToastrModule } from 'ngx-toastr';

@NgModule({
  declarations: [
    AppComponent,
    RegisterFormComponent,
    LoginFormComponent,
    HomeComponent,
    NavbarComponent,
    GameComponent,
    GameSummaryComponent,
    FormatTimePipe,
    ScoreboardComponent,
    WordInfoComponent,
    GameLogsComponent,
    GameboardComponent,
    ErrorDialogComponent,
    SettingsDialogComponent,
    ChangeWildcardDialogComponent,
    LeaveGameDialogComponent,
    GameContentDialogComponent,
    ExchangeTilesDialogComponent,
    ExchangeTilesComponent,
    GameConfigurationMenuComponent,
    GameConfigurationComponent,
    GameOptionComponent,
    GameInviteFriendsDialogComponent,
    PlayerRackComponent,
    JoinPartyWithCodeDialogComponent,
    PartyPageComponent,
    MatchesDashboardComponent,
    MatchComponent,
    LoadingScreenComponent,
  ],
  imports: [
    CommonModule,
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    NgbModule,
    MatDialogModule,
    BrowserAnimationsModule,
    DragDropModule,
    ScrollingModule,
    ClipboardModule,
    MatTooltipModule,
    ToastrModule.forRoot({
      timeOut: 10000,
      positionClass: 'toast-top-right',
      preventDuplicates: true,
    })
  ],
  providers: [ Utilities ],
  bootstrap: [AppComponent]
})
export class AppModule { }
