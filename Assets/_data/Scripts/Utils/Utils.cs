using System;
using System.Threading;
using System.Threading.Tasks;

public static class Utils
{
    public static async Task AwaitTime(int seconds, CancellationToken token)
    {
        try
        {
            await Task.Delay(seconds * 1000, token);
        }
        catch (OperationCanceledException)
        {
            // object bị hủy → bỏ qua
        }
    }
}
