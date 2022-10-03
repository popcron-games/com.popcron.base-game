#nullable enable
using Cysharp.Threading.Tasks;
using Popcron.CommandRunner;
using System.Threading.Tasks;

namespace BaseGame.Commands
{
    public readonly struct DumpLog : IAsyncCommand
    {
        string IBaseCommand.Path => "dump log";

        Result? ICommand.Run(Context context)
        {
            Log.DumpAllLogs();
            return null;
        }

        async Task<Result?> IAsyncCommand.RunAsync(Context context)
        {
            await Log.DumpAllLogs();
            return null;
        }
    }
}
