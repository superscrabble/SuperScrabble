import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Injectable({
    providedIn: 'root'
})
export class ErrorHandler {

    constructor(private router: Router, private toastr: ToastrService) {}

    handle(error: number) {
        if(error == 401) {
            //TODO: move this into a service
            //TODO: get his message from Remote Config
            //Show toast
            this.toastr.error('Трябва да влезнете в акаунта си или да си направите такъв, за да ползвате тази функционалност!', '', {
                progressBar: true,
                closeButton: true,
                progressAnimation: 'increasing',
                tapToDismiss: true,
            });
            this.router.navigateByUrl("/login");
        }
        else if(error == 404) {
            this.router.navigateByUrl("/notfound");
        }
        else if(error == 500) {
            this.router.navigateByUrl("/servererror");
        }
    }
}