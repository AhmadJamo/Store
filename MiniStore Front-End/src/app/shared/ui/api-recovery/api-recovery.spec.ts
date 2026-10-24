import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { ApiRecoveryComponent } from './api-recovery';

describe('ApiRecoveryComponent', () => {
  beforeEach(() => TestBed.configureTestingModule({ imports: [ApiRecoveryComponent], providers: [provideRouter([])] }));

  it('offers retry for a transient read failure', () => {
    const fixture = TestBed.createComponent(ApiRecoveryComponent);
    fixture.componentRef.setInput('problem', { kind: 'transient', canRetry: true, status: 503 });
    fixture.detectChanges();
    expect(fixture.nativeElement.querySelector('button')).toBeTruthy();
  });

  it('does not offer retry for forbidden or mutation-disabled failures', () => {
    const fixture = TestBed.createComponent(ApiRecoveryComponent);
    fixture.componentRef.setInput('problem', { kind: 'forbidden', canRetry: false, status: 403 });
    fixture.componentRef.setInput('allowRetry', false);
    fixture.detectChanges();
    expect(fixture.nativeElement.querySelector('.recovery-actions button')).toBeFalsy();
  });

  it('shows a support reference without exposing a stack trace', () => {
    const fixture = TestBed.createComponent(ApiRecoveryComponent);
    fixture.componentRef.setInput('problem', { kind: 'unknown', canRetry: false, correlationId: 'trace-123' });
    fixture.detectChanges();
    expect(fixture.nativeElement.textContent).toContain('trace-123');
    expect(fixture.nativeElement.textContent).not.toContain('stack');
  });
});
