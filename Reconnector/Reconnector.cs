using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Reconnector
{
    public class Reconnector
    {
        private readonly ILogger<Reconnector> _logger;
        private readonly PiaCtl _piaCtl;

        private string[] _lastRegions;

        public Reconnector(ILogger<Reconnector> logger, PiaCtl piaCtl)
        {
            _logger = logger;
            _piaCtl = piaCtl;
        }

        public async Task Reconnect()
        {
            ReadLastRegions();
            _logger.LogInformation("Reconnector > Reconnect: last used regions \'{LastRegions}\'", string.Join(",", _lastRegions));

            var currentRegion = await _piaCtl.GetPiaRegion();
            _logger.LogInformation("Reconnector > Reconnect: received current region \'{CurrentRegion}\'", currentRegion);
            _lastRegions = _lastRegions.Append(currentRegion).Where(r => r is { Length: > 0 }).Distinct().ToArray();

            var currentIp = await _piaCtl.GetCurrentIp();
            _logger.LogInformation("Reconnector > Reconnect: received current ip \'{CurrentIp}\'", currentIp);

            var possibleRegions = _piaCtl.GetValidRegions(_lastRegions);
            _logger.LogInformation("Reconnector > Reconnect: evaluated possible region list: \'{PossibleRegions}\'", string.Join(",", possibleRegions));

            var index = new Random().Next(possibleRegions.Length);
            var newRegion = possibleRegions[index];
            _logger.LogInformation("Reconnector > Reconnect: randomly selected new region: \'{NewRegion}\'", newRegion);

            await _piaCtl.SetNewRegion(newRegion);
            _lastRegions = _lastRegions.Append(newRegion).TakeLast(5).ToArray();
            PersistLastRegions();

            await _piaCtl.Reconnect();

            var newIp = await _piaCtl.GetCurrentIp();
            _logger.LogInformation("Reconnector > Reconnect: received new ip \'{NewIp}\'", newIp);
        }

        private void ReadLastRegions()
        {
            _lastRegions = File.Exists(@".\lastRegions.txt")
                ? File.ReadAllText(@".\lastRegions.txt").Split(',').Where(r => r is { Length: > 0 }).ToArray()
                : Array.Empty<string>();
        }

        private void PersistLastRegions()
        {
            if (_lastRegions.Length <= 0) return;

            var commaSeparated = string.Join(',', _lastRegions.Where(r => r is { Length: > 0 }).ToArray());
            File.WriteAllText(@".\lastRegions.txt", commaSeparated);
        }
    }
}