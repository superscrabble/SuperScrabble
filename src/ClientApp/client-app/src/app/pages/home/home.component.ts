import { Component, OnInit, ViewChild } from '@angular/core';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { NgForm } from '@angular/forms';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  @ViewChild('form', { static: false }) registerForm!: NgForm;

  user = {
    username: "",
    email: "",
    password: "",
    repeatedPassword: ""
  }

  constructor(private http: HttpClient) { }

  ngOnInit(): void {
    console.log("In home");
  }

  onRegister(): void {
    this.user.username = this.registerForm.value.username;
    this.user.email = this.registerForm.value.email;
    this.user.password = this.registerForm.value.password;
    this.user.repeatedPassword = this.registerForm.value.repeatedPassword;
    console.log(this.user);
    
    this.http.post(`https://localhost:5001/api/users/register`, 
                  this.user, 
                  { headers: {"Content-Type": "application/json"}, observe: 'response', responseType: 'text' })
                  .subscribe((res: HttpResponse<any>) => {
      console.log("Success sign up" + res.status);
      console.log(res.body)
    },
    error => {
      console.log("oops", error);
    })
  }
}
