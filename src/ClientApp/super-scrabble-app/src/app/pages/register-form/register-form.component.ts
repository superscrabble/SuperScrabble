import { Component, OnInit, ViewChild } from '@angular/core';
import { HttpResponse } from '@angular/common/http';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { ErrorHandler } from 'src/app/services/error-handler';
import { WebRequestsService } from 'src/app/services/web-requests.service';
import { AngularFireRemoteConfig } from '@angular/fire/compat/remote-config';

@Component({
  selector: 'app-register-form',
  templateUrl: './register-form.component.html',
  styleUrls: ['./register-form.component.css']
})
export class RegisterFormComponent implements OnInit {
  @ViewChild('form', { static: false }) registerForm!: NgForm;
  
  input = {
    username: null,
    email: null,
    password: null,
    repeatedPassword: null
  }

  propertyErrorMessages = new Map();
  errors: string[] = [];

  registerPageTitle: string = "";
  registerPageUsername: string = "";
  registerPageEmail: string = "";
  registerPagePassword: string = "";
  registerPageRepeatPassword: string = "";
  registerBtnText: string = "";

  constructor(private webRequestsService: WebRequestsService, private router: Router, private errorHandler: ErrorHandler,
      private remoteConfig: AngularFireRemoteConfig) {
    this.loadRemoteConfigTexts();
  }

  ngOnInit(): void {}

  private loadRemoteConfigTexts() {
    //AppConfig.isRemoteConfigFetched = false;
    this.remoteConfig.fetchAndActivate().then(hasActivatedTheFetch => {
      this.remoteConfig.getAll().then(all => {
        //AppConfig.isRemoteConfigFetched = true;
        this.registerPageTitle = all["RegisterPageTitle"].asString()!;
        this.registerPageUsername = all["RegisterPageUsername"].asString()!;
        this.registerPageEmail = all["RegisterPageEmail"].asString()!;
        this.registerPagePassword = all["RegisterPagePassword"].asString()!;
        this.registerPageRepeatPassword = all["RegisterPageRepeatPassword"].asString()!;
        this.registerBtnText = all["RegisterBtnText"].asString()!;
      })
    })
  }

  userNameErrorMessages(): string[] {
    return this.propertyErrorMessages.get("UserName");
  }
  
  emailErrorMessages(): string[] {
    return this.propertyErrorMessages.get("Email");
  }

  passwordErrorMessages(): string[] {
    return this.propertyErrorMessages.get("Password");
  }

  repeatedPasswordErrorMessages(): string[] {
    return this.propertyErrorMessages.get("RepeatedPassword");
  }

  onRegister(): void {
    this.input.username = this.registerForm.value.username;
    this.input.email = this.registerForm.value.email;
    this.input.password = this.registerForm.value.password;
    this.input.repeatedPassword = this.registerForm.value.repeatedPassword;

    const url = 'api/users/register';

    this.propertyErrorMessages = new Map();
    
    this.webRequestsService.post(url, this.input)
    .subscribe({
      next: this.handleRegisterResponse.bind(this),
      error: this.handleError.bind(this)
    });
  }

  handleRegisterResponse(res: HttpResponse<any>): void {
    let data = JSON.parse(res.body);
    localStorage.setItem("access_token", data["token"]);
    this.router.navigate(['/']);
  }

  handleError(error: any): void {
    console.log("Error on register")
    console.log(error.status);

    if (error.status == 400) {
      this.errors = JSON.parse(error.error)
      this.remoteConfig.fetchAndActivate().then(hasActivatedTheFetch => {
        this.remoteConfig.getAll().then(all => {
          for(let i = 0; i < this.errors.length; i++) {
            this.errors[i] = all[this.errors[i]].asString();
          }
        })
      })
    }
    else {
      this.errorHandler.handle(error.status);
    }
  }
}