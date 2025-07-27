import { Component, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-export-buttons',
  templateUrl: './export-buttons.component.html',
  styleUrls: ['./export-buttons.component.css']
})
export class ExportButtonsComponent {
  @Input() label: string = 'Export';
  @Input() showButtons: boolean = true;
  @Input() csvDisabled: boolean = false;
  @Input() excelDisabled: boolean = false;
  @Input() csvTooltip: string = 'Export to CSV';
  @Input() excelTooltip: string = 'Export to Excel';
  @Input() showLabel: boolean = true;
  @Input() buttonSize: 'sm' | 'md' | 'lg' = 'sm';
  
  @Output() exportCsv = new EventEmitter<void>();
  @Output() exportExcel = new EventEmitter<void>();

  onExportCsv(): void {
    if (!this.csvDisabled) {
      this.exportCsv.emit();
    }
  }

  onExportExcel(): void {
    if (!this.excelDisabled) {
      this.exportExcel.emit();
    }
  }

  get buttonClass(): string {
    return `btn-${this.buttonSize}`;
  }
} 