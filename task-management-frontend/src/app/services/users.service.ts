import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BaseApiService } from './base-api.service';
import {
  CreateUserCommand,
  UserResponse,
  UsersQueryParams,
  Resource,
  ResourceCollection,
} from '../core/models';

@Injectable({ providedIn: 'root' })
export class UsersService extends BaseApiService {
  private readonly endpoint = `${this.baseUrl}/Users`;

 
  create(command: CreateUserCommand): Observable<Resource<UserResponse>> {
    return this.http.post<Resource<UserResponse>>(this.endpoint, command);
  }


  getAll(
    params: UsersQueryParams = {}
  ): Observable<ResourceCollection<Resource<UserResponse>>> {
    return this.http.get<ResourceCollection<Resource<UserResponse>>>(
      this.endpoint,
      { params: this.buildParams(params) }
    );
  }

  getList(
    params: UsersQueryParams = {}
  ): Observable<ResourceCollection<UserResponse>> {
    return this.http.get<ResourceCollection<UserResponse>>(
      `${this.endpoint}/list`,
      { params: this.buildParams(params) ,
         headers: { Accept: 'application/json' },
      }
    );
  }

  getById(id: number): Observable<Resource<UserResponse>> {
    return this.http.get<Resource<UserResponse>>(`${this.endpoint}/${id}`);
  }
}