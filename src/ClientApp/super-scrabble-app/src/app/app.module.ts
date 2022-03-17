import { isDevMode, NgModule, Optional } from '@angular/core';
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
import { ScoreboardComponent } from './common/scoreboard/scoreboard.component';
import { WordInfoComponent } from './common/word-info/word-info.component';
import { GameLogsComponent } from './common/game-logs/game-logs.component';
import { GameboardComponent } from './common/gameboard/gameboard.component';
import { ErrorDialogComponent } from './dialogs/error-dialog/error-dialog.component';
import { SettingsDialogComponent } from './dialogs/settings-dialog/settings-dialog.component';
import { ChangeWildcardDialogComponent } from './dialogs/change-wildcard-dialog/change-wildcard-dialog.component';
import { LeaveGameDialogComponent } from './dialogs/leave-game-dialog/leave-game-dialog.component';
import { GameContentDialogComponent } from './dialogs/game-content-dialog/game-content-dialog.component';
import { ExchangeTilesDialogComponent } from './dialogs/exchange-tiles-dialog/exchange-tiles-dialog.component';
import { ExchangeTilesComponent } from './common/exchange-tiles/exchange-tiles.component';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { ScrollingModule } from '@angular/cdk/scrolling';
import { ClipboardModule } from '@angular/cdk/clipboard';
import { MatTooltipModule } from '@angular/material/tooltip';
import { PlayerRackComponent } from './common/player-rack/player-rack.component';
import { JoinPartyWithCodeDialogComponent } from './dialogs/join-party-with-code-dialog/join-party-with-code-dialog.component';
import { PartyPageComponent } from './pages/party-page/party-page.component';
import { MatchesDashboardComponent } from './common/matches-dashboard/matches-dashboard.component';
import { MatchComponent } from './common/match/match.component';
import { LoadingScreenComponent } from './pages/loading-screen/loading-screen.component';
import { ToastrModule } from 'ngx-toastr';
import { initializeApp,provideFirebaseApp } from '@angular/fire/app';
import { environment } from '../environments/environment';
import { provideRemoteConfig,getRemoteConfig, RemoteConfig, getAllChanges } from '@angular/fire/remote-config';
import { traceUntilFirst } from '@angular/fire/performance';
import { tap } from 'rxjs/operators';
import { filter, first, map, last, } from 'rxjs/operators';
import { AngularFireRemoteConfig, AngularFireRemoteConfigModule, budget, DEFAULTS, filterFresh, scanToObject, SETTINGS } from '@angular/fire/compat/remote-config';
import { AngularFireModule } from '@angular/fire/compat';
import { GameTimerComponent } from './common/game-timer/game-timer.component';
import { WaitingQueueDialogComponent } from './dialogs/waiting-queue-dialog/waiting-queue-dialog.component';

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
    PlayerRackComponent,
    JoinPartyWithCodeDialogComponent,
    PartyPageComponent,
    MatchesDashboardComponent,
    MatchComponent,
    LoadingScreenComponent,
    GameTimerComponent,
    WaitingQueueDialogComponent,
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
    }),
    //provideFirebaseApp(() => initializeApp(environment.firebase)),
    //provideRemoteConfig(() => getRemoteConfig()),
    AngularFireModule.initializeApp(environment.firebase),
    AngularFireRemoteConfigModule
  ],
  providers: [ 
    Utilities,
    AngularFireRemoteConfig,
    { provide: DEFAULTS, useValue: { enableAwesome: true } },
    {
      provide: SETTINGS,
      useFactory: () => isDevMode() ? { minimumFetchIntervalMillis: 10000, fetchTimeoutMillis: 60000 } : {}
    },
   ],
  bootstrap: [AppComponent]
})
export class AppModule {
  
  // constructor(remoteConfig: AngularFireRemoteConfig) {
  //   /*remoteConfig.changes.pipe(
  //     filterFresh(172_800_000), // ensure we have values from at least 48 hours ago
  //     first(),
  //     // scanToObject when used this way is similar to defaults
  //     // but most importantly smart-casts remote config values and adds type safety
  //     scanToObject({
  //       enableAwesome: true,
  //       titleBackgroundColor: 'blue',
  //       titleFontSize: 12
  //     })
  //   ).subscribe(() => {
  //     console.log("FIRST SUBSCRIBE")
  //   })*/
  
  //   // all remote config values cast as strings
  //   remoteConfig.strings.subscribe()
  //   remoteConfig.booleans.subscribe(); // as booleans
  //   remoteConfig.numbers.subscribe(); // as numbers
  
  //   // however those may emit more than once as the remote config cache fires and gets fresh values
  //   // from the server. You can filter it out of .changes for more control:
  //   remoteConfig.changes.pipe(
  //     filter(param => param.key === 'titleBackgroundColor'),
  //     map(param => param.asString()),
  //     // budget at most 800ms and return the freshest value possible in that time
  //     // our budget pipe is similar to timeout but won't error or abort the pending server fetch
  //     // (it won't emit it, if the deadline is exceeded, but it will have been fetched so can use the
  //     // freshest values on next subscription)
  //     budget(800),
  //     last()
  //   ).subscribe()
  
  //   // just like .changes, but scanned into an array
  //   remoteConfig.parameters.subscribe(all => {
  //     console.log("AFTER PARAMETERS: " + all);
  //   });
  
  //   // or make promisified firebase().remoteConfig() calls direct off AngularFireRemoteConfig
  //   // using our proxy
  //   remoteConfig.getAll().then(all => {
  //     console.log("AFTER GET ALL: " + all);
  //   });
  //   remoteConfig.lastFetchStatus.then(status => {
  //     console.log("REMOTE CONFIG STATUS: " + status)
  //   });
  // }
}
