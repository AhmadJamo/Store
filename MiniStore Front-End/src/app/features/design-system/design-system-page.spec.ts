import { TestBed } from '@angular/core/testing';
import { DesignSystemPage } from './design-system-page';

describe('DesignSystemPage', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [DesignSystemPage] }).compileComponents();
  });

  it('renders shared states and ERP list and record patterns', () => {
    const fixture = TestBed.createComponent(DesignSystemPage);
    fixture.detectChanges();
    const element: HTMLElement = fixture.nativeElement;
    expect(element.querySelector('.list-preview table')).toBeTruthy();
    expect(element.querySelector('.record-preview [aria-current="step"]')).toBeTruthy();
    expect(element.querySelector('[aria-busy="true"]')).toBeTruthy();
    expect(element.querySelector('[role="alert"]')).toBeTruthy();
  });
});
