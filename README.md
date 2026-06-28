# SuperScrabble
A real-time multiplayer Scrabble clone playable in the browser. Players join rooms, draw tiles, place words on a 15x15 bonus board, and compete for the highest score. Supports English and Bulgarian dictionaries.

---

## Architecture

```
Angular 13 (SPA)
    │  REST (JWT)       WebSocket (SignalR)
    ▼                          ▼
ASP.NET Core 5 Web API ◄──► GameHub
    │
    ├── Services.Game        ← turn logic, scoring, validation, in-memory state
    ├── Services.Data        ← EF Core repositories (users, games, words)
    └── Data (SQL Server)    ← AppUser, Game, UserGame, Word
```

Game state lives in-memory (`StaticGameStateManager`) and is pushed to clients over SignalR. SQL Server stores only persisted records (users, completed games, dictionary).

---

## Stack

| Layer | Tech |
|---|---|
| Frontend | Angular 13, Angular Material, Bootstrap 5, SignalR JS client |
| Backend | ASP.NET Core 5, SignalR hub, JWT auth (Identity) |
| ORM | Entity Framework Core 5 (lazy-loading, code-first migrations) |
| Database | SQL Server (SQLEXPRESS, local dev) |
| Testing | NUnit 3 + Moq (backend), Karma + Jasmine (frontend) |

---

## Running locally

**Prerequisites:** .NET 5 SDK, Node 16+, SQL Server Express

```bash
# 1. Restore backend dependencies and run migrations
dotnet restore
dotnet ef database update --project src/Data/SuperScrabble.Data

# 2. Start the API (serves Angular via SPA middleware on :5001)
dotnet run --project src/WebApi/SuperScrabble.WebApi

# 3. Or run the Angular dev server separately
cd src/ClientApp/super-scrabble-app
npm install
ng serve          # http://localhost:4200
```

The backend connection string targets `Server=.\SQLEXPRESS;Database=SuperScrabbleDB;Integrated Security=True`. Update `appsettings.json` if your SQL Server instance differs.

---

## Project structure

```
src/
  ClientApp/               Angular SPA
    super-scrabble-app/
      app/pages/           game, game-summary, home, auth forms
      app/services/        SignalrService, WebRequestsService
  Common/                  Shared constants, language resources, input/view models
  Data/                    EF Core DbContext, entity models, migrations
  Services/
    Game/                  Core gameplay: GameService, ScoringService, GameValidator,
                           StaticGameStateManager, Board, TilesBag, Player
    Data/                  UsersService, GamesService, WordsService
  WebApi/
    Controllers/           GamesController, UsersController
    Hubs/                  GameHub (SignalR)
  Tests/
    Services/              NUnit tests for game logic
tools/
  WordScrapers/            One-off scrapers used to build the word dictionaries
resources/
  final-list/              Seeded word lists (EN + BG)
```

---

## Game flow

1. User registers/logs in → receives JWT
2. User creates or joins a game room (REST)
3. Server sends `StartGame` over SignalR with initial state
4. Players take turns: place word → `WriteWord` → server validates, scores, broadcasts `UpdateGameState`
5. Game ends when tile bag empties or all players skip consecutively → `GameSummary` sent

Word validity is checked against the `Word` table seeded from `resources/final-list/`.


