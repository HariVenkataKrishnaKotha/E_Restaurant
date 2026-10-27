# E_Restaurant

> **Archived course follow-along.** I built this in August 2023 while working through Bhrugen Patel's .NET 8 microservices course (the "Mango" project). It stops partway through the course: 5 of the 18 checklist items were finished. It is not original design work and is not maintained.
>
> It was replaced by **[DesiCorner](https://github.com/HariVenkataKrishnaKotha/DesiCorner)**, my later .NET 8 microservices e-commerce project.

## What was built

The solution `Erestaurant.sln` has three ASP.NET Core projects that run as separate processes:

| Project | Role | Local URL (`https` profile) |
|---|---|---|
| `Erestaurant.Services.CouponAPI` | REST API for coupon CRUD (EF Core + SQL Server). Seeds two coupons, `10OFF` and `15OFF`. | `https://localhost:7001/swagger` |
| `Erestaurant.Service.AuthAPI` | ASP.NET Core Identity. Handles register, login and role assignment, and issues JWTs. | `https://localhost:7002/swagger` |
| `Erestaurant.Web` | MVC front end. Has coupon list, create and delete pages, plus login, register and logout. Calls both APIs over HTTP with `IHttpClientFactory`. | `https://localhost:7038` |

**API routes**

- CouponAPI: `GET /api/CouponAPI`, `GET /api/CouponAPI/{id}`, `GET /api/CouponAPI/GetByCode/{code}`, `POST /api/CouponAPI`, `PUT /api/CouponAPI`, `DELETE /api/CouponAPI/{id}`
- AuthAPI: `POST /api/authAPI/register`, `POST /api/authAPI/login`, `POST /api/authAPI/AssignRole`

**Not built:** everything after step 5 of the original checklist. That includes the product, cart, order and email services, Azure Service Bus, the Ocelot gateway, Stripe, Docker/Kubernetes and Azure deployment. There are no tests and no CI.

## Stack

- .NET 8 (`net8.0`). .NET 8 LTS support ends 2026-11-10.
- ASP.NET Core Web API and MVC
- Entity Framework Core and ASP.NET Core Identity with SQL Server. These packages are pinned to **`8.0.0-preview.7`** builds.
- JWT issued by the AuthAPI and stored in a cookie by the Web app
- AutoMapper 12.0.1, Swashbuckle 6.5.0, Bootstrap/jQuery (vendored in `wwwroot/lib`)

## How to run

**Prerequisites:** .NET 8 SDK or later, and a local SQL Server instance reachable as `Server=localhost` with Windows (trusted) authentication. Each API creates its own database (`Erestaurant_Coupon`, `Erestaurant_Auth`) and applies EF Core migrations when it starts.

```bash
git clone https://github.com/HariVenkataKrishnaKotha/E_Restaurant.git
cd E_Restaurant
dotnet build Erestaurant.sln
```

The AuthAPI reads a JWT signing key from `ApiSettings:JwtOptions:Secret`. Do not use the value committed in `appsettings.json`; set your own with user-secrets:

```bash
dotnet user-secrets init --project Erestaurant.Service.AuthAPI
dotnet user-secrets set "ApiSettings:JwtOptions:Secret" "<a long random string>" --project Erestaurant.Service.AuthAPI
```

Then start each project in its own terminal:

```bash
dotnet run --project Erestaurant.Services.CouponAPI --launch-profile https
dotnet run --project Erestaurant.Service.AuthAPI    --launch-profile https
dotnet run --project Erestaurant.Web                --launch-profile https
```

The Web app finds the APIs through `ServiceUrls` in `Erestaurant.Web/appsettings.json`, which point to `https://localhost:7001` and `https://localhost:7002`.

**Verification (2026-10-27, fresh clone, .NET SDK 10.0.301):**

- `dotnet build Erestaurant.sln` passed with 0 errors.
- The build warnings were mostly nullable-reference warnings, plus NuGet audit warnings for `AutoMapper` 12.0.1 (high, GHSA-rvv3-g6hj-g44x) and `System.IdentityModel.Tokens.Jwt` 7.0.0-preview4 (moderate, GHSA-59j7-ghrg-fj52).
- Running the apps was *(not verified: requires a local SQL Server instance)*.

## Known issues

These were found during a 2026 review and left as-is because the repo is archived.

- The CouponAPI has no authentication or `[Authorize]`, so anyone can create, update or delete coupons.
- The Web app stores the JWT in a cookie with default options, which means it is not `HttpOnly` or `Secure`.
- A JWT signing key is committed in `Erestaurant.Service.AuthAPI/appsettings.json`.
- In `AuthService.cs`, an unknown username at login throws an exception instead of returning "incorrect username or password", because the password check runs before the null check.
- `CouponService.GetCouponByCodeAsync` in the Web app builds the wrong URL (`/api/CouponAPI/GetById<code>`), so it never succeeds.
- `BaseService` never attaches the bearer token to outgoing API calls.
- The API responses and the UI show exception messages to the user.
- The MVC POST actions have no anti-forgery validation.
- The framework packages are pre-release `8.0.0-preview.7` builds that were never updated to GA.

## License

MIT. See [LICENSE](LICENSE).

Course credit: the structure and much of the code follow Bhrugen Patel's .NET 8 microservices course.
