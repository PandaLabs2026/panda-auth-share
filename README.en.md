# panda-auth-share

**PandaAuth by PandaLabs** · [简体中文](README.md)

> In development; no formally supported release yet. Access is by invitation or request. Implementation does not imply a verified release.

## Responsibility and boundaries

Cross-process contracts for PandaAuth. [PandaAuth.Shared](src/PandaAuth.Shared) contains user-state enums, endpoint constants and claim constants.

This repository contains no database entities, persistence implementation or SDK. The SDK lives in [panda-auth-sdk](https://github.com/PandaLabs2026/panda-auth-sdk). Server, SDK and Me reference Share by relative path; changes require consumer compatibility review.

## Current implementation and limitations

Shared constants and model definitions exist; they are not a complete client integration library. Cross-repository builds and compatibility combinations were not verified in this change; consult consumers and the capability matrix.

## Build

Use the .NET SDK selected by [global.json](global.json) (currently 10.0.112 with latestFeature roll-forward). Clone repositories as siblings using the [workspace layout](https://github.com/PandaLabs2026/panda-auth/blob/main/WORKSPACE.md); cross-repository links require access. Commands below run from this repository root. They were statically checked, not executed, in this documentation change.

```bash
dotnet build PandaAuth.Shared.slnx
```

This is a library repository with no standalone service port or run command. Version public contract changes according to compatibility impact and update consumer documentation.

## Roadmap and governance

Implementation targets are tracked in the [capability matrix](https://github.com/PandaLabs2026/panda-auth/blob/main/docs/open-source/capabilities.md) and [release gates](https://github.com/PandaLabs2026/panda-auth/blob/main/docs/open-source/release-readiness.md). Real product needs drive the roadmap; community requests are evaluated without delivery commitments. [Community/commercial boundaries](https://github.com/PandaLabs2026/panda-auth/blob/main/docs/open-source/strategy.md) describe scope, not delivered commercial products.

- [Security](SECURITY.md): selected private reporting channel, enablement unverified; no public vulnerability details.
- [Contributing](CONTRIBUTING.md): repository-specific checks and the shared contribution policy.
- [MIT License](LICENSE) for project-owned code/documentation, subject to [license scope](LICENSING.md); third-party terms remain applicable and brand images are excluded.
