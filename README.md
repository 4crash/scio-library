# Scio Blazor Project - Running Frontend & Backend

## Architecture

```
Frontend (Blazor WebAssembly)  ←→  Backend (ASP.NET Core API)
- Scio (port 5173/7207)              - Scio.API (port 7095)
```

## Running the Application

### Option 1: Run Both in Terminal Windows

**Terminal 1 - Backend API:**
```bash
cd Scio.API
dotnet watch run
```
Backend runs on: `https://localhost:7095`

**Terminal 2 - Frontend (Blazor WASM):**
```bash
cd Scio
dotnet watch run
```
Frontend runs on: `https://localhost:7207` or `http://localhost:5173`

### Option 2: Run from Solution

```bash
dotnet watch run --project Scio
dotnet watch run --project Scio.API
```

## Project Structure

```
blazor/
├── Scio/                          # Blazor WebAssembly Frontend
│   ├── Pages/
│   │   ├── beer.razor            # UI Component (runs in browser)
│   │   └── test.razor
│   ├── Services/
│   │   ├── BeerApiService.cs     # HTTP calls to backend API
│   │   ├── TestApiService.cs
│   │   └── TestService.cs        # Local service
│   └── Program.cs                # Frontend configuration
│
├── Scio.API/                      # ASP.NET Core Backend API
│   ├── Controllers/
│   │   └── BeerController.cs      # API endpoints
│   ├── Services/
│   │   └── BeerService.cs        # Business logic (runs on SERVER)
│   ├── Models/
│   │   └── Brewery.cs            # Shared data model
│   └── Program.cs                # Backend configuration
│
└── Scio.sln                       # Solution file
```

## Code Execution Flow

### When you click "+50" button in browser:

1. **Frontend (Browser):** beer.razor calls `BeerApiService.UpdateBreweryStockAsync()`
2. **HTTP Request:** Blazor sends POST to `https://localhost:7095/api/beer/stock/1`
3. **Backend (Server):** BeerController receives request
4. **Business Logic (Server):** `BeerService.UpdateBreweryStockAsync()` runs on server
5. **Response:** Server returns HTTP 204 No Content
6. **Frontend:** Refreshes data by calling GET `/api/beer`
7. **UI Updates:** New stock value displays in browser

## Key Differences

| Location | Code | Runs Where | Notes |
|----------|------|-----------|-------|
| BeerService.cs | Business logic | **SERVER** | Core logic, data persistence |
| BeerApiService.cs | HTTP calls | **BROWSER** | Calls the server API |
| beer.razor | UI Component | **BROWSER** | User interaction |

## Testing

Navigate to: `https://localhost:7207/beer` (or the frontend port)

Click buttons to update brewery stock. Changes persist on the server!

## Common Issues

- **Connection refused:** Make sure API is running on port 7095
- **CORS errors:** Check Program.cs in Scio.API for CORS configuration
- **Models not matching:** Keep Brewery.cs the same in both projects
