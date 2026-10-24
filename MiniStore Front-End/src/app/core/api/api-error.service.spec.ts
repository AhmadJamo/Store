import { HttpErrorResponse } from '@angular/common/http';
import { ApiErrorService } from './api-error.service';

describe('ApiErrorService', () => {
  const service = new ApiErrorService();

  it.each([
    ['validation', 'validation'], ['duplicate', 'duplicate'], ['concurrency', 'concurrency'],
    ['not_found', 'notFound'], ['unauthorized', 'unauthorized'], ['forbidden', 'forbidden'], ['transient', 'transient'],
  ] as const)('classifies the stable %s problem code', (code, expected) => {
    const result = service.normalize(new HttpErrorResponse({ status: 400, error: { code } }));
    expect(result.kind).toBe(expected);
  });

  it('allows retry only for transient failures', () => {
    const transient = service.normalize(new HttpErrorResponse({ status: 503, error: { title: 'Unavailable' } }));
    const forbidden = service.normalize(new HttpErrorResponse({ status: 403, error: { code: 'forbidden' } }));
    expect(transient.canRetry).toBe(true);
    expect(forbidden.canRetry).toBe(false);
  });

  it('treats transport failures as transient without exposing an exception', () => {
    expect(service.normalize(new Error('network'))).toEqual({ status: 0, kind: 'transient', canRetry: true });
  });
});
