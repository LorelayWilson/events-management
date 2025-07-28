import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { EventService, EventDto, PaginatedResult } from '../../services/event.service';
import { AuthService, LoginResponseDto } from '../../services/auth.service';
import { ExportService } from '../../services/export.service';

@Component({
  selector: 'app-user-profile',
  templateUrl: './user-profile.component.html',
  styleUrls: ['./user-profile.component.css']
})
export class UserProfileComponent implements OnInit {
  currentUser: LoginResponseDto | null = null;
  targetUser: LoginResponseDto | null = null;
  targetUserId: string | null = null;
  isOwnProfile = true;
  
  // All events loaded for filtering
  allEvents: EventDto[] = [];
  allEventsLoading = false;
  
  // Events created by the user
  myEvents: EventDto[] = [];
  myEventsPage = 1;
  myEventsPageSize = 10;
  myEventsTotalCount = 0;
  
  // Public events from other users
  publicEvents: EventDto[] = [];
  publicEventsPage = 1;
  publicEventsPageSize = 10;
  publicEventsTotalCount = 0;
  
  // Events the user is registered for
  registeredEvents: EventDto[] = [];
  registeredEventsPage = 1;
  registeredEventsPageSize = 10;
  registeredEventsTotalCount = 0;
  
  activeTab: 'my-events' | 'public-events' | 'registered-events' = 'my-events';

  constructor(
    private eventService: EventService,
    private authService: AuthService,
    private exportService: ExportService,
    public router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.currentUser = this.authService.getCurrentUser();
    if (!this.currentUser) {
      this.router.navigate(['/login']);
      return;
    }
    
    // Check if we're viewing a specific user's events or our own profile
    this.route.paramMap.subscribe(params => {
      const userIdParam = params.get('id');
      if (userIdParam) {
        this.targetUserId = userIdParam;
        this.isOwnProfile = this.currentUser?.userId === userIdParam;
      } else {
        this.targetUserId = this.currentUser?.userId || null;
        this.isOwnProfile = true;
      }
      
      // Set target user based on context
      if (this.isOwnProfile) {
        this.targetUser = this.currentUser;
      } else {
        // For other users, we'll get the name from the events data
        this.targetUser = {
          userId: this.targetUserId!,
          firstName: 'User',
          lastName: this.targetUserId!.substring(0, 8),
          email: 'user@example.com',
          token: ''
        };
      }
      
      this.updateMyEventsPage();
      
      // Only load all events for filtering if it's the user's own profile
      if (this.isOwnProfile) {
        this.loadAllEvents();
      }
    });
  }

  loadAllEvents(): void {
    this.allEventsLoading = true;
    // Load all events for filtering
    this.eventService.getEvents(1, 1000).subscribe({
      next: (result: PaginatedResult<EventDto>) => {
        this.allEvents = result.items;
        this.allEventsLoading = false;
        this.filterAndPaginateEvents();
      },
      error: (error) => {
        console.error('Error loading events:', error);
        this.allEventsLoading = false;
      }
    });
  }

  filterAndPaginateEvents(): void {
    if (!this.currentUser) return;

    // Filter public events
    const publicEventsFiltered = this.allEvents.filter(event => 
      !event.isPrivate && event.createdById !== this.currentUser!.userId
    );
    this.publicEventsTotalCount = publicEventsFiltered.length;
    this.updatePublicEventsPage();

    // Filter registered events
    const registeredEventsFiltered = this.allEvents.filter(event => 
      event.isRegistered
    );
    this.registeredEventsTotalCount = registeredEventsFiltered.length;
    this.updateRegisteredEventsPage();
  }



  updateMyEventsPage(): void {
    if (!this.targetUserId) return;
    this.eventService.getEventsByUser(this.targetUserId, this.myEventsPage, this.myEventsPageSize)
      .subscribe({
        next: (result: PaginatedResult<EventDto>) => {
          this.myEvents = result.items;
          this.myEventsTotalCount = result.totalCount;
          
          // Extract user name from the first event if available
          if (!this.isOwnProfile && result.items.length > 0) {
            const firstEvent = result.items[0];
            if (firstEvent.createdByName) {
              // Parse the full name into first and last name
              const nameParts = firstEvent.createdByName.split(' ');
              this.targetUser = {
                userId: this.targetUserId!,
                firstName: nameParts[0] || 'User',
                lastName: nameParts.slice(1).join(' ') || this.targetUserId!.substring(0, 8),
                email: 'user@example.com',
                token: ''
              };
            }
          }
        },
        error: (error) => {
          console.error('Error loading user events:', error);
        }
      });
  }

  updatePublicEventsPage(): void {
    const startIndex = (this.publicEventsPage - 1) * this.publicEventsPageSize;
    const endIndex = startIndex + this.publicEventsPageSize;
    const publicEventsFiltered = this.allEvents.filter(event => 
      !event.isPrivate && event.createdById !== this.currentUser!.userId
    );
    this.publicEvents = publicEventsFiltered.slice(startIndex, endIndex);
  }

  updateRegisteredEventsPage(): void {
    const startIndex = (this.registeredEventsPage - 1) * this.registeredEventsPageSize;
    const endIndex = startIndex + this.registeredEventsPageSize;
    const registeredEventsFiltered = this.allEvents.filter(event => 
      event.isRegistered
    );
    this.registeredEvents = registeredEventsFiltered.slice(startIndex, endIndex);
  }

  cancelRegistration(eventId: number): void {
    if (!this.currentUser) return;

    this.eventService.unregisterFromEvent(eventId, this.currentUser.userId).subscribe({
      next: () => {
        alert('Registration cancelled');
        this.loadAllEvents();
      },
      error: (error) => {
        console.error('Error cancelling registration:', error);
        alert('Failed to cancel registration');
      }
    });
  }

  canDelete(event: EventDto): boolean {
    const currentUser = this.authService.getCurrentUser();
    return this.authService.isLoggedIn() &&
           currentUser !== null &&
           event.createdById === currentUser.userId;
  }

  deleteEvent(eventId: number): void {
    if (!this.currentUser) return;

    if (!confirm('Are you sure you want to delete this event? This action cannot be undone.')) {
      return;
    }

    this.eventService.deleteEvent(eventId).subscribe({
      next: () => {
        alert('Event deleted successfully');
        this.loadAllEvents();
      },
      error: (error: any) => {
        console.error('Error deleting event:', error);
        alert('Failed to delete event');
      }
    });
  }

  onTabChange(tab: 'my-events' | 'public-events' | 'registered-events'): void {
    this.activeTab = tab;
    
    // Load appropriate data based on tab
    if (tab === 'public-events' && this.isOwnProfile) {
      this.loadAllEvents();
    } else if (tab === 'registered-events' && this.isOwnProfile) {
      this.loadAllEvents();
    }
  }

  onEventClick(eventId: number): void {
    this.router.navigate(['/events', eventId]);
  }

  onCreateEvent(): void {
    this.router.navigate(['/create-event']);
  }

  onMyEventsPageChange(page: number): void {
    this.myEventsPage = page;
    this.updateMyEventsPage();
  }

  onPublicEventsPageChange(page: number): void {
    this.publicEventsPage = page;
    this.updatePublicEventsPage();
  }

  onRegisteredEventsPageChange(page: number): void {
    this.registeredEventsPage = page;
    this.updateRegisteredEventsPage();
  }

  // Export methods
  exportMyEventsToCSV(): void {
    const myEventsFiltered = this.allEvents.filter(event => 
      event.createdById === this.currentUser!.userId
    );
    this.exportService.exportEventsToCSV(myEventsFiltered);
  }

  exportPublicEventsToCSV(): void {
    const publicEventsFiltered = this.allEvents.filter(event => 
      !event.isPrivate && event.createdById !== this.currentUser!.userId
    );
    this.exportService.exportEventsToCSV(publicEventsFiltered);
  }

  exportRegisteredEventsToCSV(): void {
    const registeredEventsFiltered = this.allEvents.filter(event => 
      event.isRegistered
    );
    this.exportService.exportEventsToCSV(registeredEventsFiltered);
  }

  exportMyEventsToExcel(): void {
    const myEventsFiltered = this.allEvents.filter(event => 
      event.createdById === this.currentUser!.userId
    );
    this.exportService.exportEventsToExcel(myEventsFiltered);
  }

  exportPublicEventsToExcel(): void {
    const publicEventsFiltered = this.allEvents.filter(event => 
      !event.isPrivate && event.createdById !== this.currentUser!.userId
    );
    this.exportService.exportEventsToExcel(publicEventsFiltered);
  }

  exportRegisteredEventsToExcel(): void {
    const registeredEventsFiltered = this.allEvents.filter(event => 
      event.isRegistered
    );
    this.exportService.exportEventsToExcel(registeredEventsFiltered);
  }

  formatDate(dateString: string): string {
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  logout(): void {
    this.authService.logout();
  }
} 