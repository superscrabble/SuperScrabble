import { Component, OnInit } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-join-party-with-code-dialog',
  templateUrl: './join-party-with-code-dialog.component.html',
  styleUrls: ['./join-party-with-code-dialog.component.scss']
})
export class JoinPartyWithCodeDialogComponent implements OnInit {

  code: string = "";

  constructor(public dialogRef: MatDialogRef<JoinPartyWithCodeDialogComponent>) { }

  ngOnInit(): void {
  }

  joinParty() {

  }
}
