import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class LoadingScreenService {

  isShowingLoading: boolean = false;

  constructor() {}

  showLoadingScreen() {
    if(this.isShowingLoading) return;

    setTimeout(() => {
      this.isShowingLoading = true;
    }, 0)
    
  }

  stopShowingLoadingScreen() {
    if(!this.isShowingLoading) return;

    setTimeout(() => {
      this.isShowingLoading = false;
    }, 0)
    //this.isShowingLoading = false;
  }

  isShowingLoadingScreen() : boolean {
    return this.isShowingLoading;
  }
}
