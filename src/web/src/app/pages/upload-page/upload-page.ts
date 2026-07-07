import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { ChangeDetectorRef, Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { ApiResponse, ImportResult } from '../../shared/models/api-response';

@Component({
  selector: 'app-upload-page',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './upload-page.html',
})
export class UploadPage {
  private http = inject(HttpClient);
  private router = inject(Router);
  private cdr = inject(ChangeDetectorRef);

  apiUrl = 'http://localhost:5000';

  file?: File;
  isUploading = false;

  message = '';
  isError = false;

  onDragOver(event: DragEvent) {
    event.preventDefault();
  }

  onDrop(event: DragEvent) {
    event.preventDefault();

    if (event.dataTransfer?.files.length) {
      this.setFile(event.dataTransfer.files[0]);
    }
  }

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;

    if (input.files?.length) {
      this.setFile(input.files[0]);
    }
  }

  setFile(file: File) {
    this.message = '';
    this.isError = false;

    if (!file.name.toLowerCase().endsWith('.csv')) {
      this.file = undefined;
      this.showError('Only CSV files are allowed. Please choose another file.');
      return;
    }

    this.file = file;
    this.cdr.detectChanges();
  }

  upload() {
    this.message = '';
    this.isError = false;

    if (!this.file) {
      this.showError('Please select a CSV file first.');
      return;
    }

    this.isUploading = true;
    this.cdr.detectChanges();

    const formData = new FormData();
    formData.append('file', this.file);

    this.http
      .post<ApiResponse<ImportResult>>(`${this.apiUrl}/api/imports`, formData)
      .subscribe({
        next: (response) => {
          this.isUploading = false;

          if (!response.success) {
            this.showError(response.errors[0] ?? 'Upload failed. Please choose another file.');
            return;
          }

          const successMessage =
            response.data?.message ?? 'Upload succeeded. Books were imported.';

          this.router.navigate(['/books'], {
            state: {
              successMessage,
            },
          });
        },
        error: (err) => {
          this.isUploading = false;

          const backendMessage =
            err?.error?.errors?.[0] ??
            'This CSV file was already uploaded or has an invalid format. Please choose another file.';

          this.showError(backendMessage);
        },
      });
  }

  private showError(message: string) {
    this.message = message;
    this.isError = true;
    this.cdr.detectChanges();
  }
}
