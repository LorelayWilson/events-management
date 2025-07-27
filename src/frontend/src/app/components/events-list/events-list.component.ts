import { Component, OnInit } from '@angular/core';
import { EventService, EventDto, CategoryDto, PaginatedResult } from '../../services/event.service';
import { AuthService } from '../../services/auth.service';
import { ExportService } from '../../services/export.service';
import { ActivatedRoute, Router } from '@angular/router';

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

  // PAGINATION
  currentPage = 1;
  pageSize = 9;
  totalCount = 0;
  
  // SEARCH
  searchTerm: string = '';

  constructor(
    private eventService: EventService,
    public authService: AuthService,
    private exportService: ExportService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit() {
    this.loadEvents();
    this.loadCategories();
    
    // Suscribirse a los cambios en los parámetros de la URL
    this.route.queryParams.subscribe(params => {
      this.searchTerm = params['search'] || '';
      this.currentPage = 1;
      this.loadEvents();
    });
    
    this.authService.currentUser$.subscribe(user => {
      // Reload events when user changes
      this.loadEvents();
    });
  }

  loadEvents() {
    this.loading = true;
    this.error = null;
    const page = this.currentPage;
    const pageSize = this.pageSize;

    const callback = {
      next: (result: PaginatedResult<EventDto>) => {
        this.events = result.items;
        this.totalCount = result.totalCount;
        this.loading = false;
      },
      error: (error: any) => {
        this.error = 'Failed to load events';
        this.loading = false;
        console.error('Error loading events:', error);
      }
    };

    if (this.selectedCategoryId) {
      this.eventService.getEventsByCategory(this.selectedCategoryId, page, pageSize, this.searchTerm).subscribe(callback);
    } else {
      this.eventService.getEvents(page, pageSize, this.searchTerm).subscribe(callback);
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
    this.currentPage = 1;
    this.loadEvents();
  }

  // SEARCH METHODS
  onSearchChange(searchTerm: string) {
    this.searchTerm = searchTerm;
    this.currentPage = 1;
    this.loadEvents();
  }

  clearSearch() {
    this.searchTerm = '';
    this.currentPage = 1;
    this.router.navigate(['/events']);
  }

  // PAGINATION METHODS
  onPageChange(page: number) {
    this.currentPage = page;
    this.loadEvents();
  }

  get totalPages(): number {
    return Math.ceil(this.totalCount / this.pageSize);
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

  // EXPORT METHODS
  exportAllEventsToCSV() {
    this.exportService.exportEventsToCSV(this.events);
  }

  exportAllEventsToExcel() {
    this.exportService.exportEventsToExcel(this.events);
  }

  // Export all events (not just current page)
  exportAllEventsToCSVComplete() {
    this.exportAllEventsComplete('csv');
  }

  exportAllEventsToExcelComplete() {
    this.exportAllEventsComplete('excel');
  }

  // Unified method for complete export
  private exportAllEventsComplete(format: 'csv' | 'excel') {
    if (this.loading) {
      return; // Prevent multiple simultaneous exports
    }

    this.loading = true;
    const maxEvents = 1000; // Safety limit
    let loadedEvents = 0;
    let currentPage = 1;
    const pageSize = 50; // Smaller chunks for better UX
    const allEvents: EventDto[] = [];
    let totalEvents = 0;

    const loadNextPage = () => {
      if (loadedEvents >= maxEvents) {
        this.completeExport(allEvents, format);
        return;
      }

      const observable = this.selectedCategoryId 
        ? this.eventService.getEventsByCategory(this.selectedCategoryId, currentPage, pageSize, this.searchTerm)
        : this.eventService.getEvents(currentPage, pageSize, this.searchTerm);

      observable.subscribe({
        next: (result) => {
          if (result && result.items.length > 0) {
            allEvents.push(...result.items);
            loadedEvents += result.items.length;
            totalEvents = result.totalCount;

            // Show progress
            this.showExportProgress(loadedEvents, totalEvents, format);

            // Check if we need to load more
            if (result.items.length === pageSize && loadedEvents < totalEvents && loadedEvents < maxEvents) {
              currentPage++;
              // Add small delay to prevent overwhelming the server
              setTimeout(loadNextPage, 100);
            } else {
              this.completeExport(allEvents, format);
            }
          } else {
            this.completeExport(allEvents, format);
          }
        },
        error: (error) => {
          console.error(`Error loading page ${currentPage}:`, error);
          this.loading = false;
          alert(`Failed to load events for ${format.toUpperCase()} export. Please try again.`);
        }
      });
    };

    loadNextPage();
  }

  private completeExport(events: EventDto[], format: 'csv' | 'excel') {
    try {
      if (format === 'csv') {
        this.exportService.exportEventsToCSV(events);
      } else {
        this.exportService.exportEventsToExcel(events);
      }
      
      // Show success message with count
      const message = `Successfully exported ${events.length} events to ${format.toUpperCase()}`;
      if (events.length >= 1000) {
        alert(`${message} (limited to first 1000 events for performance)`);
      } else {
        alert(message);
      }
    } catch (error) {
      console.error(`Error during ${format} export:`, error);
      alert(`Failed to create ${format.toUpperCase()} file. Please try again.`);
    } finally {
      this.loading = false;
      this.hideExportProgress();
    }
  }

  private showExportProgress(loaded: number, total: number, format: string) {
    const percentage = Math.min((loaded / total) * 100, 100);
    const message = `Loading events for ${format.toUpperCase()} export... ${Math.round(percentage)}% (${loaded}/${total})`;
    console.log(message);
  }

  private hideExportProgress() {
    // Hide progress indicator
    console.log('Export completed');
  }

}