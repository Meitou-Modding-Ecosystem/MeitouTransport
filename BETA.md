# Beta development

Develop on a working branch and open a PR targeting `beta`. All validation jobs must pass before the required `Development readiness` check succeeds. The current beta rules require a PR and successful CI, with zero approvals and no bypass needed. This setup does not change protections or bypass required CI. Main and release remain separate review and release gates.

## Automated validation

Builds the framework, tests, live verifier and start generator against Meitou 0.20.1 and 0.22.1 in separate jobs. Runs existing pack, riding, save and one/four-worker determinism checks, then creates a local handoff and verifies its checksums, source commit and byte-for-byte rebuild. Run from a clean committed checkout with dotnet on PATH.

The workflow runs on pushes and pull requests with a read-only token and pinned Actions. `GetCIEngine.ps1` downloads SHA-256-verified official managed build fixtures; no Kenshi installation, credentials or personal game data are required. To reproduce locally, install .NET 11, Python 3 and PowerShell 7, run that script with an external fixture directory, then run `CI.ps1` as shown in `.github/workflows/ci.yml`.

These are development checks for the documented baseline, not a production release certification or a test of unreleased sandbox engine hooks. No deployment, release publishing, Steam synchronization, GUI playtest or native game session runs in this workflow. Changes needing a newer engine must update the fixture/dependency pin and corresponding tests explicitly. Live rendering and real game save/load scenarios still require the documented manual trials. The manifest permits 0.20.1 through 0.22.x; the matrix covers the two named releases, not every nightly. Use the live verifier’s `-AnyBuild` option for an explicit newer-build trial.
