import { Component, OnInit } from '@angular/core';
import { Utilities } from 'src/app/common/utilities';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent implements OnInit {

  constructor(public utilities: Utilities) { }

  ngOnInit(): void {
  }
}
