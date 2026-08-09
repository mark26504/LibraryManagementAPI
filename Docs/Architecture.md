# Architecture

The project uses the Onion Layer Architecture taught in the course.

## Projects

- LibraryManagement.Domain
- LibraryManagement.Services.Abstraction
- LibraryManagement.Services
- LibraryManagement.Persistence
- LibraryManagement.Presentation
- LibraryManagement.Shared
- LibraryManagement.API

## Direct Project References

LibraryManagement.Domain
- No direct project references.

LibraryManagement.Shared
- No direct project references.

LibraryManagement.Services.Abstraction
- LibraryManagement.Shared

LibraryManagement.Services
- LibraryManagement.Domain
- LibraryManagement.Services.Abstraction

LibraryManagement.Persistence
- LibraryManagement.Services

LibraryManagement.Presentation
- LibraryManagement.Services.Abstraction

LibraryManagement.API
- LibraryManagement.Persistence
- LibraryManagement.Presentation

## Important Rule

The project references above are direct references.

Visual Studio may show transitive dependencies nested under a direct dependency.

For example:

API → Persistence → Services → Domain

does not mean that API directly references Services or Domain.

This exact dependency graph is the source of truth for this project and must not be replaced with another Onion Architecture variant.