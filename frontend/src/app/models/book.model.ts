export interface Book {
  id: string;
  title: string;
  author: string;
  isbn: string;
  publicationYear: number;
  publisher: string;
  genre: string;
  description?: string | null;
  pageCount: number;
  coverUrl?: string | null;
  createdAt: string;
  updatedAt: string;
}

export interface CreateBookDto {
  title: string;
  author: string;
  isbn: string;
  publicationYear: number;
  publisher: string;
  genre: string;
  description?: string | null;
  pageCount: number;
  coverUrl?: string | null;
}

export interface UpdateBookDto {
  title?: string | null;
  author?: string | null;
  isbn?: string | null;
  publicationYear?: number | null;
  publisher?: string | null;
  genre?: string | null;
  description?: string | null;
  pageCount?: number | null;
  coverUrl?: string | null;
}

export interface Result<T> {
  valid: boolean;
  data: T;
  messages: string[];
  statusCode: number;
}
