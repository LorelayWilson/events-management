import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service';

export interface PaginatedResult<T> {
  items: T[];
  totalCount: number;
}
export interface EventDto {
  id: number;
  title: string;
  description: string;
  eventDate: string;
  capacity: number;
  isPrivate: boolean;
  address?: string;
  createdAt: string;
  createdById: string;
  createdByName: string;
  registrationsCount: number;
  categories: CategoryDto[];
  isRegistered: boolean;
}

export interface CreateEventDto {
  title: string;
  description: string;
  eventDate: string;
  capacity: number;
  isPrivate: boolean;
  address?: string;
  categoryIds: number[];
  createdById?: string;
}

export interface CategoryDto {
  id: number;
  name: string;
  color: string;
  icon: string;
}

export interface RegisterEventDto {
  eventId: number;
  userId: string;
}

@Injectable({
  providedIn: 'root'
})
export class EventService {
  private readonly API_URL = 'http://localhost:5000/api';

  constructor(
    private http: HttpClient,
    private authService: AuthService
  ) {}

  private getHeaders(): HttpHeaders {
    const token = this.authService.getToken();
    return new HttpHeaders({
      'Authorization': token ? `Bearer ${token}` : '',
      'Content-Type': 'application/json'
    });
  }

  getEvents(page = 1, pageSize = 10, searchTerm?: string): Observable<PaginatedResult<EventDto>> {
    let url = `${this.API_URL}/events?page=${page}&pageSize=${pageSize}`;
    if (searchTerm && searchTerm.trim()) {
      url += `&search=${encodeURIComponent(searchTerm.trim())}`;
    }
    return this.http.get<PaginatedResult<EventDto>>(url, { headers: this.getHeaders() });
  }

  getEventsByCategory(categoryId: number, page = 1, pageSize = 10, searchTerm?: string): Observable<PaginatedResult<EventDto>> {
    let url = `${this.API_URL}/events/categories/${categoryId}?page=${page}&pageSize=${pageSize}`;
    if (searchTerm && searchTerm.trim()) {
      url += `&search=${encodeURIComponent(searchTerm.trim())}`;
    }
    return this.http.get<PaginatedResult<EventDto>>(url, { headers: this.getHeaders() });
  }

  getEvent(id: number): Observable<EventDto> {
    return this.http.get<EventDto>(`${this.API_URL}/events/${id}`, {
      headers: this.getHeaders()
    });
  }

  getCategories(): Observable<CategoryDto[]> {
    return this.http.get<CategoryDto[]>(`${this.API_URL}/events/categories`, {
      headers: this.getHeaders()
    });
  }

  createEvent(eventData: CreateEventDto): Observable<EventDto> {
    return this.http.post<EventDto>(`${this.API_URL}/events`, eventData, {
      headers: this.getHeaders()
    });
  }

  registerForEvent(eventId: number, userId: string): Observable<void> {
    const registerData: RegisterEventDto = {
      eventId: eventId,
      userId: userId
    };
    
    return this.http.post<void>(`${this.API_URL}/events/${eventId}/register`, registerData, {
      headers: this.getHeaders()
    });
  }

  unregisterFromEvent(eventId: number, userId: string): Observable<void> {
    const unregisterData: RegisterEventDto = {
      eventId: eventId,
      userId: userId
    };

    return this.http.post<void>(`${this.API_URL}/events/${eventId}/unregister`, unregisterData, {
      headers: this.getHeaders()
    });
  }

}