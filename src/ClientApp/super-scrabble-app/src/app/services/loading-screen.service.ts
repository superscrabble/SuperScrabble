import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class LoadingScreenService {

  constructor() {
    localStorage.setItem('showLoading', "false");
  }

  showLoadingScreen() {
    localStorage.setItem('showLoading', "true");
  }

  stopShowingLoadingScreen() {
    localStorage.setItem('showLoading', "false");
  }

  isShowingLoadingScreen() : boolean {
    let lsShowLoading =  localStorage.getItem('showLoading')
    if(lsShowLoading && lsShowLoading == "true") {      
      console.log("Show loading")
      console.log(lsShowLoading);
      return true;
    } else {
      console.log("dont Show loading")
      console.log(lsShowLoading);
      lsShowLoading =  localStorage.getItem('showLoading')
      console.log(lsShowLoading);
      return false;
    }
  }
}
