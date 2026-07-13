# ROLE AND OBJECTIVE
You are a Senior .NET/C# Solutions Architect and a specialist in deep refactoring. Your goal is to redesign the existing Silkara solution repository structure (.NET 8/9), bringing it into line with industry standards for Clean Architecture (based on Milan Jovanović's methodology) and Domain-Driven Design (DDD), as well as properly organize the physical structure of folders and projects.

---

# CURRENT CONTEXT
The project currently has a flat structure at the root of the repository and mixed responsibilities on the server side.
Existing projects:
- `SilkaraServer` (the server side of the application, which currently mixes logic, database, handlers, and sockets).
- `SilkaraClient` (the client application).
- `JsonManagerLib` (a helper library for working with JSON).
- `UTP` and `UtpTypes` (transport layer and common data types of the custom network protocol).
- `Benchmarking` (projects with BenchmarkDotNet performance measurements).
- Benchmarks are missing or in a chaotic state. The directory tree also contains the temporary build folders `bin`, `obj`, and `BenchmarkDotNet.Artifacts`.

--

# STEP-BY-STEP REFACTORING PLAN (YOUR TASKS)

## Step 1. Global reorganization of the file structure (Physical Layout)
1. Create three main directories in the repository root: `src/`, `tests/`, `benchmarks/`.
2. Move existing projects:
- To `src/`: `SilkaraServer`, `SilkaraClient`, `JsonManagerLib`, `UTP`, `UtpTypes`.
- To `benchmarks/`: `Benchmarking`.
3. Remove from tracking and physically delete all temporary `bin`, `obj`, and `BenchmarkDotNet.Artifacts` folders.
4. Update the `Silkara.sln` solution file to correctly reference the new `.csproj` file paths within the new directories.

## Step 2. Implementing a Clean Architecture for SilkaraServer
Split the monolithic SilkaraServer project into four logical layers (these can be either separate .csproj projects within the src/SilkaraServer/ folder, or strictly isolated folders with clear dependency rules. If splitting into projects is currently excessive, choose the optimal approach and justify it):

1. **Domain (Core):**
- Move all business entities, value objects, domain exceptions, and repository interfaces (e.g., database contracts and state abstractions from Identities and States) here.
- *Dependency Rule:* This layer MUST NOT have references to any other layers, databases, or network libraries.

2. **Application (Use Cases):**
- Move the application's business logic here: `Handlers`, `Managers`, state management services, DTOs, and external service interfaces.
- *Dependency Rule:* Depends ONLY on the Domain layer.

3. **Infrastructure:**
- Move the database implementation (`Database`, DbContext, migrations), network infrastructure (`Middleware`, sockets, cryptography), and settings (`Settings`) here.
- *Dependency Rule:* Depends on the Application and Domain. EF Core, database providers, and specific I/O implementations are included here.

4. **Presentation / API (Entry Point):**
- Server launch point (`Server/`, DI container configuration, UTP socket listener startup, `Program.cs`).
- *Dependency Rule:* Depends on Application and Infrastructure (only for registering dependencies in DI).

## Step 3. Reorganizing and Mirroring Tests
1. Within the `tests/` folder, create a structure for unit and integration tests named `[ProjectName].UnitTests` or `[ProjectName].IntegrationTests` (e.g. `tests/UTP.UnitTests/`, `tests/JsonManagerLib.UnitTests/`).
2. For each created test project, set up references to the corresponding projects from `src/`. 3. The internal folder structure in each test project must STRICTLY MIRROR the folder structure of the project being tested (e.g., the class `src/UTP/Connection/UtpClient.cs` should be tested in `tests/UTP.UnitTests/Connection/UtpClientTests.cs`).
4. Use the xUnit framework, FluentAssertions, and NSubstitute (or Moq) for mocking.

## Step 4. Updating Namespaces and References
1. After moving the files, update all `namespaces` in the C# files to match the new folder structure (like `Silkara.Domain.Identities` or `UTP.Connection`).
2. Ensure that all `using` statements in the code are updated and the project compiles successfully without errors.

---

# 🛑 STRICT RESTRICTIONS (GUARDRAILS)
1. **DO NOT remove business logic:** Your task is architectural refactoring and code relocation, not changing the existing behavior of the application's business logic or the UTP network protocol.
2. **NO EXTRA PACKAGES:** Do not add third-party NuGet packages (e.g., MediatR or AutoMapper) unless they are used in the current code or unless I explicitly request it. Maintain a clean architecture based on native .NET DI.
3. **Compilability:** After each logical step (moving projects -> updating the plugin -> updating namespaces), the code must remain in a working, compilable state.
4. **Explanation of actions:** Before making bulk changes to files