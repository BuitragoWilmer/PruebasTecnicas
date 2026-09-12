export interface UserResponse {
  userId: number;
  fullName: string | null;
  email: string | null;
}

export interface CreateUserCommand {
  fullName: string | null;
  email: string | null;
}

export interface UsersQueryParams {
  pageNumber?: number;
  sortBy?: string;
  sortDescending?: boolean;
  pageSize?: number;
  searchQuery?: string;
  filters?: { [key: string]: string };
  includeFields?: string;
}