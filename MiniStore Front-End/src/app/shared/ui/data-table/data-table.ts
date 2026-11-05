import { NgTemplateOutlet } from '@angular/common';
import {
  ChangeDetectionStrategy,
  Component,
  Directive,
  TemplateRef,
  contentChildren,
  inject,
  input,
  output,
} from '@angular/core';

export type DataTableColumnKind = 'text' | 'code' | 'number' | 'date' | 'status' | 'actions';
export type DataTableSortDirection = 'asc' | 'desc';

export interface DataTableColumn {
  readonly key: string;
  readonly header: string;
  readonly kind?: DataTableColumnKind;
  readonly sortable?: boolean;
  readonly width?: string;
}

export interface DataTableSort {
  readonly key: string;
  readonly direction: DataTableSortDirection;
}

@Directive({ selector: 'ng-template[appDataCell]' })
export class DataTableCellDirective {
  readonly key = input.required<string>({ alias: 'appDataCell' });
  readonly template = inject(TemplateRef<unknown>);

  static ngTemplateContextGuard(
    _directive: DataTableCellDirective,
    context: unknown,
  ): context is { $implicit: any; column: DataTableColumn } {
    return true;
  }
}

@Component({
  selector: 'app-data-table',
  imports: [NgTemplateOutlet],
  templateUrl: './data-table.html',
  styleUrl: './data-table.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class DataTableComponent {
  readonly label = input.required<string>();
  readonly columns = input<readonly DataTableColumn[]>([]);
  readonly rows = input<readonly unknown[]>([]);
  readonly rowKey = input<string>('id');
  readonly loading = input(false);
  readonly error = input('');
  readonly loadingTitle = input('');
  readonly emptyTitle = input('');
  readonly emptyDescription = input('');
  readonly retryLabel = input('');
  readonly sort = input<DataTableSort | null>(null);
  readonly sortChange = output<DataTableSort>();
  readonly retry = output<void>();
  readonly cellTemplates = contentChildren(DataTableCellDirective);

  protected cellTemplate(key: string): TemplateRef<unknown> | null {
    return this.cellTemplates().find(candidate => candidate.key() === key)?.template ?? null;
  }

  protected value(row: unknown, key: string): unknown {
    return row && typeof row === 'object' ? (row as Record<string, unknown>)[key] : '';
  }

  protected trackRow(index: number, row: unknown): unknown {
    return this.value(row, this.rowKey()) ?? index;
  }

  protected requestSort(column: DataTableColumn): void {
    if (!column.sortable) return;
    const current = this.sort();
    this.sortChange.emit({
      key: column.key,
      direction: current?.key === column.key && current.direction === 'asc' ? 'desc' : 'asc',
    });
  }

  protected ariaSort(column: DataTableColumn): 'ascending' | 'descending' | 'none' | null {
    if (!column.sortable) return null;
    const current = this.sort();
    if (current?.key !== column.key) return 'none';
    return current.direction === 'asc' ? 'ascending' : 'descending';
  }
}
