import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BaseApiService } from './base-api.service';
import {
  CreateTaskItemCommand,
  TaskItemResponse,
  TaskItemsQueryParams,
  UpdateTaskItemCommand,
  Resource,
  ResourceCollection,
} from '../core/models';

@Injectable({ providedIn: 'root' })
export class TaskItemsService extends BaseApiService {
  private readonly endpoint = `${this.baseUrl}/TaskItems`;


  create(command: CreateTaskItemCommand): Observable<Resource<TaskItemResponse>> {
    return this.http.post<Resource<TaskItemResponse>>(this.endpoint, command);
  }


  getAll(
    params: TaskItemsQueryParams = {}
  ): Observable<ResourceCollection<Resource<TaskItemResponse>>> {
    return this.http.get<ResourceCollection<Resource<TaskItemResponse>>>(
      this.endpoint,
      { params: this.buildParams(params) }
    );
  }


  getList(
    params: TaskItemsQueryParams = {}
  ): Observable<ResourceCollection<TaskItemResponse>> {
    return this.http.get<ResourceCollection<TaskItemResponse>>(
      `${this.endpoint}/list`,
      { params: this.buildParams(params),
         headers: { Accept: 'application/json' },
       }
    );
  }


  getById(id: number): Observable<Resource<TaskItemResponse>> {
    return this.http.get<Resource<TaskItemResponse>>(`${this.endpoint}/${id}`);
  }


  updateStatus(id: number, command: UpdateTaskItemCommand): Observable<void> {
    return this.http.put<void>(`${this.endpoint}/${id}/status`, command);
  }
}