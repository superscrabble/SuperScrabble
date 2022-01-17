import { Component, EventEmitter, Inject, OnInit, Output } from '@angular/core';
import { MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';

export interface SettingsDialogData {
  openLeaveGameDialog: Function;
}

@Component({
  selector: 'app-settings-dialog',
  templateUrl: './settings-dialog.component.html',
  styleUrls: ['./settings-dialog.component.css']
})
export class SettingsDialogComponent implements OnInit {

  openLeaveGameDialog: Function;

  //TODO: think how to remove MatDialog so that openLeaveGameDialog will work
  constructor(@Inject(MAT_DIALOG_DATA) public data: SettingsDialogData, private dialog: MatDialog) {
    this.openLeaveGameDialog = data.openLeaveGameDialog;
   }

  ngOnInit(): void {
  }

}
