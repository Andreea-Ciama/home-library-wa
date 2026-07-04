import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-upload-page',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './upload-page.html',
})
export class UploadPage {
  private http = inject(HttpClient);
  private router = inject(Router);

  apiUrl = 'http://localhost:5000';

  file?: File;
  isUploading = false;

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
      alert('Please select a CSV file first.');
      return;
    }

    this.isUploading = true;

    const formData = new FormData();
    formData.append('file', this.file);

    this.http.post(`${this.apiUrl}/api/imports`, formData).subscribe({
      next: () => {
        this.isUploading = false;

        setTimeout(() => {
          this.router.navigate(['/books']);
        }, 1000);
      },
      error: (err) => {
        this.isUploading = false;
        console.error(err);
        alert('Upload failed.');
      },
    });
  }
}