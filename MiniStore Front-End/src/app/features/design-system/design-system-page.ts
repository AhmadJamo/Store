import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { LocalizationService } from '../../core/localization/localization.service';

@Component({
  selector: 'app-design-system-page',
  templateUrl: './design-system-page.html',
  styleUrl: './design-system-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class DesignSystemPage {
  protected readonly i18n = inject(LocalizationService);
}
