import { Component, OnInit } from '@angular/core';
import { LanguageService } from 'src/app/services/language.service';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { SignalrService } from 'src/app/services/signalr.service';

@Component({
  selector: 'app-waiting-queue-dialog',
  templateUrl: './waiting-queue-dialog.component.html',
  styleUrls: ['./waiting-queue-dialog.component.scss']
})
export class WaitingQueueDialogComponent implements OnInit {

  message: string = "";
  cancelButtonText: string = "";

  constructor(
    private dialog: MatDialog,
    private dialogRef: MatDialogRef<WaitingQueueDialogComponent>,
    private remoteConfig: LanguageService,
    private signalrService: SignalrService) {
    this.loadRemoteConfigTexts();
    this.dialog.afterAllClosed.subscribe(() => {
      console.log("Searching stopped")
      //this.stopSearching();
    })
  }

  ngOnInit(): void {
  }

  private loadRemoteConfigTexts() {
    this.remoteConfig.fetchAndActivate().then(hasActivatedTheFetch => {
      this.remoteConfig.getAll().then(all => {
        this.message = all["WaitingInQueuePopUp"].asString()!;
        this.cancelButtonText = all["StopSearchingButtonText"].asString()!;
      })
    })
  }

  stopSearching() {
    this.signalrService.stopSearching();
    this.dialogRef.close(true);
  }
}
