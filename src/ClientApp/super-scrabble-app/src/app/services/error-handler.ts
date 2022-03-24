import { Injectable } from '@angular/core';
import { AngularFireRemoteConfig } from '@angular/fire/compat/remote-config';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { LoadingScreenService } from './loading-screen.service';

@Injectable({
    providedIn: 'root'
})
export class ErrorHandler {

    unauthorizedErrorMessage: string = "";

    constructor(private router: Router, private toastr: ToastrService,
                private loadingScreenService: LoadingScreenService,
                private remoteConfig: AngularFireRemoteConfig) {
        this.loadRemoteConfigTexts();
    }

    handle(error: number) {
        if(error == 401) {
            //TODO: move this into a service
            //TODO: get his message from Remote Config
            //TODO: clear the accessToken
            //Show toast
            this.remoteConfig.fetchAndActivate().then(hasActivatedTheFetch => {
                this.remoteConfig.getAll().then(all => {
                    this.unauthorizedErrorMessage = all["UnauthorizedErrorMessage"].asString()!;
                    this.toastr.error(this.unauthorizedErrorMessage, '', {
                        progressBar: true,
                        closeButton: true,
                        progressAnimation: 'increasing',
                        tapToDismiss: true,
                    });
                })
            })
            
            this.router.navigateByUrl("/login");
            this.loadingScreenService.stopShowingLoadingScreen();
        }
        else if(error == 404) {
            this.router.navigateByUrl("/notfound");
        }
        else if(error == 500) {
            this.router.navigateByUrl("/servererror");
        }
    }

    private loadRemoteConfigTexts() {
        this.remoteConfig.fetchAndActivate().then(hasActivatedTheFetch => {
            this.remoteConfig.getAll().then(all => {
                this.unauthorizedErrorMessage = all["UnauthorizedErrorMessage"].asString()!;
            })
        })
    }
}