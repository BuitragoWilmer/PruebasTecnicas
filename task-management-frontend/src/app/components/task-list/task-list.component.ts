import { Component, EventEmitter, Input, Output } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TaskItemResponse } from '../../core/models/task-item.model';
import { UserResponse } from '../../core/models/user.model';
import {
  TaskStatusFilter,
  TaskStatusOption,
  TASK_STATUSES,
  NEXT_STATUS_OPTIONS,
  getStatusLabel,
} from '../../core/models/task-status.model';

@Component({
  selector: 'app-task-list',
  standalone: true,
  imports: [DatePipe, FormsModule],
  templateUrl: './task-list.component.html',
  styleUrl: './task-list.component.css',
})
export class TaskListComponent {
  @Input() tasks: TaskItemResponse[] = [];
  @Input() users: UserResponse[] = [];
  @Input() statusFilter: TaskStatusFilter = 'All';
  @Input() loading = false;

  /** 🔑 Ahora emite statusId numérico, no TaskStatus string */
  @Output() statusChange = new EventEmitter<{ taskId: number; statusId: number }>();
  @Output() filterChange = new EventEmitter<TaskStatusFilter>();

  readonly statuses: TaskStatusOption[] = TASK_STATUSES;
  readonly filterOptions: TaskStatusFilter[] = [
    'All',
    ...TASK_STATUSES.map((s) => s.id),
  ];

  userName(userId: number): string {
    return this.users.find((u) => u.userId === userId)?.fullName ?? 'Sin asignar';
  }

  statusLabel(statusId: number): string {
    return getStatusLabel(statusId);
  }

  availableNextStatuses(currentStatusId: number): TaskStatusOption[] {
    const allowed = NEXT_STATUS_OPTIONS[currentStatusId] ?? [];
    return TASK_STATUSES.filter((s) => allowed.includes(s.id));
  }

  filterLabel(filter: TaskStatusFilter): string {
    return filter === 'All' ? 'Todas' : getStatusLabel(filter);
  }

  onFilterSelect(value: string): void {
    const filter: TaskStatusFilter = value === 'All' ? 'All' : Number(value);
    this.filterChange.emit(filter);
  }

  onStatusSelect(taskId: number, value: string, select: HTMLSelectElement): void {
    if (!value) return;
    this.statusChange.emit({ taskId, statusId: Number(value) });
    select.value = '';
  }
}