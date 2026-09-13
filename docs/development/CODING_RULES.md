# Coding rules observed in the repository
> Last reviewed: 2026-09-13

- Nullable references and implicit usings are enabled; asynchronous methods conventionally end in `Async`.
- Domain entities use private setters and constructor/behaviour validation.
- DTOs are manually mapped in services; entities are not intentionally returned to Razor views.
- Repository interfaces are Domain; EF implementations Infrastructure; services are registered manually in Web `Program.cs`.
- Controllers commonly add errors/TempData and redirect after writes. This is observed practice, not proof it is ideal.
- EF transaction use is expected for multi-record business documents via `IUnitOfWork`.
- Permission names use `Module.Action`; use `PermissionAuthorize` on controller actions.
- Razor views use MVC tag helpers and embedded JavaScript; user-derived JS values need safe rendering review.
- Existing formatting uses explicit line breaks and descriptive business exceptions. No formatter/editorconfig rule was found.
