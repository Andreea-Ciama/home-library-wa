import { Routes } from '@angular/router';
import { HomePage } from './pages/home-page/home-page';
import { BooksPage } from './pages/books-page/books-page';
import { UploadPage } from './pages/upload-page/upload-page';

export const routes: Routes = [
  { path: '', component: HomePage },
  { path: 'books', component: BooksPage },
  { path: 'upload', component: UploadPage },
];