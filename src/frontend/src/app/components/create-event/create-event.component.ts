import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { EventService, CreateEventDto, CategoryDto } from '../../services/event.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-create-event',
  templateUrl: './create-event.component.html',
  styleUrls: ['./create-event.component.css']
})
export class CreateEventComponent implements OnInit {
  eventData: CreateEventDto = {
    title: '',
    description: '',
    eventDate: '',
    capacity: 10,
    isPrivate: false,
    categoryIds: [],
    createdById: ''
  };
  
  categories: CategoryDto[] = [];
  loading = false;
  error: string | null = null;

  constructor(
    private eventService: EventService,
    private authService: AuthService,
    public router: Router
  ) {}

  ngOnInit() {
    if (!this.authService.isLoggedIn()) {
      this.router.navigate(['/login']);
      return;
    }

    this.loadCategories();
    
    const currentUser = this.authService.getCurrentUser();
    if (currentUser) {
      this.eventData.createdById = currentUser.userId;
    }
  }

  loadCategories() {
    this.eventService.getCategories().subscribe({
      next: (categories) => {
        this.categories = categories;
      },
      error: (error) => {
        console.error('Error loading categories:', error);
      }
    });
  }

  onCategoryChange(categoryId: number, isChecked: boolean) {
    if (isChecked) {
      this.eventData.categoryIds.push(categoryId);
    } else {
      const index = this.eventData.categoryIds.indexOf(categoryId);
      if (index > -1) {
        this.eventData.categoryIds.splice(index, 1);
      }
    }
  }

  isCategorySelected(categoryId: number): boolean {
    return this.eventData.categoryIds.includes(categoryId);
  }

  setEventVisibility(isPrivate: boolean) {
    this.eventData.isPrivate = isPrivate;
  }

  onSubmit() {
    if (!this.eventData.title || !this.eventData.description || !this.eventData.eventDate) {
      this.error = 'Please fill in all required fields';
      return;
    }

    if (this.eventData.capacity <= 0) {
      this.error = 'Capacity must be greater than 0';
      return;
    }

    this.loading = true;
    this.error = null;

    this.eventService.createEvent(this.eventData).subscribe({
      next: (event) => {
        this.loading = false;
        this.router.navigate(['/events', event.id]);
      },
      error: (error) => {
        this.loading = false;
        this.error = 'Failed to create event. Please try again.';
        console.error('Error creating event:', error);
      }
    });
  }

}