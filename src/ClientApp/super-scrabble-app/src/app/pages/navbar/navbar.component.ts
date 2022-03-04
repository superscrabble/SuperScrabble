import { Component, OnInit } from '@angular/core';
import { AngularFireRemoteConfig } from '@angular/fire/compat/remote-config';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { AppConfig } from 'src/app/app-config';
import { Utilities } from 'src/app/common/utilities';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent implements OnInit {
  
  public isMenuCollapsed = true;
  appName: string = "";
  navHomeLabel: string = "";
  navLoginLabel: string = "";
  navRegisterLabel: string = "";
  navLogOutLabel: string = "";
  
  constructor(public utilities: Utilities, private router: Router, 
              public remoteConfig: AngularFireRemoteConfig) { 
                this.loadRemoteConfigTexts();
  }

  ngOnInit(): void {
  }

  private loadRemoteConfigTexts() {
    //AppConfig.isRemoteConfigFetched = false;
    this.remoteConfig.fetchAndActivate().then(hasActivatedTheFetch => {
      this.remoteConfig.getAll().then(all => {
        //AppConfig.isRemoteConfigFetched = true;
        this.appName = all["AppName"].asString()!;
        this.navHomeLabel = all["NavHomeLabel"].asString()!;
        this.navLoginLabel = all["NavLoginLabel"].asString()!;
        this.navRegisterLabel = all["NavRegisterLabel"].asString()!;
        this.navLogOutLabel = all["NavLogOutLabel"].asString()!;
      })
    })
  }

  logout(): void {
    this.utilities.deleteAccessToken();
    this.router.navigateByUrl('/');
    console.log("logout");
  }
}
