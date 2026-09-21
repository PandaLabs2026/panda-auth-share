# panda-auth-share

**PandaAuth by PandaLabs** · [简体中文](README.md)

> In development; no formally supported release yet. Access is by invitation or request. Implementation does not imply a verified release.

## Responsibility and boundaries

Cross-process contracts for PandaAuth. [PandaAuth.Shared](src/PandaAuth.Shared) contains user-state enums, endpoint constants and claim constants.

This repository contains no database entities, persistence implementation or SDK. The SDK lives in [panda-auth-sdk](https://github.com/PandaLabs2026/panda-auth-sdk). Server, SDK and Me reference Share by relative path; changes require consumer compatibility review.

## Protocol endpoints

The public endpoint contract is the set of constants in `PandaAuthEndpoints` (paths relative to the Issuer); consumers must not copy the literals:

| Constant | Path | Purpose |
| --- | --- | --- |
| `Authorization` | `/connect/authorize` | Authorization endpoint |
| `Token` | `/connect/token` | Token endpoint |
| `Userinfo` | `/connect/userinfo` | UserInfo endpoint |
| `Logout` | `/connect/logout` | End-session endpoint |
| `Introspection` | `/connect/introspect` | Token introspection endpoint |
| `Revocation` | `/connect/revoke` | Token revocation endpoint |
| `JsonWebKeySet` | `/.well-known/jwks` | Signing key set, the `jwks_uri` of the discovery document |
| `OpenIdConfiguration` | `/.well-known/openid-configuration` | OIDC discovery document |

These values were checked one by one against production discovery on 2026-09-16 (8/8). Constants matching paths is not evidence that the endpoints passed protocol tests.

## Current implementation and limitations

Shared constants and model definitions exist; they are not a complete client integration library. Cross-repository builds and compatibility combinations were not verified in this change; consult consumers and the capability matrix.

## Build

Use the .NET SDK selected by [global.json](global.json) (currently 10.0.112 with latestFeature roll-forward). This repository can be built without the private coordination repository. Commands below run from this repository root. They were statically checked, not executed, in this documentation change.

```bash
dotnet build PandaAuth.Shared.slnx
```

This is a library repository with no standalone service port or run command. Version public contract changes according to compatibility impact and update consumer documentation.

## Roadmap and governance

Product roadmap, release gates and community/commercial boundaries remain maintainer-governed until a formal public release. This README documents only the independently reproducible Share build boundary.

- [Security](SECURITY.md): selected private reporting channel, enablement unverified; no public vulnerability details.
- [Contributing](CONTRIBUTING.md): repository-specific checks and the shared contribution policy.
- [MIT License](LICENSE) for project-owned code/documentation, subject to [license scope](LICENSING.md); third-party terms remain applicable and brand images are excluded.
