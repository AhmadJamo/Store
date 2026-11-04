import { Injectable, inject } from '@angular/core';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';
import { distinctUntilChanged, map, Observable } from 'rxjs';
import { ApiSessionService } from '../../core/api/api-session.service';

export interface PageContextField<T> {
  readonly parse: (raw: string | null) => T | undefined;
  readonly format: (value: T) => string | null;
}

export interface PageContextSchema<T extends object> {
  readonly defaults: T;
  readonly fields: { readonly [K in keyof T]?: PageContextField<T[K]> };
}

export const pageContextFields = {
  text(maxLength = 120): PageContextField<string> {
    return {
      parse: raw => raw === null ? undefined : raw.trim().slice(0, maxLength),
      format: value => value.trim() ? value.trim().slice(0, maxLength) : null,
    };
  },
  integer(minimum = 1, maximum = 10_000): PageContextField<number> {
    return {
      parse: raw => {
        const value = Number(raw);
        return Number.isInteger(value) && value >= minimum && value <= maximum ? value : undefined;
      },
      format: value => Number.isInteger(value) && value >= minimum && value <= maximum ? String(value) : null,
    };
  },
  boolean(): PageContextField<boolean> {
    return {
      parse: raw => raw === 'true' ? true : raw === 'false' ? false : undefined,
      format: value => String(value),
    };
  },
  optionalBoolean(): PageContextField<boolean | undefined> {
    return {
      parse: raw => raw === 'true' ? true : raw === 'false' ? false : undefined,
      format: value => value === undefined ? null : String(value),
    };
  },
  choice<const T extends string>(values: readonly T[]): PageContextField<T> {
    return {
      parse: raw => values.includes(raw as T) ? raw as T : undefined,
      format: value => values.includes(value) ? value : null,
    };
  },
  numberChoice<const T extends number>(values: readonly T[]): PageContextField<T> {
    return {
      parse: raw => {
        const value = Number(raw) as T;
        return values.includes(value) ? value : undefined;
      },
      format: value => values.includes(value) ? String(value) : null,
    };
  },
};

@Injectable({ providedIn: 'root' })
export class PageContextService {
  private readonly router = inject(Router);
  private readonly session = inject(ApiSessionService);
  private readonly contexts = new Map<string, object>();
  private readonly drafts = new Map<string, unknown>();
  private activeBoundary: string | null = null;

  read<T extends object>(scope: string, params: ParamMap, schema: PageContextSchema<T>): T {
    const key = this.key(scope);
    const remembered = this.contexts.get(key) as T | undefined;
    const result = { ...schema.defaults, ...remembered } as T;
    for (const fieldName of Object.keys(schema.fields) as (keyof T)[]) {
      if (!params.has(String(fieldName))) continue;
      const value = schema.fields[fieldName]?.parse(params.get(String(fieldName)));
      if (value !== undefined) result[fieldName] = value;
    }
    this.contexts.set(key, { ...result });
    return result;
  }

  watch<T extends object>(scope: string, route: ActivatedRoute, schema: PageContextSchema<T>): Observable<T> {
    return route.queryParamMap.pipe(
      map(params => this.read(scope, params, schema)),
      distinctUntilChanged((left, right) => JSON.stringify(left) === JSON.stringify(right)),
    );
  }

  replace<T extends object>(scope: string, route: ActivatedRoute, schema: PageContextSchema<T>, value: T): Promise<boolean> {
    const clean = this.normalize(value, schema);
    this.contexts.set(this.key(scope), clean);
    const queryParams: Record<string, string | null> = {};
    for (const fieldName of Object.keys(schema.fields) as (keyof T)[]) {
      queryParams[String(fieldName)] = schema.fields[fieldName]?.format(clean[fieldName]) ?? null;
    }
    return this.router.navigate([], { relativeTo: route, queryParams, queryParamsHandling: 'merge', replaceUrl: true });
  }

  saveDraft<T>(scope: string, value: T): void { this.drafts.set(this.key(scope), structuredClone(value)); }
  loadDraft<T>(scope: string): T | null {
    const value = this.drafts.get(this.key(scope));
    return value === undefined ? null : structuredClone(value) as T;
  }
  clearDraft(scope: string): void { this.drafts.delete(this.key(scope)); }

  private normalize<T extends object>(value: T, schema: PageContextSchema<T>): T {
    const result = { ...schema.defaults } as T;
    for (const fieldName of Object.keys(schema.fields) as (keyof T)[]) {
      const field = schema.fields[fieldName];
      const formatted = field?.format(value[fieldName]) ?? null;
      result[fieldName] = field?.parse(formatted) ?? schema.defaults[fieldName];
    }
    return result;
  }

  private key(scope: string): string {
    const safeScope = scope.replace(/[^a-z0-9:-]/gi, '').slice(0, 80);
    const session = this.session.session();
    const boundary = `${session?.tenant.id ?? 'anonymous'}:${session?.user.id ?? 'anonymous'}`;
    if (this.activeBoundary !== null && this.activeBoundary !== boundary) {
      this.contexts.clear();
      this.drafts.clear();
    }
    this.activeBoundary = boundary;
    return `${boundary}:${safeScope}`;
  }
}
