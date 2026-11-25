import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { LocalizationService } from '../../core/localization/localization.service';
import { NavigationGroupStateService } from '../../workspace/navigation/navigation-group-state.service';
import { NavigationLocalizationService } from '../../workspace/navigation/navigation-localization.service';
import { NavigationRegistryService } from '../../workspace/navigation/navigation-registry.service';
import { ShellNavigationLocalizationService } from './shell-navigation-localization.service';
import { CurrentAppNavigationComponent } from './current-app-navigation';

describe('CurrentAppNavigationComponent', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CurrentAppNavigationComponent],
      providers: [
        provideRouter([]),
        {
          provide: NavigationGroupStateService,
          useValue: { isOpen: () => true, setOpen: vi.fn() },
        },
        { provide: NavigationLocalizationService, useValue: { text: (key: string) => key } },
        { provide: ShellNavigationLocalizationService, useValue: { group: (key: string) => key } },
        { provide: LocalizationService, useValue: { text: (key: string) => key } },
        {
          provide: NavigationRegistryService,
          useValue: {
            metadataForItem: (item: { id: string }) => ({
              appId: 'products',
              sectionId: 'catalog',
              archetype: 'list',
              mobilePriority: item.id === 'hidden' ? 'hidden' : 'primary',
            }),
          },
        },
      ],
    }).compileComponents();
  });

  it('emits the registered destination and closes the mobile drawer', () => {
    const fixture = TestBed.createComponent(CurrentAppNavigationComponent);
    const item = { id: 'products', route: '/products', labelKey: 'products', icon: 'P' } as const;
    fixture.componentRef.setInput('mode', 'mobile');
    fixture.componentRef.setInput('groups', [{ id: 'catalog', icon: 'C', items: [item] }]);
    fixture.detectChanges();
    const drawer = fixture.nativeElement.querySelector('.mobile-navigation') as HTMLDetailsElement;
    drawer.open = true;
    let selected: unknown;
    fixture.componentInstance.navigationSelected.subscribe((value) => (selected = value));

    const link = fixture.nativeElement.querySelector('.nav-item') as HTMLAnchorElement;
    (fixture.componentInstance as any).select(
      { preventDefault: vi.fn(), currentTarget: link },
      item,
    );

    expect(selected).toBe(item);
    expect(drawer.open).toBe(false);
    expect(link.dataset['archetype']).toBe('list');
    expect(link.dataset['mobilePriority']).toBe('primary');
  });

  it('omits destinations classified as hidden from the mobile drawer', () => {
    const fixture = TestBed.createComponent(CurrentAppNavigationComponent);
    fixture.componentRef.setInput('mode', 'mobile');
    fixture.componentRef.setInput('groups', [
      {
        id: 'catalog',
        icon: 'C',
        items: [
          { id: 'products', route: '/products', labelKey: 'products', icon: 'P' },
          { id: 'hidden', route: '/products/hidden', labelKey: 'products', icon: 'H' },
        ],
      },
    ]);
    fixture.detectChanges();

    expect(fixture.nativeElement.querySelectorAll('.nav-item')).toHaveLength(1);
  });
});
