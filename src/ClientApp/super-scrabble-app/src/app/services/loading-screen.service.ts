import { Injectable, OnChanges, SimpleChanges } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class LoadingScreenService {

  isShowingLoading: boolean = false;

  lastLoadingDontShowTime: number = Number.MAX_SAFE_INTEGER;
  lastLoadingShowTime: number = Number.MAX_SAFE_INTEGER;

  constructor() {}

  showLoadingScreen() {
    if(this.isShowingLoading) return;

    setTimeout(() => {
      this.lastLoadingShowTime = Date.now();
      //this.isShowingLoading = true;
    }, 0)
    
  }

  stopShowingLoadingScreen() {
    if(!this.isShowingLoading) return;

    setTimeout(() => {
      this.lastLoadingDontShowTime = Date.now();
      this.isShowingLoading = false;
    }, 0)
    //this.isShowingLoading = false;
  }

  isShowingLoadingScreen() : boolean {
    //For now loading screen is stopped
    return false;
  }
}
