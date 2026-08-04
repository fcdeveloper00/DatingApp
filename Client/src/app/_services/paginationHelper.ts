import { HttpParams, HttpResponse } from '@angular/common/http';
import { Member } from '../_models/member';
import { UserParams } from '../_models/userParams';
import { signal } from '@angular/core';
import { PaginatedResult } from '../_models/pagination';

export function setPaginatedResult<T>(
  response: HttpResponse<T>,
  paginatedResultSignal: ReturnType<typeof signal<PaginatedResult<T> | null>>,
) {
  paginatedResultSignal.set({
    items: response.body as T,
    pagination: JSON.parse(response.headers.get('Pagination')!),
  });
}

export function setPaginationHeader(
  /* userParams:UserParams */ pageNumber: number,
  pageSize: number,
): HttpParams {
  let params = new HttpParams();
  if (pageNumber && pageSize) {
    params = params.append('pageSize', pageSize);
    params = params.append('pageNumber', pageNumber);
  }
  return params;
}
