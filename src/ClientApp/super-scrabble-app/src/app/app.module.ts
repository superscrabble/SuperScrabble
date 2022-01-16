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
import { ChangeLettersDialogComponent } from './pages/common/dialogs/change-letters-dialog/change-letters-dialog.component';
import { GameContentDialogComponent } from './pages/common/dialogs/game-content-dialog/game-content-dialog.component';

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
    ChangeLettersDialogComponent,
    GameContentDialogComponent
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
  ],
  providers: [ Utilities ],
  bootstrap: [AppComponent]
})
export class AppModule { }
