import { TestBed } from '@angular/core/testing';
import { FormFocusService } from './form-focus.service';

describe('FormFocusService', () => {
  it('focuses the first invalid control and safely maps server field names', async () => {
    const host = document.createElement('div');
    host.innerHTML = '<input formControlName="name" class="ng-invalid"><input formControlName="email">';
    document.body.append(host);
    const service = TestBed.inject(FormFocusService);
    service.focusFirstInvalid(host); await Promise.resolve();
    expect(document.activeElement?.getAttribute('formControlName')).toBe('name');
    service.focusField(host, 'Form.Email'); await Promise.resolve();
    expect(document.activeElement?.getAttribute('formControlName')).toBe('email');
    host.remove();
  });

  it('ignores unsafe server field selectors', async () => {
    const host = document.createElement('div'); host.innerHTML = '<input formControlName="name">'; document.body.append(host);
    TestBed.inject(FormFocusService).focusField(host, 'name"] body'); await Promise.resolve();
    expect(document.activeElement).not.toBe(host.querySelector('input'));
    host.remove();
  });
});
