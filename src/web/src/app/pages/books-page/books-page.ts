import { Component, ChangeDetectorRef, inject, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-books-page',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './books-page.html',
})
export class BooksPage implements OnInit, OnDestroy {
  private http = inject(HttpClient);
  private cdr = inject(ChangeDetectorRef);

  books: any[] = [];
  successMessage = '';

  apiUrl = 'http://localhost:5000';

  private intervalId?: number;

  ngOnInit() {
    const state = history.state;

    if (state.successMessage) {
      this.successMessage = state.successMessage;

      setTimeout(() => {
        this.successMessage = '';
        this.cdr.detectChanges();
      }, 5000);
    }

    this.loadBooks();

    this.intervalId = window.setInterval(() => {
      this.loadBooks();
    }, 2000);
  }

  ngOnDestroy() {
    if (this.intervalId) {
      window.clearInterval(this.intervalId);
    }
  }

  loadBooks() {
    this.http.get<any[]>(`${this.apiUrl}/api/books`).subscribe({
      next: (res) => {
        this.books = [...res];
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Failed to load books:', err);
      },
    });
  }
}
