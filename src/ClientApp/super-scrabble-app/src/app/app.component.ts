import { Component } from '@angular/core';
import { LoadingScreenService } from './services/loading-screen.service';

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
