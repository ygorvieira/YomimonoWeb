export interface Author {
  id: string;
  name: string;
  createdAt: string;
  updatedAt: string;
}

export interface CreateAuthorDto {
  name: string;
}

export interface UpdateAuthorDto {
  name: string;
}

export interface Result<T> {
  valid: boolean;
  data: T;
  messages: string[];
  statusCode: number;
}
