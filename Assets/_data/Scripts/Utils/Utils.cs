using System;
using System.Threading;
using System.Threading.Tasks;

public static class Utils
{
    public static async Task AwaitTime(float seconds, CancellationToken token)
    {
        try
        {
            // Dùng hàm FromSeconds tự động xử lý float
            await Task.Delay(TimeSpan.FromSeconds(seconds), token);
        }
        catch (OperationCanceledException)
        {
            // object bị hủy → bỏ qua
        }
    }
}
