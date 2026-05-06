using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SpinningWheel.Models;
using SpinningWheel.Services;

namespace SpinningWheel.Components.Organisms;

public partial class SpinningWheelView : IAsyncDisposable
{
    private const double Radius = 180;
    private const double HubRadius = 18;
    private const double ViewboxPadding = 28;

    /// <summary>Number of full rotations for the continuous-spin phase.</summary>
    private const int ContinuousSpinTurns = 90;

    /// <summary>Duration (ms) of the continuous-spin phase – keeps the wheel turning at a steady pace.</summary>
    private const int ContinuousSpinDurationMs = 60_000;

    [Inject] private ISpinService SpinService { get; set; } = default!;
    [Inject] private ISpinBroadcaster SpinBroadcaster { get; set; } = default!;
    [Inject] private IJSRuntime Js { get; set; } = default!;

    [Parameter, EditorRequired] public IReadOnlyList<WheelEntry> Entries { get; set; } = [];
    [Parameter] public EventCallback<WheelEntry> OnWinner { get; set; }

    private double _currentRotationDeg;
    private int _pendingWinnerIndex = -1;
    private int _currentDurationMs;
    private SpinState _state = SpinState.Idle;

    private IJSObjectReference? _jsModule;
    private ElementReference _rotorRef;

    private bool CanStartSpin => Entries.Count >= 2 && _state == SpinState.Idle;
    private bool CanStopSpin => _state == SpinState.Spinning;
    private bool IsClickable => CanStartSpin || CanStopSpin;

    private string ViewBox
    {
        get
        {
            var size = (Radius + ViewboxPadding) * 2;
            return string.Create(CultureInfo.InvariantCulture,
                $"{-Radius - ViewboxPadding} {-Radius - ViewboxPadding} {size} {size}");
        }
    }

    private string RotorStyle
    {
        get
        {
            var duration = _state switch
            {
                SpinState.Spinning => ContinuousSpinDurationMs,
                SpinState.Decelerating => _currentDurationMs,
                _ => 0,
            };

            var easing = _state switch
            {
                SpinState.Spinning => "linear",
                _ => "var(--easing-spin)",
            };

            return string.Create(CultureInfo.InvariantCulture,
                $"transform: rotate({_currentRotationDeg:F3}deg); transition: transform {duration}ms {easing};");
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _jsModule = await Js.InvokeAsync<IJSObjectReference>("import", "./js/wheelInterop.js");
        }
    }

    private async Task HandleClickAsync()
    {
        if (CanStartSpin)
        {
            StartSpin();
        }
        else if (CanStopSpin)
        {
            await HandleStopAsync();
        }
    }

    private void StartSpin()
    {
        _currentRotationDeg += ContinuousSpinTurns * 360.0;
        _state = SpinState.Spinning;
        SpinBroadcaster.NotifySpinStarted();
        StateHasChanged();
    }

    private async Task HandleStopAsync()
    {
        if (_jsModule is null)
        {
            return;
        }

        // 1. Read the current visual rotation from the DOM (mid-transition).
        var visualDeg = await _jsModule.InvokeAsync<double>("getCurrentRotation", _rotorRef);

        // 2. Plan the deceleration spin from the current visual position.
        //    Pass the current angular velocity so the deceleration starts seamlessly.
        var angularVelocity = ContinuousSpinTurns * 360.0 / ContinuousSpinDurationMs;
        var plan = SpinService.PlanStop(Entries.Count, visualDeg, angularVelocity);
        _pendingWinnerIndex = plan.WinnerIndex;
        _currentDurationMs = plan.DurationMs;

        // 3. Snap the DOM to the current visual position (cancel the running transition).
        await _jsModule.InvokeVoidAsync("snapToRotation", _rotorRef, visualDeg);

        // 4. Now set state to Decelerating and apply the new target; Blazor will render
        //    a deceleration transition from visualDeg → plan.TargetRotationDeg.
        _currentRotationDeg = plan.TargetRotationDeg;
        _state = SpinState.Decelerating;
        StateHasChanged();

        // 5. Wait for the deceleration animation to complete, then reveal the winner.
        await Task.Delay(plan.DurationMs + 60);
        await FinishSpinAsync();
    }

    private async Task FinishSpinAsync()
    {
        if (_state != SpinState.Decelerating)
        {
            return;
        }

        _state = SpinState.Idle;
        var winnerIndex = _pendingWinnerIndex;
        _pendingWinnerIndex = -1;
        StateHasChanged();

        if (winnerIndex >= 0 && winnerIndex < Entries.Count)
        {
            var winner = Entries[winnerIndex];
            SpinBroadcaster.NotifyWinnerRevealed(winner);
            await OnWinner.InvokeAsync(winner);
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_jsModule is not null)
        {
            await _jsModule.DisposeAsync();
        }
    }

    private enum SpinState
    {
        Idle,
        Spinning,
        Decelerating,
    }
}
