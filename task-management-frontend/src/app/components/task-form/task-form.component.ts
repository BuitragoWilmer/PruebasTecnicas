import { Component, EventEmitter, Input, Output, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { CreateTaskItemCommand } from '../../core/models/task-item.model';
import { UserResponse } from '../../core/models/user.model';
import { TASK_STATUSES, TaskStatusOption } from '../../core/models/task-status.model';

@Component({
  selector: 'app-task-form',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './task-form.component.html',
  styleUrl: './task-form.component.css',
})
export class TaskFormComponent {
  @Input() users: UserResponse[] = [];
  @Output() create = new EventEmitter<CreateTaskItemCommand>();

  private fb = inject(FormBuilder);

  /** Estados disponibles para asignar al crear la tarea. */
  readonly statuses: TaskStatusOption[] = TASK_STATUSES;

  /** Estado por defecto: 1 = Pending */
  private readonly defaultStatusId = 1;

  form = this.fb.group({
    title: this.fb.control('', {
      nonNullable: true,
      validators: [Validators.required, Validators.maxLength(200)],
    }),
    description: this.fb.control('', { nonNullable: true }),
    assignedUserId: this.fb.control<number | null>(null, {
      validators: [Validators.required],
    }),
    statusId: this.fb.control<number>(this.defaultStatusId, {
      nonNullable: true,
      validators: [Validators.required],
    }),
    additionalInfo: this.fb.control('', { nonNullable: true }),
  });

  get titleControl() {
    return this.form.controls.title;
  }

  get assignedUserIdControl() {
    return this.form.controls.assignedUserId;
  }

  get statusIdControl() {
    return this.form.controls.statusId;
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const value = this.form.getRawValue();

    const command: CreateTaskItemCommand = {
      title: value.title.trim(),
      description: value.description.trim() || null,
      assignedUserId: value.assignedUserId!,
      statusId: value.statusId,
      additionalInfo: value.additionalInfo.trim() || null,
    };

    this.create.emit(command);

    this.form.reset({
      title: '',
      description: '',
      assignedUserId: null,
      statusId: this.defaultStatusId,
      additionalInfo: '',
    });
  }
}