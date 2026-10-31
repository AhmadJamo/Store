import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ApiSessionService } from '../../core/api/api-session.service';
import { LocalizationService } from '../../core/localization/localization.service';

@Component({
  selector: 'app-access-denied-page',
  imports: [RouterLink],
  templateUrl: './access-denied-page.html',
  styleUrl: './access-denied-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AccessDeniedPageComponent {
  protected readonly i18n = inject(LocalizationService);
  protected readonly session = inject(ApiSessionService);
}
