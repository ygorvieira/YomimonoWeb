export interface Genre {
  id: string;
  name: string;
  createdAt: string;
  updatedAt: string;
}

export interface CreateGenreDto {
  name: string;
}

export interface UpdateGenreDto {
  name: string;
}

export interface Result<T> {
  valid: boolean;
  data: T;
  messages: string[];
  statusCode: number;
}
