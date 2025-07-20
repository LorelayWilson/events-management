import { Component, OnInit } from '@angular/core';
import { EventService, EventDto, CategoryDto } from '../services/event.service';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-events-list',
  templateUrl: './events-list.component.html',
  styleUrls: ['./events-list.component.css']
})
export class EventsListComponent implements OnInit {
  events: EventDto[] = [];
  categories: CategoryDto[] = [];
  selectedCategoryId: number | null = null;
  loading = false;
  error: string | null = null;

  constructor(
    private eventService: EventService,
    public authService: AuthService
  ) {}

  ngOnInit() {
    this.loadEvents();
    this.loadCategories();
    
    this.authService.currentUser$.subscribe(user => {
      // Reload events when user changes
      this.loadEvents();
    });
  }

  loadEvents() {
    this.loading = true;
    this.error = null;
    
    if (this.selectedCategoryId) {
      this.eventService.getEventsByCategory(this.selectedCategoryId).subscribe({
        next: (events) => {
          this.events = events;
          this.loading = false;
        },
        error: (error) => {
          this.error = 'Failed to load events';
          this.loading = false;
          console.error('Error loading events:', error);
        }
      });
    } else {
      this.eventService.getEvents().subscribe({
        next: (events) => {
          this.events = events;
          this.loading = false;
        },
        error: (error) => {
          this.error = 'Failed to load events';
          this.loading = false;
          console.error('Error loading events:', error);
        }
      });
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

  onCategoryChange() {
    this.loadEvents();
  }

  registerForEvent(eventId: number) {
    if (!this.authService.isLoggedIn()) {
      alert('Please log in to register for events');
      return;
    }

    const currentUser = this.authService.getCurrentUser();
    if (!currentUser) {
      alert('Please log in to register for events');
      return;
    }

    this.eventService.registerForEvent(eventId, currentUser.userId).subscribe({
      next: () => {
        alert('Successfully registered for event!');
        this.loadEvents(); // Reload to update registration status
      },
      error: (error) => {
        alert('Failed to register for event');
        console.error('Error registering for event:', error);
      }
    });
  }

}