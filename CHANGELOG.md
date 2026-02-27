# Changelog

All notable changes to this project are documented here.
Format follows [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).
Versioning follows [Semantic Versioning](https://semver.org/).

---

## How to use this file

**When to add an entry:** every time you make a meaningful change —
new endpoint, new ADR, new API project added, dependency updated,
breaking change in a contract.

**Never edit past entries** — only add new ones at the top.
The history is the value.

**Version format:** MAJOR.MINOR.PATCH
- MAJOR → breaking change in an API contract
- MINOR → new feature, new endpoint, new API project added
- PATCH → bug fix, refactor, documentation update

---

## [Unreleased]

### Added
- Initial monorepo structure with shared Domain and Infrastructure
- ADR-0001 through ADR-0008 covering core architectural decisions
- Docker Compose for local development (Postgres, Redis, MinIO)
- Kubernetes base manifests with Kustomize overlays
- Copilot instructions configured for .NET study workflow

---

## [0.1.0] — 2026-02-26

### Added
- Project inception
- MVP specification document (docs/architecture/MVP.md)
- Initial DDD Strategic Design documentation
- Bounded contexts defined: Catalogue, Negotiation, 
  Identity & Profiling, Media Protection, 
  Security & Trust, Backoffice