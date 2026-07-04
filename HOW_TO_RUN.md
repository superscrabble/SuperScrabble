# How to run SuperScrabble on your machine

Two processes run side by side in development:

| Part | Tech | URL |
|---|---|---|
| Backend (Web API + SignalR) | ASP.NET Core (.NET 8) | `https://localhost:7168` (Swagger at `/swagger`) |
| Frontend (SPA) | Angular 13 | `http://localhost:4200` |

The Angular dev server at port **4200** talks to the API at port **7168** — both values are already wired up (`environment.ts` → `serverUrl`, CORS in `Program.cs`).

## 1. Prerequisites

- **.NET SDK 8.0** — check with `dotnet --list-sdks`
- **Node.js** (v16+; verified working on v24) — check with `node -v`
- **SQL Server LocalDB** — ships with Visual Studio; check with `sqllocaldb info MSSQLLocalDB`.
  Any full SQL Server instance also works, see [Configuration](#4-configuration-reference).
- Visual Studio 2022 is optional — everything below works from a plain terminal.

## 2. First-time setup

### Backend

Nothing to install manually — NuGet restore happens on first build. The database **creates itself on first run**: migrations are applied automatically, and the word dictionary (~866,000 words from `src/Server/WebApi/SuperScrabble.WebApi/all/*.txt`) is seeded.

> **First boot takes a minute or two** because of word seeding. Progress is logged to the console ("Seeding 865809 unique words…"). Every later boot detects the words are already there and skips seeding instantly.

### Frontend

```powershell
cd src\ClientApp\super-scrabble-app
npm install
```

(The repo contains an `.npmrc` with `legacy-peer-deps=true`, so no extra flags are needed.)

## 3. Running the app

### Start the backend

**From the terminal:**

```powershell
cd src\Server\WebApi\SuperScrabble.WebApi
dotnet run
```

**Or from Visual Studio:** open the server solution and start the **SuperScrabble.WebApi** profile (not IIS Express, so the URLs match). WebApi is the only startup project you need.

> **Smart App Control warning (this machine).** Windows Smart App Control is enabled and blocks freshly compiled DLLs with `FileLoadException … An Application Control policy has blocked this file (0x800711C7)` — in both Debug **and** Release output folders, unpredictably. It has no exclusion list. If startup fails with this error, the reliable fix is turning Smart App Control off: Windows Security → App & browser control → Smart App Control settings → Off (note: it cannot be re-enabled without resetting Windows). Retrying or rebuilding occasionally gets past it, but not dependably.

Verify it's up: open https://localhost:7168/swagger

### Start the frontend

In a second terminal:

```powershell
cd src\ClientApp\super-scrabble-app
npm start
```

Then open **http://localhost:4200**, register a user, and play.

> Two browser windows (one of them incognito, so it has its own login) are enough to test a Duel game against yourself.

## 4. Configuration reference

All backend settings live in `src\Server\WebApi\SuperScrabble.WebApi\appsettings.json` (with development overrides in `appsettings.Development.json`):

| Setting | Default | Notes |
|---|---|---|
| `ConnectionStrings:DefaultConnection` | `(localdb)\MSSQLLocalDB`, database `SuperScrabble` | Point this at any SQL Server. Keep `TrustServerCertificate=True` for local instances. |
| `Jwt:SigningKey` | dev key in `appsettings.Development.json` | Must be ≥ 32 characters; the app refuses to start without one. For production supply it via the `JWT__SIGNINGKEY` environment variable — never commit a real key. |
| `Seeding:WordsDirectory` | *(empty)* → `<WebApi>\all` | Set an absolute path to use another dictionary folder (e.g. the sibling `superscrabble-dictionary` repo). All `*.txt` files in the folder are read, one word per line. |

`dotnet run` and the VS profile both set `ASPNETCORE_ENVIRONMENT=Development`, which picks up the dev JWT key automatically. If you run the published DLL directly (`dotnet SuperScrabble.WebApi.dll`), the environment is Production and you must provide `JWT__SIGNINGKEY` yourself.

## 5. Running the tests

```powershell
cd src\Server
dotnet test -c Release
```

> **Use `-c Release` on this machine.** Windows Smart App Control intermittently blocks freshly built DLLs in `Debug` output folders (error `0x800711C7`). This is a Windows policy quirk, not a code problem.

Frontend production build check:

```powershell
cd src\ClientApp\super-scrabble-app
npm run build
```

## 6. Troubleshooting

- **"The signing key is missing or too short" on startup** — you're running without the Development environment; set `Jwt:SigningKey` in `appsettings.json` or the `JWT__SIGNINGKEY` environment variable (≥ 32 chars).
- **SQL connection errors mentioning certificates/encryption** — make sure the connection string contains `TrustServerCertificate=True;` (SqlClient encrypts by default since EF Core 7).
- **Words were not seeded** — seeding is skipped whenever the `Words` table is non-empty. To force a re-seed, drop the database (`sqllocaldb` instances: delete the `SuperScrabble` database in SSMS or via `DROP DATABASE`) and boot again; or check the startup log for a warning that the words directory was not found.
- **Login works but the game page doesn't connect** — the SignalR hub uses the same origin (`https://localhost:7168/gamehub`) with the JWT passed as `access_token`. If you changed API ports, update `serverUrl` in `src\ClientApp\super-scrabble-app\src\environments\environment.ts` *and* the CORS origins in `Program.cs`.
- **`npm install` dependency conflicts** — should not happen thanks to `.npmrc`; if you deleted it, run `npm install --legacy-peer-deps`.
- **Texts on the home page are empty** — home-page labels (game-mode names/descriptions) are loaded from Firebase Remote Config, so the first load needs internet access.
