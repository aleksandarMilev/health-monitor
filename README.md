# Health Monitor

Project skeleton for the **NASA Space Apps Challenge 2026**: _Create Health Monitoring Software for Astronauts on Space Missions_.

It contains **plumbing only**: structure, configuration, deployment and one generic example feature (`Samples`). Everything about the challenge itself (health indicators, thresholds, statuses, guidance, UI) is **your team's work**.

---

## Table of contents

1. [First 30 minutes](#1-first-30-minutes)
2. [Run it locally](#2-run-it-locally)
3. [File map: what to touch](#3-file-map-what-to-touch)
4. [Add a new feature](#4-add-a-new-feature)
5. [Tech stack](#5-tech-stack)
6. [Project tour](#6-project-tour)
7. [Configuration](#7-configuration)
8. [Working with data (3 patterns)](#8-working-with-data-3-patterns)
9. [Testing with Scalar](#9-testing-with-scalar)
10. [Git: first-time setup](#10-git-first-time-setup)
11. [Git: team workflow](#11-git-team-workflow)
12. [Deploy to Render](#12-deploy-to-render)
13. [Troubleshooting](#13-troubleshooting)
14. [Submission checklist](#14-submission-checklist)

---

## 1. First 30 minutes

**Everyone**

1. Install the [.NET 10 SDK](https://dotnet.microsoft.com/download) and [Git](https://git-scm.com/downloads). Docker Desktop is optional.
2. Run the skeleton once (the owner uses the unzipped folder; everyone else waits for step 6 and uses their clone):

   ```bash
   cd src/HealthMonitor.Api
   dotnet run
   ```

3. Open these and check they work:
   - <http://localhost:5080/>: home page shows "Online"
   - <http://localhost:5080/#/samples>: table + chart
   - <http://localhost:5080/scalar>: API docs

**Team owner only**

4. Do [10. Git: first-time setup](#10-git-first-time-setup): Git identity, GitHub repo, push, invite teammates.
5. Do [12. Deploy to Render](#12-deploy-to-render) and share the live URL in the team chat.

**Everyone else**

6. Set your Git identity ([step 1 of section 10](#10-git-first-time-setup)), accept the GitHub invite, then clone:

   ```bash
   git clone https://github.com/<owner>/health-monitor.git
   cd health-monitor
   ```

**Together**

7. Split the work **by feature**: each person owns one page folder (`wwwroot/js/pages/<page>/`) plus its own controller/service. See [3. File map](#3-file-map-what-to-touch).

---

## 2. Run it locally

```bash
cd src/HealthMonitor.Api
dotnet run
```

| What     | URL                            |
| -------- | ------------------------------ |
| App      | <http://localhost:5080>        |
| API docs | <http://localhost:5080/scalar> |

The port comes from `src/HealthMonitor.Api/Properties/launchSettings.json`. A browser tab opens automatically.

**Alternatives**

- Visual Studio / Rider: open `health-monitor.slnx` and press **Run**.
- Auto-restart on C# changes:

  ```bash
  cd src/HealthMonitor.Api
  dotnet watch
  ```

Changed a `.js`, `.html` or `.css` file? Refresh the browser (Ctrl+F5). Changed C# or `appsettings*.json`? Restart the app.

### Optional: Docker check (same container as Render)

Only needed if Render fails to build, or before you change the `Dockerfile` or `.csproj`. Requires Docker Desktop. From the **repository root**:

```bash
docker build -t health-monitor .
docker run --rm -p 8080:8080 health-monitor
```

Open <http://localhost:8080>. Stop with `Ctrl+C`.

Inside the container:

- The home page shows the environment as **Production**.
- `appsettings.Local.json` is **not** included, so the app uses the values in `appsettings.json`.

---

## 3. File map: what to touch

All paths below are inside `src/HealthMonitor.Api/`.

### ✅ You'll work here (your own files)

| What             | Where                                                                                       |
| ---------------- | ------------------------------------------------------------------------------------------- |
| Controllers      | `Controllers/<Name>Controller.cs`                                                           |
| Services         | `Services/<Name>/I<Name>Service.cs` + `<Name>Service.cs`                                    |
| Entities         | `Models/Entities/<Name>.cs`                                                                 |
| DTOs             | `Models/Dtos/<Name>CreateDto.cs`, `<Name>ResponseDto.cs`                                    |
| Seed row classes | `Models/Seed/<Name>SeedRow.cs`                                                              |
| Seed files       | `Data/Files/<name>.json` or `<name>.csv`                                                    |
| Pages            | `wwwroot/js/pages/<page>/<page>.html` + `<page>.js`                                         |
| Styles           | `wwwroot/css/style.css`: add your rules **at the end**, under a comment with your page name |

### ✏️ Shared files: one-line edits only

| File                                        | The only change allowed                                                                               |
| ------------------------------------------- | ----------------------------------------------------------------------------------------------------- |
| `Data/AppDbContext.cs`                      | Add one `DbSet` line (and optionally a model config block for your entity)                            |
| `Extensions/ServiceCollectionExtensions.cs` | Add one `.AddScoped<IFoo, Foo>()` line inside `AddServices()`                                         |
| `Data/Seeding/DataSeeder.cs`                | Add one `await Seed...Async(cancellationToken);` call in `Seed()`, plus your own method at the bottom |
| `wwwroot/js/router.js`                      | Add one line to the `routes` object                                                                   |
| `wwwroot/index.html`                        | Add one `<a>` nav link                                                                                |
| `appsettings.json`                          | Change **config values** only. **Never a real API key** (the repo is public)                          |

### 🔒 Don't touch: use it like a library

| File                                              | What you call                                                                                                                            |
| ------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------- |
| `Program.cs`                                      | Nothing. It's already wired up.                                                                                                          |
| `Extensions/*` (except the one registration line) | Nothing                                                                                                                                  |
| `Data/Seeding/JsonLoader.cs`                      | `await jsonLoader.Load<T>("file.json", cancellationToken)` → `List<T>`                                                                   |
| `Data/Seeding/CsvLoader.cs`                       | `await csvLoader.Load<T>("file.csv", cancellationToken)` → `List<T>`                                                                     |
| `Data/Seeding/SeedFilePaths.cs`                   | Nothing (used by the loaders)                                                                                                            |
| `Services/External/ExternalApiClient.cs`          | Inject `IExternalApiClient`: `await externalApi.Get<T>("path", cancellationToken)`                                                       |
| `Options/*.cs`                                    | Nothing. Change values in `appsettings.json` instead                                                                                     |
| `wwwroot/js/api.js`                               | `api.get(url)`, `api.post(url, body)`, `api.put(url, body)`, `api.delete(url)`. Errors throw an `ApiError` with `.message` and `.status` |
| `wwwroot/js/router.js` (internals)                | `navigate("/samples")` to change page from code                                                                                          |
| `wwwroot/js/charts.js`                            | `createLineChart(canvas, labels, datasets, extraOptions?)` returns a Chart. Call `.destroy()` on it later                                |
| `wwwroot/js/utils.js`                             | `escapeHtml(v)`, `formatDateTime(iso)`, `formatShortDateTime(iso)`, `showMessage(el, text, type)`, `hideMessage(el)`                     |
| `Properties/launchSettings.json`                  | Nothing                                                                                                                                  |
| `HealthMonitor.Api.csproj`                        | Only via `dotnet add package <Name>`, and tell the team first                                                                            |
| `Dockerfile`, `.gitignore`, `.dockerignore`       | Nothing                                                                                                                                  |

---

## 4. Add a new feature

Example: a `Reading` resource with its own page. Copy the `Samples` files and adapt them.

Use namespaces that match the folder, like the existing files do, e.g. `namespace HealthMonitor.Api.Models.Entities;`.

### Backend

1. **Entity:** `Models/Entities/Reading.cs`.
2. **DTOs:** `Models/Dtos/ReadingCreateDto.cs` + `ReadingResponseDto.cs`, with validation attributes (`[Required]`, `[Range]`, `[StringLength]`, …) on the create DTO.
3. **DbSet:** in `Data/AppDbContext.cs` add:

   ```csharp
   public DbSet<Reading> Readings { get; init; }
   ```

4. **Service:** `Services/Readings/IReadingService.cs` + `ReadingService.cs`. Copy `Services/Samples/` and adapt.
5. **Register it:** in `Extensions/ServiceCollectionExtensions.cs`, inside `AddServices()`, add your line **above the last line** (so you don't move the `;`):

   ```csharp
   public IServiceCollection AddServices()
       => services
           .AddScoped<JsonLoader>()
           .AddScoped<CsvLoader>()
           .AddScoped<DataSeeder>()
           .AddScoped<IReadingService, ReadingService>()   // ← your line
           .AddScoped<ISampleService, SampleService>();
   ```

   Add the matching `using Services.Readings;` at the top of the file.

6. **Controller:** `Controllers/ReadingsController.cs`. Copy `SamplesController` and adapt. `[Route("api/[controller]")]` makes it `/api/readings`.
7. **Check:** run the app and open `/scalar`. Your endpoints appear automatically.

Every async method in the codebase takes a `CancellationToken` as its last parameter. Accept it in your controller action and pass it on, like `SamplesController` does.

**Optional: seed data**

1. `Models/Seed/ReadingSeedRow.cs` (property names = JSON keys or CSV headers).
2. `Data/Files/readings.json` (a JSON **array**). It's copied to the build output automatically.
3. In `Data/Seeding/DataSeeder.cs`, add a call in `Seed()` under `//add more seeders here.`, and your method at the bottom of the class:

   ```csharp
   // in Seed():
   await SeedReadingsAsync(cancellationToken);

   // new method:
   private async Task SeedReadingsAsync(CancellationToken cancellationToken)
   {
       if (await data.Readings.AnyAsync(cancellationToken))
           return;

       var rows = await jsonLoader.Load<ReadingSeedRow>("readings.json", cancellationToken);

       data.AddRange(rows.Select(r => new Reading
       {
           // map the fields from r
       }));

       await data.SaveChangesAsync(cancellationToken);
   }
   ```

### Frontend

1. Copy `wwwroot/js/pages/samples/` to `wwwroot/js/pages/readings/` and rename the files to `readings.html` and `readings.js`. **Folder name = file names = route value.**
2. `wwwroot/js/router.js`: add to `routes`:

   ```js
   "/readings": "readings",
   ```

3. `wwwroot/index.html`: add to the `<nav>`:

   ```html
   <a href="#/readings" data-route="/readings">Readings</a>
   ```

Each page module exports `init(root)` (called when the page opens) and optionally `destroy()` (called before leaving the page).

### Frontend rules of thumb

- **Always** use `escapeHtml()` when putting API/user data into `innerHTML`, or use `textContent`.
- **Always** call `chart.destroy()` in your page's `destroy()` and before redrawing a chart.
- Query elements **inside your page's root** (`root.querySelector(...)`), not `document`.
- Use the CSS variables (`var(--color-ok)`, …) instead of hard-coded colors.
- Status classes are ready: `badge status-ok | status-warning | status-critical | status-info`. Deciding **which** value gets which status is your challenge logic.
- Charts use a category x-axis: points are evenly spaced, not proportional to time.

---

## 5. Tech stack

| Part     | Technology                                                                  |
| -------- | --------------------------------------------------------------------------- |
| Backend  | ASP.NET Core (.NET 10), classic **controllers**                             |
| Database | **EF Core InMemory**: no migrations, no database server                     |
| API docs | OpenAPI + **Scalar** UI at `/scalar`                                        |
| Frontend | **Vanilla JavaScript** (ES modules), HTML, CSS: no framework, no build step |
| Charts   | **Chart.js 4.6.1** (loaded from a CDN)                                      |
| Hosting  | **Render**: one Web Service built from the `Dockerfile`                     |

**One app, one URL.** The backend serves the frontend from `wwwroot/`, so the frontend calls the API with relative URLs (`/api/samples`). No CORS, no second deployment.

> ⚠️ **In-memory database = data lives in RAM.** Every restart or redeploy wipes it. On startup, `DataSeeder` refills it from the seed files. Anything a user adds is lost on the next restart. That's fine for a hackathon demo.

---

## 6. Project tour

```
health-monitor/
├── health-monitor.slnx               ← solution file (open in VS / Rider)
├── Dockerfile                        ← used by Render (and the optional Docker check)
├── .gitignore / .dockerignore
└── src/HealthMonitor.Api/
    ├── Program.cs                    ← app setup (calls the Extensions/ methods)
    ├── appsettings.json              ← committed configuration (Seeding, ExternalApi)
    ├── appsettings.Local.json        ← OPTIONAL, you create it, git-ignored (real keys)
    ├── HealthMonitor.Api.csproj
    ├── Properties/launchSettings.json  ← local port 5080
    │
    ├── Controllers/                  ← HTTP endpoints, keep them THIN
    │   ├── StatusController.cs       ← GET /api/status (Render health check)
    │   ├── SamplesController.cs      ← example CRUD: copy it
    │   └── ExternalController.cs     ← GET /api/external?path=... (external API test)
    │
    ├── Services/                     ← business logic, the only place that uses the DbContext
    │   ├── Samples/                  ← ISampleService + SampleService
    │   └── External/                 ← IExternalApiClient + ExternalApiClient
    │
    ├── Models/
    │   ├── Entities/                 ← what is stored in the DB
    │   ├── Dtos/                     ← what the API receives / returns
    │   └── Seed/                     ← shape of rows in the seed files
    │
    ├── Options/                      ← config classes
    │   ├── ApiKeyLocation.cs         ← None | Query | Header | Bearer
    │   ├── ExternalApiOptions.cs
    │   └── SeedingOptions.cs
    │
    ├── Extensions/
    │   ├── ServiceCollectionExtensions.cs  ← DI registrations (AddServices() = your line)
    │   └── WebApplicationExtensions.cs     ← runs the seeder at startup
    │
    ├── Data/
    │   ├── AppDbContext.cs           ← one DbSet per entity
    │   ├── Seeding/                  ← DataSeeder, JsonLoader, CsvLoader, SeedFilePaths
    │   └── Files/                    ← seed files: samples.json, samples.csv
    │
    └── wwwroot/                      ← THE FRONTEND
        ├── index.html                ← shell only: header, nav, <main id="app">, footer
        ├── css/style.css             ← colors (incl. status colors) + components
        └── js/
            ├── main.js               ← entry point
            ├── router.js             ← maps #/url → page
            ├── api.js                ← fetch wrapper for our backend
            ├── charts.js             ← Chart.js helper
            ├── utils.js              ← escapeHtml, date formatting, messages
            └── pages/
                ├── home/             ← home.html + home.js
                ├── samples/          ← samples.html + samples.js (reference page)
                ├── external/         ← external.html + external.js
                └── not-found/        ← not-found.html + not-found.js
```

### Request flow

```
Browser ──fetch('/api/samples')──▶ SamplesController ──▶ SampleService ──▶ AppDbContext (in memory)
   ▲                                                                              │
   └────────────────────────────── JSON (SampleResponseDto) ◀─────────────────────┘
```

### How the frontend pages work

`index.html` never changes per page. When the URL is `#/samples`, `router.js`:

1. calls the old page's `destroy()` (if it has one)
2. fetches `js/pages/samples/samples.html` and puts it inside `<main id="app">`
3. imports `js/pages/samples/samples.js` and calls its `init(root)`

**Each page = one folder. Each person owns their page(s) → almost no merge conflicts.**

---

## 7. Configuration

### Where settings come from

| #   | Source                              | Where                                    | In Git?            | Used             |
| --- | ----------------------------------- | ---------------------------------------- | ------------------ | ---------------- |
| 1   | `appsettings.json`                  | `src/HealthMonitor.Api/`                 | ✅ yes (public!)   | Always: defaults |
| 2   | Environment variables               | Render dashboard → **Environment**       | ❌ no              | On Render        |
| 3   | `appsettings.Local.json` (optional) | `src/HealthMonitor.Api/` (you create it) | ❌ no, git-ignored | On your laptop   |

**Later sources win, key by key.** You only write the keys you want to change, and everything else comes from `appsettings.json`. `appsettings.Local.json` is loaded last, so locally it wins over everything. It never exists on Render.

> 🔑 **Hard rule: real API keys never go in `appsettings.json`.** The repo is public. Use `appsettings.Local.json` locally and an environment variable on Render.

### `appsettings.Local.json`

Create `src/HealthMonitor.Api/appsettings.Local.json` with **only the keys that differ**. Usually just the key:

```json
{
  "ExternalApi": {
    "ApiKey": "paste-your-real-key-here"
  }
}
```

Don't copy the whole `appsettings.json` into it. If you do, your local file silently overrides every later change your teammates make to `appsettings.json`. Restart the app after editing it.

### Naming: `:` in JSON, `__` in environment variables

| In JSON (nesting)                             | As a key name                | As an environment variable (Render)         |
| --------------------------------------------- | ---------------------------- | ------------------------------------------- |
| `{ "ExternalApi": { "ApiKey": "..." } }`      | `ExternalApi:ApiKey`         | `ExternalApi__ApiKey` (**two** underscores) |
| `{ "ExternalApi": { "TimeoutSeconds": 30 } }` | `ExternalApi:TimeoutSeconds` | `ExternalApi__TimeoutSeconds`               |

### All settings

| Key                          | Default                 | Meaning                                                       |
| ---------------------------- | ----------------------- | ------------------------------------------------------------- |
| `Seeding:UseJson`            | `true`                  | Seed samples from the JSON file                               |
| `Seeding:JsonFile`           | `samples.json`          | File in `Data/Files/`                                         |
| `Seeding:UseCsv`             | `false`                 | Seed samples from the CSV file (can be on together with JSON) |
| `Seeding:CsvFile`            | `samples.csv`           | File in `Data/Files/`                                         |
| `ExternalApi:BaseUrl`        | `https://api.nasa.gov/` | Base URL of the external API                                  |
| `ExternalApi:ApiKeyLocation` | `Query`                 | `None`, `Query`, `Header` or `Bearer`                         |
| `ExternalApi:ApiKeyName`     | `api_key`               | Query parameter or header name (not used for `None`/`Bearer`) |
| `ExternalApi:ApiKey`         | `DEMO_KEY`              | The key. Real keys only in `appsettings.Local.json` / Render  |
| `ExternalApi:TimeoutSeconds` | `15`                    | Request timeout                                               |

---

## 8. Working with data (3 patterns)

### Pattern 1: JSON file (on by default)

For reference data you write yourselves (normal ranges, explanations, tips) and demo data.

1. Put a JSON **array** in `Data/Files/`, e.g. `readings.json`.
2. Create a matching class in `Models/Seed/`. Property names = JSON keys, case-insensitive.
3. Load it (e.g. in `DataSeeder`, which already has `jsonLoader`):

   ```csharp
   var rows = await jsonLoader.Load<ReadingSeedRow>("readings.json", cancellationToken);
   ```

Files in `Data/Files/` are copied to the build output automatically.

### Pattern 2: CSV file (off by default)

For tabular data you downloaded (spreadsheets, datasets).

1. Put the CSV in `Data/Files/`.
2. Format rules:
   - The first row is a **header**. Column names = property names, case-insensitive.
   - Values are **comma-separated** (`,`), decimals use a **dot** (`36.6`).
   - Dates look like `2026-11-01T08:00:00Z`. Dates without a time zone are treated as UTC.
3. Load it:

   ```csharp
   var rows = await csvLoader.Load<ReadingSeedRow>("readings.csv", cancellationToken);
   ```

> ⚠️ Excel uses your Windows list separator, so with a Bulgarian (or other European) locale it saves `;`-separated files with decimal commas, **even when you pick "CSV (Comma delimited)"**. Always open the saved file in a text editor and check: `,` between values and `.` in numbers. Fix it with Find & Replace if needed. A wrong file doesn't crash; it loads rows with empty names and zero values.
>
> For data you type yourselves, prefer JSON ([Pattern 1](#pattern-1-json-file-on-by-default)). Use CSV only for downloaded datasets.

**See it in action:** set `"UseCsv": true` under `Seeding` (in `appsettings.Local.json` to try it just for you, or in `appsettings.json` for the whole team) and restart. "Metric C" from `samples.csv` appears next to the JSON data.

### Pattern 3: External HTTP API (NASA demo by default)

For live data from a public API. The browser never talks to the external API directly:

```
Browser ──▶ /api/external?path=... (our backend adds the key) ──▶ external API
```

So the key never reaches the browser. The skeleton is wired for **one** external API, configured in the `ExternalApi` section.

#### Pick your case and paste it into `appsettings.json`

**1. No key** (e.g. Open-Meteo):

```json
"ExternalApi": {
  "BaseUrl": "https://api.open-meteo.com/v1/",
  "ApiKeyLocation": "None",
  "ApiKeyName": "",
  "ApiKey": "",
  "TimeoutSeconds": 15
}
```

**2. Key in the query string** (NASA, `?api_key=...`). **This is the default:**

```json
"ExternalApi": {
  "BaseUrl": "https://api.nasa.gov/",
  "ApiKeyLocation": "Query",
  "ApiKeyName": "api_key",
  "ApiKey": "DEMO_KEY",
  "TimeoutSeconds": 15
}
```

**3. Key in a header** (e.g. `X-Api-Key: ...`):

```json
"ExternalApi": {
  "BaseUrl": "https://api.example.com/",
  "ApiKeyLocation": "Header",
  "ApiKeyName": "X-Api-Key",
  "ApiKey": "",
  "TimeoutSeconds": 15
}
```

**4. Bearer token** (`Authorization: Bearer ...`):

```json
"ExternalApi": {
  "BaseUrl": "https://api.example.com/",
  "ApiKeyLocation": "Bearer",
  "ApiKeyName": "",
  "ApiKey": "",
  "TimeoutSeconds": 15
}
```

Whenever you use a real key (cases 2–4), it goes in `appsettings.Local.json` locally ([see 7](#7-configuration)) and in the `ExternalApi__ApiKey` environment variable on Render ([see 12](#12-deploy-to-render)). An empty key with `Query`, `Header` or `Bearer` gives a 503 with a clear message.

`ApiKeyLocation` must be one of `None`, `Query`, `Header`, `Bearer`. Any other word (e.g. `QueryString`) makes `/api/external` fail with a 500 ([see 13](#13-troubleshooting)).

> ⚠️ **`DEMO_KEY` is shared and has very low rate limits.** You'll soon get `429 Too Many Requests`, especially on Render. Get a free personal key in one minute at <https://api.nasa.gov> and set it as described above.

#### Test it

- In the app: the **External API** page. Enter a path such as `planetary/apod`.
- In Scalar: `GET /api/external` with `path` = `planetary/apod` ([see 9](#9-testing-with-scalar)).

#### Status codes from `/api/external`

| Code | Meaning                                                         | What to do                                                                     |
| ---- | --------------------------------------------------------------- | ------------------------------------------------------------------------------ |
| 200  | The external API's JSON, passed through                         | n/a                                                                            |
| 400  | Missing path, or a full URL (`https://...`) was given           | Use a **relative** path, e.g. `planetary/apod`                                 |
| 502  | The external API answered with an error, or couldn't be reached | Read `detail`: it contains the external status and message (e.g. `429`, `403`) |
| 503  | Configuration is incomplete                                     | Read `detail`: it says exactly which setting is missing                        |
| 504  | No answer within `TimeoutSeconds`                               | Try again, or increase `ExternalApi:TimeoutSeconds`                            |

#### Use it in your own service

`IExternalApiClient` is already registered. Inject it and call `Get<T>` with a class that matches the API's JSON:

```csharp
public class ApodService(IExternalApiClient externalApi) : IApodService
{
    public async Task<ApodDto?> GetToday(CancellationToken cancellationToken = default)
        => await externalApi.Get<ApodDto>("planetary/apod", cancellationToken);
}
```

`Get<T>` throws `HttpRequestException` when the external API returns an error. See `ExternalController` for how to turn exceptions into proper responses. `GetRaw(path, cancellationToken)` returns the raw JSON string instead.

---

## 9. Testing with Scalar

1. Run the app and open <http://localhost:5080/scalar> (or `https://<your-service>.onrender.com/scalar`).
2. Pick an endpoint on the left, e.g. `POST /api/Samples`.
3. Click **Test Request**, then fill in the parameters or the JSON body.
4. Click **Send** and check the status code and response. A 400 lists the validation errors per field.

New controllers show up automatically after a restart.

---

## 10. Git: first-time setup

**1. Everyone, once per computer:** set your identity (it's shown on every commit):

```bash
git config --global user.name "Your Name"
git config --global user.email "you@example.com"
```

**2. Team owner only:** create the local repository from the unzipped folder:

```bash
# in the repository root (the folder with this README)
git init -b main
git add .
git commit -m "Initial commit"
```

**3. Team owner, on GitHub:**

1. **New repository** → name `health-monitor` → **Public** (judges must be able to open it) → do **not** add a README, .gitignore or license (we already have them).
2. Run the commands GitHub shows under "push an existing repository":

   ```bash
   git remote add origin https://github.com/<owner>/health-monitor.git
   git push -u origin main
   ```

   When `git push` asks you to sign in, choose **Sign in with your browser**. Git for Windows includes Git Credential Manager for this. GitHub does **not** accept your account password in the terminal. Alternative: use [GitHub Desktop](https://desktop.github.com/).

3. **Settings → Collaborators** → invite every teammate.
4. _(Optional)_ Protect `main`: **Settings → Rules → Rulesets → New ruleset → New branch ruleset**:
   - Enforcement status: **Active**
   - Target branches: **Include default branch**
   - Enable **Require a pull request before merging**, and set **Required approvals** to **0**, so nobody gets blocked at 3 a.m. waiting for a review.

**4. Everyone else:** accept the invite (check your email or GitHub notifications), then:

```bash
git clone https://github.com/<owner>/health-monitor.git
cd health-monitor
```

---

## 11. Git: team workflow

```
main ── always working, auto-deploys to Render
  │
  ├── feature/readings-api        (one branch per task)
  ├── feature/dashboard-page
  └── fix/chart-labels
```

For every task:

```bash
git switch main
git pull                                  # start from the latest main
git switch -c feature/short-description   # new branch

# ...work, commit often...
git add .
git commit -m "Add readings endpoint"

git push -u origin feature/short-description
```

Then on GitHub: **Compare & pull request** → describe what you did → ask a teammate to look → **Merge** → delete the branch.

**Rules**

- Never work directly on `main`.
- Small branches, merged often (hours, not a whole day). Big branches = big conflicts.
- Before opening a PR: `git pull origin main` into your branch, run the app, make sure it works.
- Merging to `main` deploys to Render automatically. **A broken `main` = a broken live demo.**
- Each person owns their own files/folders (their controller, service, page folder).
- Shared files get **one-line** changes only (see [3. File map](#3-file-map-what-to-touch)): `AppDbContext.cs`, `ServiceCollectionExtensions.cs`, `DataSeeder.cs`, `router.js`, `index.html`, `appsettings.json`, `style.css`.

**Resolving the most common conflict**

Two people both added a registration to `AddServices()`. Git shows:

```
<<<<<<< HEAD
        .AddScoped<IReadingService, ReadingService>()
=======
        .AddScoped<IAlertService, AlertService>()
>>>>>>> feature/alerts
```

**Keep both lines** and delete the three marker lines (`<<<<<<<`, `=======`, `>>>>>>>`). Check that only the **last** line of the chain ends with `;`. Then build, commit and push. The same applies to `routes` in `router.js` and the nav links in `index.html`: keep both.

---

## 12. Deploy to Render

Render builds the `Dockerfile` from your GitHub repo and runs it. The free tier is enough. **Push to GitHub first** ([section 10](#10-git-first-time-setup)).

1. Sign in at <https://render.com> with GitHub.
2. **New → Web Service** → pick the `health-monitor` repository.
3. Settings:

   | Field              | Value                                                               |
   | ------------------ | ------------------------------------------------------------------- |
   | Name               | `health-monitor` (becomes `health-monitor.onrender.com` or similar) |
   | Language / Runtime | **Docker**                                                          |
   | Branch             | `main`                                                              |
   | Root Directory     | _(empty)_                                                           |
   | Dockerfile Path    | `./Dockerfile`                                                      |
   | Instance Type      | Free                                                                |

4. **Environment Variables**:

   | Key                   | Value         | When                                |
   | --------------------- | ------------- | ----------------------------------- |
   | `PORT`                | `8080`        | **Always.** The app listens on 8080 |
   | `ExternalApi__ApiKey` | your real key | Only if you use a real key          |

   Any other setting from [7. Configuration](#7-configuration) works the same way, with `__` instead of `:`.

   > ⚠️ Use **two** underscores. A misspelled name (`ExternalApi_ApiKey`, `ExternalAPI__Apikey2`) is **silently ignored**, and the app keeps using the value from `appsettings.json`.

5. **Advanced**:
   - **Health Check Path**: `/api/status`
   - **Auto-Deploy**: On Commit
6. **Create Web Service**. The first build takes a few minutes; watch the logs.
7. Open `https://<your-service>.onrender.com` and `/scalar`.

From now on, every merge to `main` redeploys automatically.

**Things to know**

- Render **doesn't read `appsettings.Local.json`**: it's not in Git. Everything secret on Render comes from environment variables.
- **Change an environment variable later:** open the service → **Environment** → edit → save. Render redeploys with the new value. If it doesn't, use **Manual Deploy → Deploy latest commit**.
- **Free services sleep after 15 minutes without traffic.** The first request after that takes about a minute, and the in-memory data is re-seeded. Open the site a minute before anyone looks at it.
- **Build failed?** Run the [optional Docker check](#optional-docker-check-same-container-as-render) locally. You'll see the same error.

---

## 13. Troubleshooting

| Symptom                                                | Cause / fix                                                                                                                                                                        |
| ------------------------------------------------------ | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `dotnet run`: "address already in use" / port 5080     | The app is already running somewhere (another terminal, VS, Rider). Stop it and run again.                                                                                         |
| Docker check: port 8080 in use                         | Use another local port: `docker run --rm -p 8081:8080 health-monitor`, then open <http://localhost:8081>.                                                                          |
| Page shows **Failed to load page "x"**                 | The route value in `router.js` must equal the folder name **and** both file names: `pages/x/x.html` + `pages/x/x.js`. Press F12 → Console for details.                             |
| Chart is missing                                       | Chart.js loads from a CDN. Check your internet connection (F12 → Console: "Chart.js is not loaded").                                                                               |
| External API: **429 Too Many Requests** (inside a 502) | `DEMO_KEY` rate limit. Get a free key at <https://api.nasa.gov> and set it ([Pattern 3](#8-working-with-data-3-patterns)).                                                         |
| External API: **500** with a generic message           | Usually a config typo, e.g. `"ApiKeyLocation": "QueryString"`. Read the console where `dotnet run` runs (or Render **Logs**). Allowed values: `None`, `Query`, `Header`, `Bearer`. |
| External API: **503**                                  | Read the `detail` message. It names the missing or invalid setting.                                                                                                                |
| My data disappeared                                    | In-memory database: every restart/redeploy (and Render's sleep) wipes it and re-seeds from `Data/Files/`.                                                                          |
| CSV loads rows with empty names / zero values          | Wrong separator or decimal comma. The file must use `,` between values and `.` in numbers ([Pattern 2](#8-working-with-data-3-patterns)).                                          |
| Changed `appsettings*.json` but nothing changed        | Restart the app. On Render, change environment variables instead (the JSON comes from Git).                                                                                        |
| Render deploy fails                                    | Read the Render logs. Run the [optional Docker check](#optional-docker-check-same-container-as-render) locally to reproduce. Check that `PORT` = `8080` is set.                    |
| `git push` asks for a password                         | GitHub doesn't accept passwords. Choose **Sign in with your browser**, or use GitHub Desktop ([section 10](#10-git-first-time-setup)).                                             |

---

## 14. Submission checklist

Deadline: **Sunday, November 15, 23:59** (local time of the team owner). No edits after that.

- [ ] Project page completed **in English**.
- [ ] **Link to final project**: public GitHub repo and/or the Render URL. Test both in a private browser window (no login required).
- [ ] **Demo**: max **7 slides** (incl. title) **or** max **30-second video** with English subtitles. Over the limit = not eligible for global judging.
- [ ] **No recognizable names, faces or voices of anyone under 18** in the submission.
- [ ] **NASA data** field: which NASA resources you used and how (required for global awards).
- [ ] **References**: list _everything_ external - Chart.js, Scalar, CsvHelper, and any images/icons.
- [ ] **No real API keys in the repo.** Search the GitHub repo for your key before submitting. `appsettings.json` should still contain only `DEMO_KEY` or an empty key.
- [ ] Every team member is registered at the same event and listed on the team page.
- [ ] Clicked **Submit for Judging** and saw the confirmation banner.
- [ ] **Remove the demo code.** Judges should see your app, not the skeleton's example. Delete the `Samples` feature end to end (all paths inside `src/HealthMonitor.Api/`):
  - `Controllers/SamplesController.cs`
  - `Services/Samples/` (the whole folder)
  - `Models/Entities/Sample.cs`, `Models/Dtos/SampleCreateDto.cs`, `Models/Dtos/SampleResponseDto.cs`, `Models/Seed/SampleSeedRow.cs`
  - `Data/Files/samples.json`, `Data/Files/samples.csv`
  - `wwwroot/js/pages/samples/` (the whole folder)
  - Its lines in the shared files: the `Samples` `DbSet` in `AppDbContext.cs`, the `ISampleService` registration in `AddServices()`, the samples seeding call and method in `DataSeeder.cs`, the `/samples` route in `router.js`, and the nav link in `index.html`
  - ⚠️ The `ISampleService` registration may be the **last** line of the `AddServices()` chain. If it is, move its `;` to the new last line.
  - `Seeding` settings: point `JsonFile` / `CsvFile` at your own files, or remove the `Seeding` section and `SeedingOptions` if you no longer use them
- [ ] **Not using an external API?** Delete `Controllers/ExternalController.cs`, `wwwroot/js/pages/external/`, its route and its nav link. Using one? Keep the External page only if it's part of your demo.
- [ ] **Replace the placeholder content:** the home page text ("Project skeleton…"), the status-color demo card, and the app name in the header, footer and `<title>` if you renamed the project.
- [ ] **Keep:** `StatusController` (Render's health check), the `not-found` page, and all "don't touch" files from [3. File map](#3-file-map-what-to-touch).
- [ ] After removing the demo code: `dotnet build` with 0 errors, click through every page, check F12 → Console for errors, then merge and confirm that the Render deploy works.
- [ ] **Rewrite this README for your project.** Delete the skeleton's content and describe _your_ app instead:
  - what it does and which astronaut health problem it solves
  - its main features, with screenshots (no faces of team members under 18)
  - the live Render URL
  - how to run it locally (keep the short `dotnet run` part)
  - the NASA data and resources you used
  - your tech stack
  - the team (first names or nicknames only)
  - References, including this skeleton
  - an AI usage note
