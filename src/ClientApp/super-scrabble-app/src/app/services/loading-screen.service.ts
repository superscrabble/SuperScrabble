import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class LoadingScreenService {

  isShowingLoading: boolean = false;

  constructor() {}

  showLoadingScreen() {
    this.isShowingLoading = true;
  }

  stopShowingLoadingScreen() {
    this.isShowingLoading = false;
  }

  isShowingLoadingScreen() : boolean {
    return this.isShowingLoading;
  }
}
