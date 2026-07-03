import { Component, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { UploadDialog } from './upload-dialog/upload-dialog';
import { ChangeDetectorRef} from '@angular/core';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, MatDialogModule],
  templateUrl: './app.html',
})
export class App {
  
  private dialog = inject(MatDialog);
  private http = inject(HttpClient);

  books: any[] = [];

   constructor() {
    console.log("App instance", this);
  }

  

  apiUrl = 'http://localhost:5091'; // schimbă dacă API rulează pe alt port

   ngOnInit() {
    console.log("ngOnInit");
    this.loadBooks();
    setInterval(() => this.loadBooks(), 2000);
  }

  openUploadDialog() {
  const dialogRef = this.dialog.open(UploadDialog, {
    width: '650px'
  });

  dialogRef.afterClosed().subscribe(result => {
    if (result === true) {
      this.loadBooks();
    }
  });
}

 

private cdr = inject(ChangeDetectorRef);

loadBooks() {
  this.http.get<any[]>(`${this.apiUrl}/api/books`)
    .subscribe(res => {
      this.books = res;
      this.cdr.detectChanges();
    });
}

  
}