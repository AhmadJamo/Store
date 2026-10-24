# MiniStore Front-End

Angular workspace for the incremental MiniStore ERP presentation modernization.

## Foundation

- Angular 22 standalone components
- strict TypeScript
- lazy feature routing
- Angular Signals for synchronous workspace state
- RxJS-ready HTTP boundary
- semantic SCSS tokens with system/light/dark themes and comfortable/compact density
- persisted Arabic RTL and English LTR display preferences
- accessible focus, reduced-motion, forced-colors and shared component-state patterns
- lazy `/design-system` visual reference
- same-origin cookie/API strategy through the development proxy

The existing ASP.NET Core MVC application remains operational and authoritative. This project must not reproduce accounting, inventory, pricing, authorization, numbering, concurrency or tenant-isolation rules in the browser.

## Local commands

```bash
npm ci
npm start
npm test -- --watch=false
npm run build
```

`npm start` proxies `/api` and `/Account` to the local ASP.NET Core host configured in `proxy.conf.json`.

## Planned structure

- `core/`: authentication, typed API clients, errors, localization and permissions.
- `shell/`: application chrome and navigation.
- `workspace/`: tabs, history, restoration and dirty-state ownership.
- `shared/`: proven reusable UI, forms, tables and validation.
- `features/`: real ERP vertical slices only when approved.

## Protection rules

- Never expose EF entities directly.
- Never trust client-side permissions or validation as a security boundary.
- Never calculate or post financial/inventory effects in Angular.
- Never store long-lived credentials or sensitive financial payloads in browser storage.
- Add abstractions only after a real use case demonstrates reuse.
