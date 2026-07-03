import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';

import {
  MatDialogModule,
  MatDialogRef
} from '@angular/material/dialog';

import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-upload-dialog',
  standalone: true,
  imports: [
    CommonModule,
    MatDialogModule,
    MatButtonModule
  ],
  templateUrl: './upload-dialog.html',
  styleUrl: './upload-dialog.css',
})
export class UploadDialog {

  private http = inject(HttpClient);

  private dialogRef = inject(MatDialogRef<UploadDialog>);

  apiUrl = 'http://localhost:5091'; // pune portul corect al API-ului

  file?: File;
  onDragOver(event: DragEvent) {
  event.preventDefault();
}

onDrop(event: DragEvent) {
  event.preventDefault();

  if (event.dataTransfer?.files.length) {
    this.file = event.dataTransfer.files[0];
  }
}

onFileSelected(event: Event) {
  const input = event.target as HTMLInputElement;

  if (input.files?.length) {
    this.file = input.files[0];
  }
}

upload() {

  if (!this.file) {
    return;
  }

  const formData = new FormData();

  formData.append("file", this.file);

  this.http
    .post(`${this.apiUrl}/api/imports`, formData)
    .subscribe({
      next: () => {
        this.dialogRef.close(true);
      },
      error: (err) => {
        console.error(err);
        alert("Upload failed.");
      }
    });

}

}