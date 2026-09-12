import { Component, OnInit, inject, signal } from '@angular/core';
import { UsersService } from './services/users.service';
import { TaskItemsService } from './services/task-items.service';
import { UserResponse } from './core/models/user.model';
import {
  TaskItemResponse,
  CreateTaskItemCommand,
} from './core/models/task-item.model';
import { TaskListComponent } from './components/task-list/task-list.component';
import { TaskFormComponent } from './components/task-form/task-form.component';

export type TaskStatusFilter = 'All' | number;

@Component({
  selector: 'app-root',
  imports: [TaskListComponent, TaskFormComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})
export class AppComponent implements OnInit {
  private userService = inject(UsersService);
  private taskService = inject(TaskItemsService);

  users = signal<UserResponse[]>([]);
  tasks = signal<TaskItemResponse[]>([]);
  statusFilter = signal<TaskStatusFilter>('All');
  loadingTasks = signal(false);
  errorMessage = signal<string | null>(null);

  ngOnInit(): void {
    this.loadUsers();
    this.loadTasks();
  }

  loadUsers(): void {
    this.userService
      .getList({ pageNumber: 1, pageSize: 100, sortBy: 'fullName' })
      .subscribe({
        next: (res) => this.users.set(res.items ?? []),
        error: () =>
          this.errorMessage.set('No se pudo cargar la lista de usuarios.'),
      });
  }

  loadTasks(): void {
    this.loadingTasks.set(true);
    this.errorMessage.set(null);

    const filter = this.statusFilter();
    const filters: { [key: string]: string } =
      filter === 'All' ? {} : { statusId: String(filter) };

    this.taskService
      .getList({
        pageNumber: 1,
        pageSize: 50,
        sortBy: 'createdAt',
        sortDescending: true,
        filters,
      })
      .subscribe({
        next: (res) => {
          this.tasks.set(res.items ?? []);
          this.loadingTasks.set(false);
        },
        error: () => {
          this.errorMessage.set(
            'No se pudieron cargar las tareas. Intenta nuevamente.'
          );
          this.loadingTasks.set(false);
        },
      });
  }

  onFilterChange(status: TaskStatusFilter): void {
    this.statusFilter.set(status);
    this.loadTasks();
  }

  onCreateTask(request: CreateTaskItemCommand): void {
    this.errorMessage.set(null);
    this.taskService.create(request).subscribe({
      next: () => this.loadTasks(),
      error: () =>
        this.errorMessage.set(
          'No se pudo crear la tarea. Verifica los datos e intenta de nuevo.'
        ),
    });
  }

 onStatusChange(event: { taskId: number; statusId: number }): void {
  this.errorMessage.set(null);
  this.taskService
    .updateStatus(event.taskId, {
      taskItemId: event.taskId,
      statusId: event.statusId,
    })
    .subscribe({
      next: () => this.loadTasks(),
      error: () =>
        this.errorMessage.set(
          'No se pudo actualizar el estado. Esa transición podría no estar permitida.'
        ),
    });
}

  dismissError(): void {
    this.errorMessage.set(null);
  }
}