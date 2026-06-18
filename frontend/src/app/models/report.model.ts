export interface GenreReportItem {
  genreId: string;
  genreName: string;
  bookCount: number;
  likeCount: number;
}

export interface AuthorReportItem {
  authorId: string;
  authorName: string;
  bookCount: number;
  totalPagesRead: number;
  likeCount: number;
}

export interface ReportData {
  totalBooks: number;
  totalRead: number;
  totalPagesRead: number;
  booksByGenre: GenreReportItem[];
  genresByLikes: GenreReportItem[];
  booksByAuthor: AuthorReportItem[];
  topAuthorsByLikes: AuthorReportItem[];
}
