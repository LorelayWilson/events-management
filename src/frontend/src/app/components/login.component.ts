import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService, LoginDto } from '../services/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  loginData: LoginDto = {
    email: '',
    password: ''
  };
  
  loading = false;
  error: string | null = null;

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  onSubmit() {
    if (!this.loginData.email || !this.loginData.password) {
      this.error = 'Please fill in all fields';
      return;
    }

    this.loading = true;
    this.error = null;

    this.authService.login(this.loginData).subscribe({
      next: (response) => {
        this.loading = false;
        this.router.navigate(['/events']);
      },
      error: (error) => {
        this.loading = false;
        this.error = 'Invalid email or password';
        console.error('Login error:', error);
      }
    });
  }
}