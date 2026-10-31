import { Component, signal } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { SafeFormShortcutsDirective } from './safe-form-shortcuts.directive';

@Component({ imports: [SafeFormShortcutsDirective], template: '<form appSafeFormShortcuts [saving]="saving()" (saveShortcut)="recordSave()"><input /></form>' })
class ShortcutHost { saving = signal(false); saves = 0; recordSave(): void { this.saves += 1; } }

describe('SafeFormShortcutsDirective', () => {
  let fixture: ComponentFixture<ShortcutHost>;
  beforeEach(async () => { await TestBed.configureTestingModule({ imports: [ShortcutHost] }).compileComponents(); fixture = TestBed.createComponent(ShortcutHost); fixture.detectChanges(); });

  it('emits once for Ctrl+S and prevents the browser save action', () => {
    const event = new KeyboardEvent('keydown', { key: 's', ctrlKey: true, bubbles: true, cancelable: true });
    fixture.nativeElement.querySelector('input').dispatchEvent(event);
    expect(fixture.componentInstance.saves).toBe(1);
    expect(event.defaultPrevented).toBe(true);
  });

  it('does not emit repeated shortcuts while saving', () => {
    fixture.componentInstance.saving.set(true); fixture.detectChanges();
    fixture.nativeElement.querySelector('input').dispatchEvent(new KeyboardEvent('keydown', { key: 's', ctrlKey: true, bubbles: true, cancelable: true }));
    expect(fixture.componentInstance.saves).toBe(0);
  });
});
