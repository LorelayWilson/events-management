import { Injectable } from '@angular/core';
import * as XLSX from 'xlsx';
import { saveAs } from 'file-saver';
import { EventDto } from './event.service';

@Injectable({
  providedIn: 'root'
})
export class ExportService {

  constructor() {}

  /**
   * Exports a single event to CSV
   */
  exportEventToCSV(event: EventDto): void {
    const csvContent = this.convertEventToCSV(event);
    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
    const fileName = `event_${event.id}_${this.formatDateForFileName(event.eventDate)}.csv`;
    saveAs(blob, fileName);
  }

  /**
   * Exports a single event to Excel
   */
  exportEventToExcel(event: EventDto): void {
    const worksheet = this.convertEventToWorksheet(event);
    const workbook = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(workbook, worksheet, 'Event Details');
    
    const excelBuffer = XLSX.write(workbook, { bookType: 'xlsx', type: 'array' });
    const blob = new Blob([excelBuffer], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
    const fileName = `event_${event.id}_${this.formatDateForFileName(event.eventDate)}.xlsx`;
    saveAs(blob, fileName);
  }

  /**
   * Exports multiple events to CSV
   */
  exportEventsToCSV(events: EventDto[]): void {
    const csvContent = this.convertEventsToCSV(events);
    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
    const fileName = `events_${this.formatDateForFileName(new Date())}.csv`;
    saveAs(blob, fileName);
  }

  /**
   * Exports multiple events to Excel
   */
  exportEventsToExcel(events: EventDto[]): void {
    const worksheet = this.convertEventsToWorksheet(events);
    const workbook = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(workbook, worksheet, 'Events List');
    
    const excelBuffer = XLSX.write(workbook, { bookType: 'xlsx', type: 'array' });
    const blob = new Blob([excelBuffer], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
    const fileName = `events_${this.formatDateForFileName(new Date())}.xlsx`;
    saveAs(blob, fileName);
  }

  /**
   * Converts a single event to CSV format
   */
  private convertEventToCSV(event: EventDto): string {
    const headers = [
      'ID', 'Title', 'Description', 'Event Date', 'Capacity',
      'Registrations Count', 'Available Spots', 'Is Private',
      'Address', 'Created By', 'Created At', 'Categories'
    ];

    const values = [
      event.id,
      `"${event.title}"`,
      `"${event.description}"`,
      new Date(event.eventDate).toLocaleString(),
      event.capacity,
      event.registrationsCount,
      event.capacity - event.registrationsCount,
      event.isPrivate ? 'Yes' : 'No',
      event.address ?? '',
      event.createdByName,
      new Date(event.createdAt).toLocaleString(),
      `"${event.categories.map(c => c.name).join(', ')}"`
    ];

    return [headers.join(','), values.join(',')].join('\n');
  }

  /**
   * Converts multiple events to CSV format
   */
  private convertEventsToCSV(events: EventDto[]): string {
    const headers = [
      'ID', 'Title', 'Description', 'Event Date', 'Capacity',
      'Registrations Count', 'Available Spots', 'Is Private',
      'Address', 'Created By', 'Created At', 'Categories'
    ];

    const rows = events.map(event => [
      event.id,
      `"${event.title}"`,
      `"${event.description}"`,
      new Date(event.eventDate).toLocaleString(),
      event.capacity,
      event.registrationsCount,
      event.capacity - event.registrationsCount,
      event.isPrivate ? 'Yes' : 'No',
      event.address ?? '',
      event.createdByName,
      new Date(event.createdAt).toLocaleString(),
      `"${event.categories.map(c => c.name).join(', ')}"`
    ]);

    return [headers.join(','), ...rows.map(row => row.join(','))].join('\n');
  }

  /**
   * Converts a single event to Excel worksheet
   */
  private convertEventToWorksheet(event: EventDto): XLSX.WorkSheet {
    const data = [
      ['Event Details'],
      [''],
      ['ID', event.id],
      ['Title', event.title],
      ['Description', event.description],
      ['Event Date', new Date(event.eventDate).toLocaleString()],
      ['Capacity', event.capacity],
      ['Registrations Count', event.registrationsCount],
      ['Available Spots', event.capacity - event.registrationsCount],
      ['Is Private', event.isPrivate ? 'Yes' : 'No'],
      ['Address', event.address ?? ''],
      ['Created By', event.createdByName],
      ['Created At', new Date(event.createdAt).toLocaleString()],
      ['Categories', event.categories.map(c => c.name).join(', ')],
      [''],
      ['Registration Details'],
      [''],
      ['Total Capacity', event.capacity],
      ['Registered Users', event.registrationsCount],
      ['Available Spots', event.capacity - event.registrationsCount],
      ['Registration Rate', `${((event.registrationsCount / event.capacity) * 100).toFixed(1)}%`]
    ];

    return XLSX.utils.aoa_to_sheet(data);
  }

  /**
   * Converts multiple events to Excel worksheet
   */
  private convertEventsToWorksheet(events: EventDto[]): XLSX.WorkSheet {
    const headers = [
      'ID', 'Title', 'Description', 'Event Date', 'Capacity',
      'Registrations Count', 'Available Spots', 'Is Private',
      'Address', 'Created By', 'Created At', 'Categories'
    ];

    const data = [
      headers,
      ...events.map(event => [
        event.id,
        event.title,
        event.description,
        new Date(event.eventDate).toLocaleString(),
        event.capacity,
        event.registrationsCount,
        event.capacity - event.registrationsCount,
        event.isPrivate ? 'Yes' : 'No',
        event.address ?? '',
        event.createdByName,
        new Date(event.createdAt).toLocaleString(),
        event.categories.map(c => c.name).join(', ')
      ])
    ];

    return XLSX.utils.aoa_to_sheet(data);
  }

  /**
   * Formats date for filename
   */
  private formatDateForFileName(date: Date | string): string {
    const d = new Date(date);
    return d.toISOString().split('T')[0]; // YYYY-MM-DD format
  }
} 