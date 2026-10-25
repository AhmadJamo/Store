# MOD-018 advanced Angular acceptance gate

> Status: IMPLEMENTED 2026-10-25

`scripts/verify-angular-modernization.ps1` is the reusable acceptance command for every Angular migration wave. It now
builds the Release solution, runs the backend regression executable, enforces external SCSS, runs Angular tests and a
production statistics build, checks explicit initial/lazy bundle limits, applies static accessibility contracts and
runs a real Edge/Playwright pass for RTL/LTR, keyboard skip navigation, loading/empty/error states and a 390px viewport.

The command always writes `artifacts/verification/angular-acceptance/report.json` with separate passed, failed and
skipped results. A missing API host or authenticated storage-state file is reported as skipped; it is never presented as
successful E2E evidence. When `-ApiUrl` is supplied, the browser runner verifies the anonymous session 401 contract and
the performance runner uses the existing bounded HTTP workload. Supplying `-AuthenticatedStorageState` additionally
checks the real session plus only allowlisted read endpoints for permissions present in that external session. No
credential, cookie or token belongs in source control.

The final MOD-018B local run passed all eleven outer results with zero failures and zero skips. The browser passed four
UI checks, the real anonymous 401 contract, the authenticated session and three permission-allowed read endpoints. Initial production
bytes were 290,987 against a 358,400-byte limit; the largest lazy JavaScript chunk was 52,866 against an 81,920-byte
limit. The 100-request, concurrency-10 availability smoke completed with zero failures, p95 40.37 ms and p99 61.85 ms.
The browser run also found and fixed the shell skip link so it transfers focus to `main-content` in Edge.

`scripts/Invoke-AngularAuthenticatedAcceptance.ps1` opens an interactive Edge login, captures storage state only under
the Windows temporary directory, runs the same gate and deletes the temporary directory in `finally`. Credentials are
never read or logged. A direct gate run must still record a skip when no external state is supplied. MVC remains the rollback path.
