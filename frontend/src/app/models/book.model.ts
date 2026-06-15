export interface Book {
  id: string;
  title: string;
  authorIds: string[];
  authorNames: string[];
  isbn?: string | null;
  publicationYear: number;
  publisher: string;
  genreId: string;
  genreName: string;
  description?: string | null;
  pageCount?: number | null;
  coverUrl?: string | null;
  readingStatus?: string | null;
  isLiked: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface CreateBookDto {
  title: string;
  authorIds: string[];
  isbn?: string | null;
  publicationYear: number;
  publisher: string;
  genreId: string;
  description?: string | null;
  pageCount?: number | null;
  coverUrl?: string | null;
  readingStatus?: string | null;
  isLiked: boolean;
}

export interface UpdateBookDto {
  title?: string | null;
  authorIds?: string[] | null;
  isbn?: string | null;
  publicationYear?: number | null;
  publisher?: string | null;
  genreId?: string | null;
  description?: string | null;
  pageCount?: number | null;
  coverUrl?: string | null;
  readingStatus?: string | null;
  isLiked?: boolean | null;
}

export interface UpdateBookStatusDto {
  readingStatus?: string | null;
  isLiked?: boolean | null;
}

export interface Result<T> {
  valid: boolean;
  data: T;
  messages: string[];
  statusCode: number;
}
