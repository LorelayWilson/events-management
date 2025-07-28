import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { FormControl } from '@angular/forms';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';

@Component({
  selector: 'app-search',
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.css']
})
export class SearchComponent implements OnInit {
  @Output() searchChange = new EventEmitter<string>();
  
  searchControl = new FormControl('');
  searchTerm: string = '';

  ngOnInit() {
    // Configurar debounce para evitar demasiadas búsquedas
    this.searchControl.valueChanges
      .pipe(
        debounceTime(300), // Esperar 300ms después de que el usuario deje de escribir
        distinctUntilChanged() // Solo emitir si el valor cambió
      )
      .subscribe(value => {
        this.searchTerm = value || '';
        this.searchChange.emit(this.searchTerm);
      });
  }

  clearSearch() {
    this.searchControl.setValue('');
  }
} 