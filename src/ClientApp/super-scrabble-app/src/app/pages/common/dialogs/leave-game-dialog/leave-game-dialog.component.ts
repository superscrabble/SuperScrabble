import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ErrorDialogComponent } from '../error-dialog/error-dialog.component';

@Component({
  selector: 'app-leave-game-dialog',
  templateUrl: './leave-game-dialog.component.html',
  styleUrls: ['./leave-game-dialog.component.css']
})
export class LeaveGameDialogComponent implements OnInit {

  constructor(public dialogRef: MatDialogRef<ErrorDialogComponent>) {}

  ngOnInit(): void {
  }

}