import { HttpClient, HttpResponse } from '@angular/common/http';
import { Component, OnInit, ViewChild } from '@angular/core';
import { AngularFireRemoteConfig } from '@angular/fire/compat/remote-config';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { AppConfig } from 'src/app/app-config';
import { ErrorHandler } from 'src/app/services/error-handler';
import { WebRequestsService } from 'src/app/services/web-requests.service';

@Component({
  selector: 'app-login-form',
  templateUrl: './login-form.component.html',
  styleUrls: ['./login-form.component.css']
})
export class LoginFormComponent implements OnInit {
  @ViewChild('form', { static: false }) loginForm!: NgForm;

  input = {
    username: null,
    password: null
  }

  loginPageTitle: string = "";
  loginPageUsername: string = "";
  loginPagePassword: string = "";
  loginBtnText: string = "";

  propertyErrorMessages = new Map();
  noSuchUserError: string = "";
  showError: boolean = false;

  constructor(private webRequestsService: WebRequestsService, private router: Router, private errorHandler: ErrorHandler,
              private remoteConfig: AngularFireRemoteConfig) {
    this.loadRemoteConfigTexts();
  }

  ngOnInit(): void {
  }

  private loadRemoteConfigTexts() {
    //AppConfig.isRemoteConfigFetched = false;
    this.remoteConfig.fetchAndActivate().then(hasActivatedTheFetch => {
      this.remoteConfig.getAll().then(all => {
        //AppConfig.isRemoteConfigFetched = true;
        this.loginPageTitle = all["LoginPageTitle"].asString()!;
        this.loginPageUsername = all["LoginPageUsername"].asString()!;
        this.loginPagePassword = all["LoginPagePassword"].asString()!;
        this.loginBtnText = all["LoginBtnText"].asString()!;
        this.noSuchUserError = all["NoSuchUser"].asString()!;
      })
    })
  }


  userNameErrorMessages(): string[] {
    return this.propertyErrorMessages.get("UserName");
  }

  passwordErrorMessages(): string[] {
    return this.propertyErrorMessages.get("Password");
  }

  onLogin(): void {
    this.input.username = this.loginForm.value.username;
    this.input.password = this.loginForm.value.password;

    const url = 'api/users/login';

    this.propertyErrorMessages = new Map();

    this.showError = false;
    
    this.webRequestsService.post(url, this.input)
    .subscribe({
      next: this.handleLoginResponse.bind(this),
      error: this.handleError.bind(this)
    });
  }

  handleLoginResponse(res: HttpResponse<any>): void {
    let data = JSON.parse(res.body);
    localStorage.setItem("access_token", data["token"]);
    this.router.navigate(['/']);
  }

  handleError(error: any): void {
    //console.log("LOGIN ERROR")
    //console.log(error);

    if (error.status == 400 || error.status == 404) {
      this.showError = true;
      
      /*const errors = JSON.parse(error.error);
      for (let i = 0; i < errors.length; i++) {
        let propertyName = errors[i].key;
        let errorMessages = errors[i].value;

        console.log(propertyName)
        console.log(errorMessages);
        this.propertyErrorMessages.set(propertyName, errorMessages);
      }*/
    }
    else {
      //this.errorHandler.handle(error.status);
    }
  }
}
