namespace PiControlPanel.Infrastructure.OnDemand.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using NLog;
    using PiControlPanel.Domain.Contracts.Constants;
    using PiControlPanel.Domain.Contracts.Infrastructure.OnDemand;
    using PiControlPanel.Domain.Contracts.Util;
    using PiControlPanel.Domain.Models.Hardware.Network;

    public class NetworkService : BaseService<Network>, INetworkService
    {
        private readonly ISubject<IList<NetworkInterfaceStatus>> networkInterfacesStatusSubject;

        public NetworkService(ISubject<IList<NetworkInterfaceStatus>> networkInterfacesStatusSubject,
            ILogger logger)
            : base(logger)
        {
            this.networkInterfacesStatusSubject = networkInterfacesStatusSubject;
        }

        public async Task<IList<NetworkInterfaceStatus>> GetNetworkInterfacesStatusAsync(IList<string> networkInterfaceNames, int samplingInterval)
        {
            logger.Info("Infra layer -> NetworkService -> GetNetworkInterfacesStatusAsync");
            return await this.GetNetworkInterfacesStatusFromStatsAsync(networkInterfaceNames, samplingInterval);
        }

        public IObservable<NetworkInterfaceStatus> GetNetworkInterfaceStatusObservable(string networkInterfaceName)
        {
            logger.Info("Infra layer -> NetworkService -> GetNetworkInterfaceStatusObservable");
            return this.networkInterfacesStatusSubject
                .Select(l => l.FirstOrDefault(i => i.NetworkInterfaceName == networkInterfaceName))
                .AsObservable();
        }

        public void PublishNetworkInterfacesStatus(IList<NetworkInterfaceStatus> networkInterfacesStatus)
        {
            logger.Info("Infra layer -> NetworkService -> PublishNetworkInterfacesStatus");
            this.networkInterfacesStatusSubject.OnNext(networkInterfacesStatus);
        }

        protected override Network GetModel()
        {
            var model = new Network()
            {
                NetworkInterfaces = new List<NetworkInterface>()
            };

            var result = BashCommands.Ifconfig.Bash();
            logger.Debug($"Result of '{BashCommands.Ifconfig}' command: '{result}'");

            var regex = new Regex(@"(?<name>\S+):\sflags=\d+<\S*RUNNING\S*>\s+mtu\s\d+\r?\n\s+inet\s(?<ip>\S+)\s+netmask\s(?<mask>\S+)\s+broadcast\s(?<gateway>\S+)");
            var matches = regex.Matches(result);
            foreach (Match match in matches)
            {
                GroupCollection groups = match.Groups;
                model.NetworkInterfaces.Add(
                new NetworkInterface()
                {
                    Name = groups["name"].Value,
                    IpAddress = groups["ip"].Value,
                    SubnetMask = groups["mask"].Value,
                    DefaultGateway = groups["gateway"].Value
                });
            }

            return model;
        }

        private async Task<IList<NetworkInterfaceStatus>> GetNetworkInterfacesStatusFromStatsAsync(IList<string> networkInterfaceNames, int samplingInterval)
        {
            //var result = BashCommands.CatCpuFreqStats.Bash();
            //logger.Debug($"Result of '{BashCommands.CatCpuFreqStats}' command: '{result}'");
            //string[] lines = result.Split(new[] { Environment.NewLine },
            //    StringSplitOptions.RemoveEmptyEntries);

            //var frequencyStats = new Dictionary<int, long>();
            //foreach (var line in lines)
            //{
            //    var state = line.Split(' ');
            //    if (int.TryParse(state[0], out var frequency) && long.TryParse(state[1], out var time))
            //    {
            //        frequencyStats.Add(frequency, time);
            //    }
            //    else
            //    {
            //        logger.Warn($"Could not parse frequency stats: '{line}'");
            //    }
            //}

            await Task.Delay(samplingInterval);

            //result = BashCommands.CatCpuFreqStats.Bash();
            //logger.Debug($"Result of '{BashCommands.CatCpuFreqStats}' command: '{result}'");
            //lines = result.Split(new[] { Environment.NewLine },
            //    StringSplitOptions.RemoveEmptyEntries);

            //foreach (var line in lines)
            //{
            //    var state = line.Split(' ');
            //    if (int.TryParse(state[0], out var frequency) && long.TryParse(state[1], out var time))
            //    {
            //        var oldTime = frequencyStats.ContainsKey(frequency) ? frequencyStats[frequency] : 0;
            //        frequencyStats[frequency] = time - oldTime;
            //    }
            //    else
            //    {
            //        logger.Warn($"Could not parse frequency stats: '{line}'");
            //        if (frequencyStats.ContainsKey(frequency))
            //        {
            //            frequencyStats.Remove(frequency);
            //        }
            //    }
            //}

            //if (frequencyStats.Any())
            //{
            //    var totalTime = frequencyStats.Values.Sum();
            //    var weightedAverage = frequencyStats.Select(f => f.Key * f.Value).Sum() / totalTime;
            //    return new CpuFrequency()
            //    {
            //        Frequency = Convert.ToInt32(weightedAverage / 1000),
            //        DateTime = DateTime.Now
            //    };
            //}
            //logger.Warn($"Could get cpu frequency stats");
            //return null;

            IList<NetworkInterfaceStatus> list = new List<NetworkInterfaceStatus>();

            foreach (var networkInterfaceName in networkInterfaceNames)
            {
                list.Add(new NetworkInterfaceStatus()
                {
                    NetworkInterfaceName = networkInterfaceName,
                    DateTime = DateTime.Now,
                    TotalReceived = networkInterfaceName.Equals("eth0") ? 888 : 444,
                    TotalSent = networkInterfaceName.Equals("eth0") ? 222 : 111,
                    ReceiveSpeed = networkInterfaceName.Equals("eth0") ? 8 : 4,
                    SendSpeed = networkInterfaceName.Equals("eth0") ? 2 : 1
                });
            }

            return list;
        }
    }
}
