import { Component, EventEmitter, Input, OnChanges, OnInit, Output, Pipe, PipeTransform, SimpleChanges } from '@angular/core';

@Pipe({
  name: "formatTime"
})
export class FormatTimePipe implements PipeTransform {
  transform(value: number): string {
    const minutes: number = Math.floor(value / 60);
    return (
        ("00" + minutes).slice(-2) +
        ":" +
        ("00" + Math.floor(value - minutes * 60)).slice(-2)
    );
  }
}

@Component({
  selector: 'app-game-timer',
  templateUrl: './game-timer.component.html',
  styleUrls: ['./game-timer.component.scss']
})
export class GameTimerComponent implements OnInit, OnChanges {
  
  FULL_DASH_ARRAY = 283;
  WARNING_THRESHOLD = 10;
  ALERT_THRESHOLD = 5;
  
  COLOR_CODES = {
    info: {
      color: "green"
    },
    warning: {
      color: "orange",
      threshold: this.WARNING_THRESHOLD
    },
    alert: {
      color: "red",
      threshold: this.ALERT_THRESHOLD
    }
  };
  
  //TIME_LIMIT = 20;
  timePassed = 0;
  timerInterval: number = 0;
  remainingPathColor = this.COLOR_CODES.info.color;
  circleDasharray: string =  "";
  
  @Input() timerMaxSeconds: number = 90;
  @Input() time: number = 0;
  @Input() isNewGame: boolean = false;
  @Output() setTimer: EventEmitter<void> = new EventEmitter();
  
  timeLeft = this.timerMaxSeconds;

  constructor() { }

  ngOnChanges(changes: SimpleChanges): void {
    //console.log("CHANGED: " + changes['time'].currentValue)

    if(changes['isNewGame']) {
      if(changes['isNewGame'].currentValue) {
        //this.timerMaxSeconds = changes['time'].currentValue;
      }
    }

    if(changes['time'].isFirstChange()) {
      //console.log("First Change " + changes['time'].currentValue + " " + changes['time'].previousValue)
      //this.timePassed = 0;
      //this.timeLeft = changes['time'].currentValue;
      //this.timerMaxSeconds = changes['time'].currentValue;
    }

    if(Math.abs(changes['time'].currentValue - changes['time'].previousValue) > 1) {
      if(this.timerMaxSeconds == changes['time'].currentValue) {
        this.timePassed = 0;
        this.remainingPathColor = this.COLOR_CODES.info.color;
      } else {
        this.timePassed = this.timerMaxSeconds - changes['time'].currentValue;
      }
      
      this.timeLeft = changes['time'].currentValue;
      //this.timerMaxSeconds = changes['time'].currentValue;
    }
  }

  ngOnInit(): void {
    this.startTimer()
    /*this.setTimer.subscribe(() => {
      console.log("Set TIMER called in GAME TIMER")
    })*/
  }


// startTimer();

// function onTimesUp() {
//   clearInterval(timerInterval);
//}

  startTimer() {

    this.timerInterval = window.setInterval(() => {
      //console.log("Tick " + this.timeLeft + " " + this.timePassed);
      //this.setTimer.emit();
      this.timePassed = this.timePassed += 1;
      this.timeLeft = this.timerMaxSeconds - this.timePassed;
      //this.document.getElementById("base-timer-label").innerHTML = formatTime(
      //  timeLeft
      //);
      this.setCircleDasharray();
      this.setRemainingPathColor(this.timeLeft);

      if (this.timeLeft === 0) {
        //this.onTimesUp();
      }
    }, 1000);
  }

// formatTime(time) {
//   const minutes = Math.floor(time / 60);
//   let seconds: string = time % 60;

//   if (seconds < 10) {
//     seconds = `0${seconds}`;
//   }

//   return `${minutes}:${seconds}`;
//  }

  setRemainingPathColor(timeLeft: number) {
    const { alert, warning, info } = this.COLOR_CODES;
    if (timeLeft <= alert.threshold) {
      // document
      //   .getElementById("base-timer-path-remaining")
      //   .classList.remove(warning.color);
      // document
      //   .getElementById("base-timer-path-remaining")
      //   .classList.add(alert.color);
      this.remainingPathColor = alert.color;
    } else if (timeLeft <= warning.threshold) {
      this.remainingPathColor = warning.color;
      // document
      //   .getElementById("base-timer-path-remaining")
      //   .classList.remove(info.color);
      // document
      //   .getElementById("base-timer-path-remaining")
      //   .classList.add(warning.color);
    } else {
      this.remainingPathColor = info.color;
    }
  }

  calculateTimeFraction() {
    const rawTimeFraction = this.timeLeft / this.timerMaxSeconds;
    return rawTimeFraction - (1 / this.timerMaxSeconds) * (1 - rawTimeFraction);
  }

  setCircleDasharray() {
    this.circleDasharray = `${(
      this.calculateTimeFraction() * this.FULL_DASH_ARRAY
    ).toFixed(0)} 283`; //{{this.TIME_LIMIT}}
    
    // document
    //   .getElementById("base-timer-path-remaining")
    //   .setAttribute("stroke-dasharray", circleDasharray);
  }

}
