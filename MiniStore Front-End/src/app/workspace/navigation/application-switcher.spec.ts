import { ComponentFixture, TestBed } from '@angular/core/testing';
import { signal } from '@angular/core';
import { ApplicationSwitcherComponent } from './application-switcher';
import { NavigationLocalizationService } from './navigation-localization.service';
import { NavigationRegistryService, VisibleNavigationApplication } from './navigation-registry.service';

describe('ApplicationSwitcherComponent', () => {
  let fixture: ComponentFixture<ApplicationSwitcherComponent>;
  const entry = { id: 'products', route: '/products', labelKey: 'products', icon: 'P' } as const;
  const applications = signal<readonly VisibleNavigationApplication[]>([
    { id: 'products', labelKey: 'appProducts', icon: 'P', entryNavigationIds: ['products'], navigationIds: ['products'], entry },
  ]);

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ApplicationSwitcherComponent],
      providers: [
        { provide: NavigationRegistryService, useValue: { visibleApplications: applications } },
        { provide: NavigationLocalizationService, useValue: { text: (key: string) => key } },
      ],
    }).compileComponents();
    fixture = TestBed.createComponent(ApplicationSwitcherComponent);
    fixture.componentRef.setInput('currentApplicationId', 'products');
    fixture.detectChanges();
  });

  it('renders only registry-approved applications and emits the safe entry', () => {
    let selected: unknown;
    fixture.componentInstance.applicationSelected.subscribe((item) => selected = item);
    const link = fixture.nativeElement.querySelector('.application-menu a') as HTMLAnchorElement;

    expect(link.textContent).toContain('appProducts');
    expect(link.getAttribute('aria-current')).toBe('page');
    link.click();
    expect(selected).toBe(entry);
  });
});
