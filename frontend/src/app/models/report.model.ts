export interface GenreReportItem {
  genreId: string;
  genreName: string;
  bookCount: number;
  likeCount: number;
}

export interface ReportData {
  totalBooks: number;
  totalRead: number;
  booksByGenre: GenreReportItem[];
  genresByLikes: GenreReportItem[];
}
