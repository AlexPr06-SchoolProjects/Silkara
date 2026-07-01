# Silkara: Custom UTP Protocol Implementation

Silkara is a custom, high-performance transport protocol implementation built on top of TCP, utilizing .NET's `System.IO.Pipelines` for efficient data handling.

## Project Architecture

The project is structured into several key components:

- **`UTP/`**: The core protocol engine. It handles low-level socket communication, packet serialization/deserialization, and efficient data streaming using `System.IO.Pipelines`.
- **`UtpTypes/`**: Contains protocol-specific types, message structures, dispatchers, and middleware interfaces.
- **`SikaraServer/`**: The server application implemented as a `BackgroundService`. It listens for incoming TCP connections and manages client states.
- **`SikaraClient/`**: The client application implementation, also as a `BackgroundService`, which connects to the server and handles protocol-level message exchange.
- **`JsonManagerLib/`**: A library dedicated to JSON serialization and management of packet payloads.

## Building and Running

The project is a standard .NET solution.

### Prerequisites
- .NET SDK (latest stable version recommended).
- Manual setup of `certs/` and `secrets/` folders (see `README_CERTS.md`).

### Commands

- **Build:**
  ```bash
  dotnet build Silkara.sln
  ```
- **Run Server:**
  ```bash
  dotnet run --project SikaraServer/SilkaraServer.csproj
  ```
- **Run Client:**
  ```bash
  dotnet run --project SikaraClient/SilkaraClient.csproj
  ```

## Security and Configuration

The project uses encrypted configuration files and requires SSL for database connections.

- **Secrets (`/secrets`)**: Place `db_user.txt` and `db_password.txt` (raw strings) in this directory.
- **Certs (`/certs`)**: Contains `server.crt` and `server.key` for SSL.
- **See `README_CERTS.md`** for detailed instructions on generating these files and setting up the required directories, as they are ignored by git.

## Development Conventions

- **Networking**: Use `System.IO.Pipelines` (via `UtpEngine`) for all new networking components to maintain performance consistency.
- **Architecture**: Long-running services should be implemented as `BackgroundService`.
- **Protocol**: All new protocol additions must define their structures in `UtpTypes` and follow the `IUtpMessage` patterns.
