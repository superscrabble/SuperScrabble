import { Component, ElementRef, EventEmitter, Input, OnChanges, OnInit, Output, Renderer2, SimpleChanges } from '@angular/core';
import { GameConfig } from 'src/app/models/game-configuaration/game-config';
import { GameOption } from 'src/app/models/game-configuaration/game-option';

@Component({
  selector: 'app-game-configuration',
  templateUrl: './game-configuration.component.html',
  styleUrls: ['./game-configuration.component.css']
})
export class GameConfigurationComponent implements OnInit, OnChanges {

  @Input() gameConfig: GameConfig = new GameConfig("", [], () => {});

  @Output() onChosenOption: EventEmitter<any> = new EventEmitter<any>();

  constructor(private renderer: Renderer2, private elementRef: ElementRef) {
    //Check the height, if overflow => add margin

    //this.renderer.setStyle(this.elementRef.nativeElement, '--game-option-height', cellWidth, 2);
    this.renderer.setStyle(this.elementRef.nativeElement, '--options-count', this.gameConfig.gameOptions.length, 2);
  }

  ngOnInit(): void {
  }

  ngOnChanges(changes: SimpleChanges) {
    //This line is for an assurance of --options-count's value, can be removed if not useful
    this.renderer.setStyle(this.elementRef.nativeElement, '--options-count', this.gameConfig.gameOptions.length, 2);
  }

  chooseOption(gameOption: GameOption) {
    this.gameConfig.gameOptions.forEach(option => {
      option.isSelected = false;
    });

    gameOption.isSelected = true;
    this.onChosenOption.emit({chosenValue: gameOption});
  }
}
