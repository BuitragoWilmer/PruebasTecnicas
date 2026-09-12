export type TaskStatus = 'Pending' | 'InProgress' | 'Done';

export interface TaskItemResponse {
  taskId: number;
  title: string | null;
  description: string | null;
  assignedUserId: number;
  statusId: number;
  additionalInfo: string | null;
  createdAt: string;
  updatedAt: string | null;
}

export interface CreateTaskItemCommand {
  title: string | null;
  description: string | null;
  assignedUserId: number;
  statusId: number;
  additionalInfo: string | null;
}

export interface UpdateTaskItemCommand {
  taskItemId: number;
  statusId: number;
}

export interface TaskItemsQueryParams {
  pageNumber?: number;
  sortBy?: string;
  sortDescending?: boolean;
  pageSize?: number;
  searchQuery?: string;
  filters?: { [key: string]: string };
  includeFields?: string;
}