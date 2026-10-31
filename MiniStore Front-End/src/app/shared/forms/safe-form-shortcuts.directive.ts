import { Directive, HostListener, input, output } from '@angular/core';

@Directive({ selector: 'form[appSafeFormShortcuts]' })
export class SafeFormShortcutsDirective {
  readonly saving = input(false);
  readonly saveShortcut = output<void>();

  @HostListener('keydown', ['$event'])
  handleKeydown(event: KeyboardEvent): void {
    if (!(event.ctrlKey || event.metaKey) || event.altKey || event.key.toLocaleLowerCase() !== 's') return;
    event.preventDefault();
    if (!event.repeat && !this.saving()) this.saveShortcut.emit();
  }
}
