import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService, RegisterDto } from '../../services/auth.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {
  registerData: RegisterDto = {
    email: '',
    password: '',
    firstName: '',
    lastName: ''
  };
  
  loading = false;
  error: string | null = null;

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  onSubmit() {
    if (!this.registerData.email || !this.registerData.password || 
        !this.registerData.firstName || !this.registerData.lastName) {
      this.error = 'Please fill in all fields';
      return;
    }

    this.loading = true;
    this.error = null;

    this.authService.register(this.registerData).subscribe({
      next: (response) => {
        this.loading = false;
        this.router.navigate(['/events']);
      },
      error: (error) => {
        this.loading = false;
        this.error = 'Registration failed. Please try again.';
        console.error('Registration error:', error);
      }
    });
  }
}