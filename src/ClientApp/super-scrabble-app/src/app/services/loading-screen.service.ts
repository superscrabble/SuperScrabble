import { Injectable, OnChanges, SimpleChanges } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class LoadingScreenService implements OnChanges {

  isShowingLoading: boolean = false;

  lastLoadingDontShowTime: number = Number.MAX_SAFE_INTEGER;
  lastLoadingShowTime: number = Number.MAX_SAFE_INTEGER;

  constructor() {}

  ngOnChanges(changes: SimpleChanges): void {
    console.log("ON CHANGES " + this.isShowingLoading);
    console.log(changes)
  }

  showLoadingScreen() {
    console.log("Show" + " " + Date.now())
    if(this.isShowingLoading) return;

    setTimeout(() => {
      this.lastLoadingShowTime = Date.now();
      //this.isShowingLoading = true;
    }, 0)
    
  }

  stopShowingLoadingScreen() {
    console.log("Don't Show" + " " + Date.now())
    if(!this.isShowingLoading) return;

    setTimeout(() => {
      this.lastLoadingDontShowTime = Date.now();
      this.isShowingLoading = false;
    }, 0)
    //this.isShowingLoading = false;
  }

  isShowingLoadingScreen() : boolean {
    //console.log("IS SHOWING CALLED " + this.isShowingLoading + " " + Date.now());
    //if(this.lastLoadingDontShowTime >= this.lastLoadingShowTime) {
      //console.log("FALSE" + this.lastLoadingDontShowTime + " " + this.lastLoadingShowTime);
      return false;
    //}
    //console.log("TRUE" + this.lastLoadingDontShowTime + " " + this.lastLoadingShowTime);
    //return true;

    //return this.isShowingLoading;
  }
}
