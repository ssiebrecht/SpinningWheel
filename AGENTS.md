# SpinningWheel – Agent Instructions

Blazor WebAssembly app (net10.0) that implements a spinning wheel for randomly picking a winner from a name list. Entries are persisted in browser `localStorage`.

## Build & Run

```bash
dotnet run        # start the dev server (hot-reload enabled)
dotnet build      # compile only
```

No test project exists yet. Confirm with `dotnet build` that the solution compiles after any change.

## Architecture

Follows **Atomic Design** with a strict layer hierarchy:

```
Atoms → Molecules → Organisms → Templates → Pages
```

- **Atoms** (`Components/Atoms/`) – primitive UI elements (button, input, wheel segment, pointer, …)
- **Molecules** (`Components/Molecules/`) – small compositions of atoms (NameInputForm, NameListItem, WinnerBadge)
- **Organisms** (`Components/Organisms/`) – full feature sections (SpinningWheelView, NameList, WinnerOverlay)
- **Templates** (`Components/Templates/`) – page layouts using `RenderFragment` slots (Header, Wheel, Sidebar)
- **Pages** (`Components/Pages/`) – routed pages; `Home` is the single page

## Component Conventions

Every component is a **partial class split across three files**:

| File | Purpose |
|------|---------|
| `Foo.razor` | Markup; declares `@namespace` matching folder |
| `Foo.razor.cs` | `partial class Foo` – parameters, logic, lifecycle |
| `Foo.razor.css` | Scoped CSS – BEM naming (`.foo__element--modifier`) |

Key rules:
- `[Parameter]` for all inputs; add `[EditorRequired]` for required parameters.
- `[Inject]` property injection (never constructor injection in components), initialised with `default!`.
- All async event handlers are named `HandleXxxAsync`, return `Task`, and guard with a `Can…` predicate.
- Use `@key` in `@for`/`@foreach` loops (e.g. `@key="entry.Id"`).
- All numeric-to-string formatting uses `string.Create(CultureInfo.InvariantCulture, …)`.
- `StateHasChanged()` is called explicitly after mutating private state outside of Blazor's event pipeline.
- Components that subscribe to events (`Store.OnChanged`) **must** implement `IDisposable` and unsubscribe in `Dispose()`.

## Services

Registered as **Scoped** in [`Program.cs`](Program.cs). Always inject by interface.

| Interface | Implementation | Purpose |
|-----------|----------------|---------|
| `ILocalStorageService` | `LocalStorageService` | JS interop wrapping `localStorage` (JSON serialized) |
| `INameStore` | `NameStore` | Reactive in-memory store + localStorage persistence; exposes `event Action? OnChanged` |
| `ISpinService` | `SpinService` | Pure stateless spin math (`Plan(segmentCount, currentRotation)` → `SpinPlan`) |

When adding a new service: create the interface first, then the implementation, then register in `Program.cs`.

## Models

Immutable `record` types in `Models/`:
- `WheelEntry(Guid Id, string Name, string ColorToken)` – one wheel entry; `ColorToken` is a CSS custom property name like `--color-segment-1`.
- `SpinPlan(int WinnerIndex, double TargetRotationDeg, int DurationMs)` – output of `ISpinService.Plan`.

## CSS Design System

All design tokens are CSS custom properties defined in [`wwwroot/css/tokens.css`](wwwroot/css/tokens.css):
- Colors: `--color-bg`, `--color-surface`, `--color-primary`, `--color-accent`, …
- Wheel palette: `--color-segment-1` … `--color-segment-8` (8 colours, cycled by `NameStore`)
- Spacing: `--space-1` (0.25 rem) … `--space-8` (4 rem)
- Radii: `--radius-sm`, `--radius-md`, `--radius-lg`, `--radius-full`
- Motion: `--duration-fast`, `--duration-standard`, `--easing-standard`, `--easing-spin`
- Shadows: `--shadow-sm`, `--shadow-md`, `--shadow-lg`, `--shadow-glow`

**Never hardcode colours, spacing, or radii.** Always use a token from `tokens.css`.

Global namespaces (all component namespaces, models, and services) are pre-imported in [`_Imports.razor`](_Imports.razor).

## SVG / Wheel Internals

The wheel SVG uses a **clockwise-from-top (12-o'clock)** coordinate system. The `Polar()` helper in `WheelSegment` converts degrees to Cartesian. `transitionend` is unreliable on SVG `<g>` in Blazor WebAssembly – spin completion is determined via `Task.Delay(duration + 60ms)` instead.
