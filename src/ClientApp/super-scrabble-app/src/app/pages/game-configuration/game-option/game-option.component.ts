import { Component, Input, OnInit } from '@angular/core';
import { GameOption } from 'src/app/models/game-configuaration/game-option';

@Component({
  selector: 'app-game-option',
  templateUrl: './game-option.component.html',
  styleUrls: ['./game-option.component.css']
})
export class GameOptionComponent implements OnInit {

  @Input() gameOption: GameOption = {
    title: "",
    description: "",
    hint: "",
    value: 0,
    backgroundColorHex: ""
  };

  constructor() {
  }

  ngOnInit(): void {
  }

  openDescriptionDialog(): void {

  } 
}
