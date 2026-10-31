import { TestBed } from '@angular/core/testing';
import { App } from './app';

describe('App', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [App] }).compileComponents();
  });

  it('creates the application root', () => {
    expect(TestBed.createComponent(App).componentInstance).toBeTruthy();
  });
});
