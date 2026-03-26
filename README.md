# PlanIt - Projekthantering

En Trello-liknande projekthanteringsapp byggd med ASP.NET Core och Blazor Server.

## Teknikstack

| Lager | Teknologi |
|-------|-----------|
| Backend | ASP.NET Core Web API (.NET 10) |
| Frontend | Blazor Server |
| Databas | SQLite + Entity Framework Core |
| Autentisering | JWT (Access Token + Refresh Token) |
| Testning | xUnit + Moq |

## Projektstruktur

```
Projekthantering/
├── Projekthantering/           # API-projekt (backend)
│   ├── Controllers/            # API-endpoints
│   ├── Services/               # Affärslogik
│   ├── Repositories/           # Databasåtkomst
│   ├── Models/                 # Entiteter (User, Board, BoardList, Card)
│   └── Data/                   # DbContext och konfiguration
├── Projekthantering.Client/    # Blazor Server (frontend)
│   ├── Components/
│   │   ├── Pages/              # Sidor (Login, Register, Boards, Settings)
│   │   ├── Shared/             # Komponenter (ListComponent, CardComponent)
│   │   └── Layout/             # MainLayout, NavMenu
│   └── Services/               # Klient-tjänster (AuthService, BoardService)
├── Projekthantering.Shared/    # Delade DTOs och valideringsmodeller
└── Projekthantering.Tests/     # Enhetstester
    ├── Services/               # Service-tester
    ├── Controllers/            # Controller-tester
    └── Repositories/           # Repository-tester
```

## Kom igång

### Förutsättningar

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- En IDE (Visual Studio 2022+ eller VS Code)

### Installation

1. Klona repot:
   ```bash
   git clone https://github.com/JackGriph/Projekthantering.git
   cd Projekthantering
   ```

2. Starta API:et (Terminal 1):
   ```bash
   cd Projekthantering
   dotnet run
   ```

3. Starta klienten (Terminal 2):
   ```bash
   cd Projekthantering.Client
   dotnet run
   ```

4. Öppna webbläsaren:
   - **Frontend:** https://localhost:7147
   - **API (Swagger):** https://localhost:7191/swagger

### Köra tester

```bash
cd Projekthantering.Tests
dotnet test
```

## Funktioner

### Autentisering
- Registrera konto med e-post och lösenord
- Logga in/ut med JWT-tokens
- Automatisk tokenförnyelse med refresh tokens
- Skyddade sidor och API-endpoints

### Tavlor (Boards)
- Skapa, redigera och ta bort tavlor
- Visa egna tavlor på en översiktssida

### Listor
- Skapa, redigera och ta bort listor inom en tavla
- Kanban-vy med listor sida vid sida

### Kort (Cards)
- Skapa, redigera och ta bort kort
- Drag-and-drop för att flytta kort mellan listor
- Tilldela kort till användare
- Sätta status (Att göra, Pågående, Klar)

### Profil och inställningar
- Visa profilinformation
- Byta lösenord
- Ta bort konto

## Arkitektur

Projektet följer en **lagerarkitektur** med tydlig separation:

```
Controller → Service → Repository → Databas
```

- **Controllers** hanterar HTTP-förfrågningar och validering
- **Services** innehåller affärslogik
- **Repositories** hanterar databasåtkomst via Entity Framework
- **DTOs** (Data Transfer Objects) används för kommunikation mellan lager
- **Dependency Injection** med interface-baserade tjänster genomgående

### API-endpoints

| Metod | Endpoint | Beskrivning |
|-------|----------|-------------|
| POST | `/api/auth/register` | Registrera ny användare |
| POST | `/api/auth/login` | Logga in |
| POST | `/api/auth/refresh` | Förnya access token |
| GET | `/api/boards` | Hämta användarens tavlor |
| POST | `/api/boards` | Skapa ny tavla |
| PUT | `/api/boards/{id}` | Uppdatera tavla |
| DELETE | `/api/boards/{id}` | Ta bort tavla |
| GET | `/api/boards/{id}/lists` | Hämta listor för en tavla |
| POST | `/api/boards/{id}/lists` | Skapa ny lista |
| PATCH | `/api/lists/{id}` | Uppdatera lista |
| DELETE | `/api/lists/{id}` | Ta bort lista |
| GET | `/api/lists/{id}/cards` | Hämta kort i en lista |
| POST | `/api/lists/{id}/cards` | Skapa nytt kort |
| PATCH | `/api/cards/{id}` | Uppdatera kort |
| PUT | `/api/cards/{id}/move` | Flytta kort till annan lista |
| DELETE | `/api/cards/{id}` | Ta bort kort |

## Teammedlemmar

| Namn | Ansvar |
|------|--------|
| Jack | Projektstruktur, modeller, databas, autentisering, frontend design |
| Sebastian | Board CRUD |
| Filip | List & Card CRUD |
| Joel | Användare, medlemmar, tester |

## Tekniska val

- **Blazor Server** valdes för att kunna skriva hela stacken i C# utan JavaScript
- **JWT med refresh tokens** ger säker autentisering utan att behöva sessions på servern
- **SQLite** valdes för enkelhet under utveckling – kräver ingen separat databasserver
- **ProtectedLocalStorage** används för säker tokenlagring i webbläsaren
- **HTML5 Drag and Drop API** för att flytta kort utan externa bibliotek
