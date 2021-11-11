import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Utilities } from 'src/app/common/utilities';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent implements OnInit {

  constructor(public utilities: Utilities, private router: Router) { }

  ngOnInit(): void {
  }

  logout(): void {
    this.utilities.deleteAccessToken();
    this.router.navigateByUrl('/');
    console.log("logout");
  }
}
