export interface Book {
  id: string;
  title: string;
  authorIds: string[];
  authorNames: string[];
  organizerIds: string[];
  organizerNames: string[];
  publicationYear: number;
  publisher: string;
  genreIds: string[];
  genreNames: string[];
  description?: string | null;
  pageCount?: number | null;
  coverUrl?: string | null;
  readingStatus?: string | null;
  isLiked: boolean;
  reReadCount: number;
  isTradePaperback: boolean;
  tradeEdition?: string | null;
  isDigital: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface CreateBookDto {
  title: string;
  authorIds: string[];
  publicationYear: number;
  publisher: string;
  genreIds: string[];
  description?: string | null;
  pageCount?: number | null;
  coverUrl?: string | null;
  readingStatus?: string | null;
  isLiked: boolean;
  organizerIds?: string[] | null;
  isTradePaperback?: boolean;
  tradeEdition?: string | null;
  isDigital?: boolean;
}

export interface UpdateBookDto {
  title?: string | null;
  authorIds?: string[] | null;
  publicationYear?: number | null;
  publisher?: string | null;
  genreIds?: string[] | null;
  description?: string | null;
  pageCount?: number | null;
  coverUrl?: string | null;
  readingStatus?: string | null;
  isLiked?: boolean | null;
  organizerIds?: string[] | null;
  isTradePaperback?: boolean | null;
  tradeEdition?: string | null;
  isDigital?: boolean | null;
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
