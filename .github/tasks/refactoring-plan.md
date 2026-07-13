# Silkara Clean Architecture Refactoring Plan

## Overview
This document outlines the step-by-step plan to transition the Silkara solution into a Clean Architecture (DDD-based) layout.

## Phase 1: Physical Reorganization
1. Create `src/`, `tests/`, and `benchmarks/` at the root.
2. Move existing projects to `src/`:
   - `SilkaraServer`
   - `SilkaraClient`
   - `JsonManagerLib`
   - `UTP`
   - `UtpTypes`
3. Move `Benchmarking` project to `benchmarks/`.
4. Remove all temporary `bin/`, `obj/`, and `BenchmarkDotNet.Artifacts/` folders.
5. Update `Silkara.sln` to reference new project locations, ensuring paths to `certs/` and `secrets/` remain valid.

## Phase 2: SilkaraServer Decomposition (Clean Architecture)
1. Within the current `src/SilkaraServer/` project, establish these top-level folders:
   - **Domain:** Entities, Value Objects, Domain Exceptions, Repository Interfaces.
   - **Application:** Handlers, Managers, DTOs, Service Interfaces.
   - **Infrastructure:** DB implementation, Network/Middleware, Settings.
   - **Presentation:** `Program.cs`, DI Container setup, Startup logic.
2. As files are moved, immediately update their `namespaces` and `using` statements to reflect the new structure.
3. Run `dotnet build` after moving each logical block to ensure continued compilability.

## Phase 3: Test Restructuring
1. Create `tests/` directory with `[ProjectName].UnitTests` projects.
2. Ensure folder structure in `tests/` strictly mirrors `src/`.
3. Implement required testing frameworks (xUnit, FluentAssertions, NSubstitute).
4. As files are created/moved, immediately update namespaces.
5. Run `dotnet build` after each test class setup.

## Phase 4: Final Cleanup & Validation
1. Perform a final review of all namespaces, project references, and configuration paths.
2. Validate final compilation.
3. Run all unit and integration tests.
