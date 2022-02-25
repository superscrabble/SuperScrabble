import { Injectable } from '@angular/core';
import { Router } from '@angular/router';

@Injectable({
    providedIn: 'root'
})
export class ErrorHandler {

    constructor(private router: Router) {}

    handle(error: number) {
        if(error == 401) {
            //Show toast
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