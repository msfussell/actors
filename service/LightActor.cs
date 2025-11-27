using Dapr.Actors.Runtime;
using Light.Interfaces;

namespace Light.Service;

public class LightActor : Actor, ILight, IRemindable
{
    private readonly LightData _lightData;

    public LightActor(ActorHost host) : base(host)
    {
        _lightData = new LightData();
    }

    public async Task<int> IncreaseBrightnessAsync(int delta = 1)
    {
        var has = await StateManager.TryGetStateAsync<int>(_lightData.Brightness);
        //increase brightness and save to state store
        var next = (has.HasValue ? has.Value : 0) + delta;
        await StateManager.SetStateAsync(_lightData.Brightness, next);
        return next;
    }

    public async Task<int> DecreaseBrightnessAsync(int delta = 1)
    {
        var has = await StateManager.TryGetStateAsync<int>(_lightData.Brightness);
        //decrease brightness and save to state store
        var next = (has.HasValue ? has.Value : 0) - delta;
        await StateManager.SetStateAsync(_lightData.Brightness, next);
        return next;
    }

    public Task<int> GetBrightnessAsync() => StateManager.GetOrAddStateAsync(_lightData.Brightness, 0);

    public async Task ResetAsync() => await StateManager.SetStateAsync(_lightData.Brightness, 0);

    protected override async Task OnActivateAsync()
    {
        await RegisterReminderAsync(
            _lightData.AutoDimReminder,
            null,
            dueTime: TimeSpan.FromSeconds(1),
            //period: TimeSpan.FromMinutes(10));
            period: TimeSpan.FromSeconds(20));
    }

    public async Task ReceiveReminderAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period)
    {
        if (reminderName == _lightData.AutoDimReminder)
        {
            var current = await StateManager.GetOrAddStateAsync(_lightData.Brightness, 0);
            if (current > 0)
            {
                await StateManager.SetStateAsync(_lightData.Brightness, current - 1);
                Logger.LogInformation($"[{Id}] Auto-dimmed brightness to {current - 1}");
            }
        }
    }
}

