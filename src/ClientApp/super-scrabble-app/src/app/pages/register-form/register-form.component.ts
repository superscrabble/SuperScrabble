import { Component, OnInit, ViewChild } from '@angular/core';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { NgForm } from '@angular/forms';

@Component({
  selector: 'app-register-form',
  templateUrl: './register-form.component.html',
  styleUrls: ['./register-form.component.css']
})
export class RegisterFormComponent implements OnInit {
  @ViewChild('form', { static: false }) registerForm!: NgForm;

  user = {
    username: "",
    email: "",
    password: "",
    repeatedPassword: ""
  }

  propertyErrorMessages = new Map();

  constructor(private http: HttpClient) {}

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

    this.user.username = this.registerForm.value.username;
    this.user.email = this.registerForm.value.email;
    this.user.password = this.registerForm.value.password;
    this.user.repeatedPassword = this.registerForm.value.repeatedPassword;

    const url = 'https://localhost:5001/api/users/register';

    this.propertyErrorMessages = new Map();
    
    this.http.post(url, this.user, 
                  { headers: {"Content-Type": "application/json"}, observe: 'response', responseType: 'text' })
                  .subscribe((res: HttpResponse<any>) => {
      console.log("Success sign up" + res.status);
      console.log(res.body)
    },
    error => {
      if (error.status == 400) {
        const errors = JSON.parse(error.error);
        for (let i = 0; i < errors.length; i++) {
          let propertyName = errors[i].propertyName;
          let errorMessages = errors[i].errorMessages;
          this.propertyErrorMessages.set(propertyName, errorMessages);
        }
      }
    });
  }
}