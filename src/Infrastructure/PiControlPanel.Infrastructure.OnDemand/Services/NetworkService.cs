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

    /// <inheritdoc/>
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
            this.logger.Debug("Infra layer -> NetworkService -> GetNetworkInterfacesStatusAsync");

            var result = BashCommands.CatProcNetDev.Bash();
            var now = DateTime.Now;
            this.logger.Trace($"Result of '{BashCommands.CatProcNetDev}' command: '{result}'");
            var lines = result
                .Split(
                    new[] { Environment.NewLine },
                    StringSplitOptions.RemoveEmptyEntries)
                .Select(l => l.TrimStart());

            var networkInterfacesStatuses = new Dictionary<string, NetworkInterfaceStatus>();
            foreach (var networkInterfaceName in networkInterfaceNames)
            {
                var networkInterfaceLine = lines
                    .SingleOrDefault(l => l.StartsWith($"{networkInterfaceName}:"));
                var networkInterfaceInfo = Regex.Split(networkInterfaceLine, @"\s+");

                long.TryParse(networkInterfaceInfo[1], out var received);
                long.TryParse(networkInterfaceInfo[9], out var sent);

                networkInterfacesStatuses.Add(
                    networkInterfaceName,
                    new NetworkInterfaceStatus()
                    {
                        NetworkInterfaceName = networkInterfaceName,
                        DateTime = now,
                        TotalReceived = received,
                        TotalSent = sent
                    });
            }

            await Task.Delay(samplingInterval);

            result = BashCommands.CatProcNetDev.Bash();
            now = DateTime.Now;
            this.logger.Trace($"Result of '{BashCommands.CatProcNetDev}' command: '{result}'");
            lines = result
                .Split(
                    new[] { Environment.NewLine },
                    StringSplitOptions.RemoveEmptyEntries)
                .Select(l => l.TrimStart());

            foreach (var networkInterfaceName in networkInterfaceNames)
            {
                var networkInterfaceLine = lines
                    .SingleOrDefault(l => l.StartsWith($"{networkInterfaceName}:"));
                var networkInterfaceInfo = Regex.Split(networkInterfaceLine, @"\s+");
                long.TryParse(networkInterfaceInfo[1], out var received);
                long.TryParse(networkInterfaceInfo[9], out var sent);

                var networkInterfacesStatus = networkInterfacesStatuses[networkInterfaceName];
                var elapsedSpan = new TimeSpan(now.Ticks - networkInterfacesStatus.DateTime.Ticks);

                networkInterfacesStatus.ReceiveSpeed =
                    (received - networkInterfacesStatus.TotalReceived) / elapsedSpan.TotalSeconds;
                networkInterfacesStatus.SendSpeed =
                    (sent - networkInterfacesStatus.TotalSent) / elapsedSpan.TotalSeconds;
                networkInterfacesStatus.TotalReceived = received;
                networkInterfacesStatus.TotalSent = sent;
                networkInterfacesStatus.DateTime = now;
            }

            return networkInterfacesStatuses.Values.ToList();
        }

        public IObservable<NetworkInterfaceStatus> GetNetworkInterfaceStatusObservable(string networkInterfaceName)
        {
            this.logger.Debug("Infra layer -> NetworkService -> GetNetworkInterfaceStatusObservable");
            return this.networkInterfacesStatusSubject
                .Select(l => l.FirstOrDefault(i => i.NetworkInterfaceName == networkInterfaceName))
                .AsObservable();
        }

        public void PublishNetworkInterfacesStatus(IList<NetworkInterfaceStatus> networkInterfacesStatus)
        {
            this.logger.Debug("Infra layer -> NetworkService -> PublishNetworkInterfacesStatus");
            this.networkInterfacesStatusSubject.OnNext(networkInterfacesStatus);
        }

        protected override Network GetModel()
        {
            var model = new Network()
            {
                NetworkInterfaces = new List<NetworkInterface>()
            };

            var result = BashCommands.Ifconfig.Bash();
            this.logger.Trace($"Result of '{BashCommands.Ifconfig}' command: '{result}'");

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
    }
}
