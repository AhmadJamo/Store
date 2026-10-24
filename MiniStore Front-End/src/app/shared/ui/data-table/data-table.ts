import { ChangeDetectionStrategy, Component, input } from '@angular/core';

@Component({ selector: 'app-data-table', templateUrl: './data-table.html', styleUrl: './data-table.scss', changeDetection: ChangeDetectionStrategy.OnPush })
export class DataTableComponent { readonly label = input.required<string>(); }
