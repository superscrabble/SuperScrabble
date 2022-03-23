import { Component } from '@angular/core';
import { LoadingScreenService } from './services/loading-screen.service';
import { AngularFireRemoteConfig, budget, filterFresh, scanToObject } from '@angular/fire/compat/remote-config';
import { traceUntilFirst } from '@angular/fire/performance';
import { fetchAndActivate, getAll, getAllChanges, getRemoteConfig, getValue, RemoteConfig, RemoteConfigInstances, RemoteConfigModule } from '@angular/fire/remote-config';
import { Router } from '@angular/router';
import { fetchConfig } from 'firebase/remote-config';
import { EMPTY, filter, first, last, map, Observable, scan, tap } from 'rxjs';
import { AppConfig } from './app-config';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'super-scrabble-app';
  
  
  constructor(private loadingScreenService: LoadingScreenService) {
    this.loadingScreenService.stopShowingLoadingScreen();
  }

  showLoading() : boolean {
    return this.loadingScreenService.isShowingLoadingScreen();
  }
}
