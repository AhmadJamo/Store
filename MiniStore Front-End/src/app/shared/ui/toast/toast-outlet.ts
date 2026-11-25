import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { LocalizationService } from '../../../core/localization/localization.service';
import { ToastService } from './toast.service';

@Component({
  selector: 'app-toast-outlet',
  templateUrl: './toast-outlet.html',
  styleUrl: './toast-outlet.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ToastOutletComponent {
  protected readonly toasts = inject(ToastService);
  protected readonly i18n = inject(LocalizationService);
}