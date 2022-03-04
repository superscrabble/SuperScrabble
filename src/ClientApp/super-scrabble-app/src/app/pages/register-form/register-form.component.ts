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

  constructor(private webRequestsService: WebRequestsService, private router: Router, private errorHandler: ErrorHandler,
      private remoteConfig: AngularFireRemoteConfig) {}

  ngOnInit(): void {}

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
    console.log("ERRORS IN REGIS")
    console.log(error);

    if (error.status == 400) {
      console.log(error.error);
      //let re = /\"/gi;
      //this.errors = error.error.replace('\[', '').replace('\]', '').trim().replace(re, '').split(',');
      
      console.log(JSON.parse(error.error));
      this.errors = JSON.parse(error.error)
      this.remoteConfig.fetchAndActivate().then(hasActivatedTheFetch => {
        this.remoteConfig.getAll().then(all => {
          for(let i = 0; i < this.errors.length; i++) {
            this.errors[i] = all[this.errors[i]].asString();
          }
        })
      })

      /*const errors = JSON.parse(error.error);
      for (let i = 0; i < errors.length; i++) {
        let propertyName = errors[i].propertyName;
        let errorMessages = errors[i].errorMessages;
        this.propertyErrorMessages.set(propertyName, errorMessages);
      }*/
    }
    else {
      this.errorHandler.handle(error.status);
    }
  }
}