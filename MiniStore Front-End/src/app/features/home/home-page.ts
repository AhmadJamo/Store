import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { LocalizationService, MessageKey } from '../../core/localization/localization.service';

@Component({
  selector: 'app-home-page',
  templateUrl: './home-page.html',
  styleUrl: './home-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HomePage {
  protected readonly i18n = inject(LocalizationService);
  protected readonly milestones: readonly { order: string; key: MessageKey; state: 'done' | 'next' }[] = [
    { order: '01', key: 'foundation', state: 'done' },
    { order: '02', key: 'api', state: 'next' },
    { order: '03', key: 'navigation', state: 'next' },
    { order: '04', key: 'customers', state: 'next' },
  ];
}
