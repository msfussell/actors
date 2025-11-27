
using Dapr.Actors;
using Dapr.Actors.Client;
using Light.Interfaces;

class Program
{
    static async Task Main()
    {
        var actorType = "LightActor";
        var actorId = new ActorId("light-42");
        var proxy = ActorProxy.Create<ILight>(actorId, actorType);
        Console.WriteLine($"ActorProxy created, proxy: {proxy}, actorId: {actorId}");


        Console.WriteLine("Increasing brightness...");
        var v1 = await proxy.IncreaseBrightnessAsync(1);
        Console.WriteLine($"After IncreaseBrightnessAsync(1): {v1}");
        var v2 = await proxy.IncreaseBrightnessAsync(10);
        Console.WriteLine($"After IncreaseBrightnessAsync(10): {v2}");
        var v3 = await proxy.GetBrightnessAsync();
        Console.WriteLine($"After GetBrightnessAsync(): {v3}");

        Console.WriteLine("Decreasing brightness...");
        var v4 = await proxy.DecreaseBrightnessAsync(1);
        Console.WriteLine($"After DecreaseBrightnessAsync(1): {v4}");
        var v5 = await proxy.DecreaseBrightnessAsync(1);
        Console.WriteLine($"After DecreaseBrightnessAsync(1): {v5}");
        var v6 = await proxy.GetBrightnessAsync();
        Console.WriteLine($"After GetBrightnessAsync(): {v6}");

        Console.WriteLine("Reset the brightness to 0...");
        await proxy.ResetAsync();
        Console.WriteLine($"After reset: {await proxy.GetBrightnessAsync()}");

        //Concurrent test
        Console.WriteLine("Concurrent brightness test. " +
            "Everyone is increasing the brightness at the same time, but the brightness can only beupdated sequentially. Only one switch!");
        var tasks = new Task<int>[10];
        for (int i = 0; i < 10; i++)
        {
            tasks[i] = proxy.IncreaseBrightnessAsync(1);
        }
        await Task.WhenAll(tasks);
        var initialBrightness = await proxy.GetBrightnessAsync();
        Console.WriteLine($"After 10 concurrent calls: {initialBrightness}");


        //Let's slowly dim the lights and monitor AutoDim reminder
        Console.WriteLine("\nMonitoring AutoDim reminder (brightness decreases every 10 seconds)...");
        Console.WriteLine("Press Ctrl+C to stop monitoring.\n");

        var cancellationTokenSource = new CancellationTokenSource();
        Console.CancelKeyPress += (sender, e) =>
        {
            e.Cancel = true;
            cancellationTokenSource.Cancel();
            Console.WriteLine("\nStopping monitor...");
        };

        var lastBrightness = initialBrightness;
        var checkInterval = TimeSpan.FromSeconds(2); // Check every 2 seconds

        try
        {
            while (!cancellationTokenSource.Token.IsCancellationRequested)
            {
                var currentBrightness = await proxy.GetBrightnessAsync();
                var timestamp = DateTime.Now.ToString("HH:mm:ss");

                if (currentBrightness != lastBrightness)
                {
                    Console.WriteLine($"[{timestamp}] Brightness changed: {lastBrightness} → {currentBrightness}");
                    lastBrightness = currentBrightness;

                    if (currentBrightness == 0)
                    {
                        Console.WriteLine($"[{timestamp}] Light has been fully dimmed!");
                        break;
                    }
                }
                else
                {
                    Console.WriteLine($"[{timestamp}] Brightness: {currentBrightness}");
                }

                await Task.Delay(checkInterval, cancellationTokenSource.Token);
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("\nMonitor stopped by user.");
        }

        var finalBrightness = await proxy.GetBrightnessAsync();
        Console.WriteLine($"\nLights Out: {finalBrightness}");
    }
}
