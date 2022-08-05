using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Reconnector.Options;

namespace Reconnector
{
    public class PiaCtl
    {
        private readonly ILogger<PiaCtl> _logger;
        private readonly IOptions<PiaOptions> _piaOptions;

        public PiaCtl(IOptions<PiaOptions> piaOptions, ILogger<PiaCtl> logger)
        {
            _piaOptions = piaOptions;
            _logger = logger;
        }

        public string[] GetValidRegions(string[] excludedRegions = null)
        {
            var servers = _piaOptions.Value.ValidServers;
            if (excludedRegions is { Length: > 0 })
                servers = servers.Where(s => !excludedRegions.Contains(s)).ToArray();
            return servers;
        }

        public async Task<string> GetCurrentIp()
        {
            return await ExecPiaCommand("get vpnip");
        }

        public async Task<string> GetPiaRegion()
        {
            return await ExecPiaCommand("get region");
        }

        public async Task SetNewRegion(string region)
        {
            await ExecPiaCommand($"set region {region}");
        }

        public async Task Reconnect()
        {
            const int maxSecondsToWait = 30;
            var currentIp = await GetCurrentIp();

            await ExecPiaCommand("disconnect");
            Thread.Sleep(1 * 1000);
            await ExecPiaCommand("connect");

            for (var i = 0; i < maxSecondsToWait; i++)
            {
                Thread.Sleep(1000);
                var newIp = await GetCurrentIp();
                if (newIp != "Unknown" && newIp != currentIp)
                    break;
            }
        }

        private async Task<string> ExecPiaCommand(string arguments)
        {
            string data = null;
            try
            {
                using var p = new Process();
                // set start info
                p.StartInfo = new ProcessStartInfo(_piaOptions.Value.PiaCtlPath + @"\piactl.exe")
                {
                    Arguments = arguments,
                    WorkingDirectory = _piaOptions.Value.PiaCtlPath,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                p.Start();
                while (!p.StandardOutput.EndOfStream) data += await p.StandardOutput.ReadLineAsync();

                // start process
                await p.WaitForExitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PiaCtl > ExecPiaCommand: Encountered error!");
            }

            return data;
        }
    }
}