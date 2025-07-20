import { Component, OnInit } from '@angular/core';
import { AuthService } from './services/auth.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'Events Management System';

  constructor(public authService: AuthService) {}

  ngOnInit() {
    // Try to restore user session from localStorage
    this.authService.checkAuthStatus();
  }

  logout() {
    this.authService.logout();
  }
}