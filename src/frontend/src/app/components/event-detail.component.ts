import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { EventService, EventDto } from '../services/event.service';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-event-detail',
  templateUrl: './event-detail.component.html',
  styleUrls: ['./event-detail.component.css']
})
export class EventDetailComponent implements OnInit {
  event: EventDto | null = null;
  loading = false;
  error: string | null = null;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private eventService: EventService,
    public authService: AuthService
  ) {}

  ngOnInit() {
    const eventId = this.route.snapshot.params['id'];
    if (eventId) {
      this.loadEvent(+eventId);
    }
  }

  loadEvent(eventId: number) {
    this.loading = true;
    this.error = null;

    this.eventService.getEvent(eventId).subscribe({
      next: (event) => {
        this.event = event;
        this.loading = false;
      },
      error: (error) => {
        this.error = 'Event not found or failed to load';
        this.loading = false;
        console.error('Error loading event:', error);
      }
    });
  }

  registerForEvent() {
    if (!this.event || !this.authService.isLoggedIn()) {
      alert('Please log in to register for events');
      return;
    }

    if (this.event.isRegistered) {
      alert('You are already registered for this event');
      return;
    }

    if (this.event.registrationsCount >= this.event.capacity) {
      alert('This event is at full capacity');
      return;
    }

    const currentUser = this.authService.getCurrentUser();
    if (!currentUser) {
      alert('Please log in to register for events');
      return;
    }

    this.eventService.registerForEvent(this.event.id, currentUser.userId).subscribe({
      next: () => {
        alert('Successfully registered for event!');
        this.loadEvent(this.event!.id); // Reload to update registration status
      },
      error: (error) => {
        alert('Failed to register for event');
        console.error('Error registering for event:', error);
      }
    });
  }

  goBack() {
    this.router.navigate(['/events']);
  }

  getAvailableSpots(): number {
    return this.event ? this.event.capacity - this.event.registrationsCount : 0;
  }

  isEventFull(): boolean {
    return this.getAvailableSpots() <= 0;
  }

  isEventPast(): boolean {
    if (!this.event) return false;
    return new Date(this.event.eventDate) < new Date();
  }

  canRegister(): boolean {
    return this.authService.isLoggedIn() && 
           this.event !== null &&
           !this.event.isRegistered && 
           !this.isEventFull() && 
           !this.isEventPast();
  }

}