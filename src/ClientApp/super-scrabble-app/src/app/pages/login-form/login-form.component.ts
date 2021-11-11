import { HttpClient, HttpResponse } from '@angular/common/http';
import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { ErrorHandler } from 'src/app/services/error-handler';

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

  propertyErrorMessages = new Map();

  constructor(private http: HttpClient, private router: Router, private errorHandler: ErrorHandler) { }

  ngOnInit(): void {
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

    const url = 'https://localhost:5001/api/users/login';

    this.propertyErrorMessages = new Map();
    
    this.http.post(url, this.input, 
                  { headers: {"Content-Type": "application/json"}, observe: 'response', responseType: 'text' })
                  .subscribe((res: HttpResponse<any>) => {
      let data = JSON.parse(res.body);
      localStorage.setItem("access_token", data["token"]);
      this.router.navigate(['/']);
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
      else {
        this.errorHandler.handle(error.status);
      }
    });
  }
}
