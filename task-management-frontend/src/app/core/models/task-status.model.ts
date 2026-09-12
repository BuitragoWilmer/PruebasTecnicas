export interface TaskStatusOption {
  id: number;
  name: string;
  label: string;
}

export const TASK_STATUSES: TaskStatusOption[] = [
  { id: 1, name: 'Pending',    label: 'Pendiente'   },
  { id: 2, name: 'InProgress', label: 'En progreso' },
  { id: 3, name: 'Done',       label: 'Completada'  },
];

export type TaskStatusFilter = 'All' | number;

export const NEXT_STATUS_OPTIONS: Record<number, number[]> = {
  1: [2],    
  2: [1, 3],   
  3: [2],     
};


export function getStatusLabel(statusId: number): string {
  return TASK_STATUSES.find((s) => s.id === statusId)?.label ?? `Estado ${statusId}`;
}