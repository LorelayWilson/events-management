import { Component, OnInit } from '@angular/core';
import { AuthService } from './services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'Events Management System';

  constructor(public authService: AuthService, private router: Router) {}

  ngOnInit() {
    // Try to restore user session from localStorage
    this.authService.checkAuthStatus();
  }

  logout() {
    this.authService.logout();
  }

  onSearchChange(searchTerm: string) {
    // Navegar a la lista de eventos con el término de búsqueda
    if (searchTerm.trim()) {
      this.router.navigate(['/events'], { queryParams: { search: searchTerm } });
    } else {
      this.router.navigate(['/events']);
    }
  }
}