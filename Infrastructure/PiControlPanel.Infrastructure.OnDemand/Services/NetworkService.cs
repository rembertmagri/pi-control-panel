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
        public NetworkService(ILogger logger)
            : base(logger)
        {
        }

        protected override Network GetModel()
        {
            var model = new Network()
            {
                Interfaces = new List<Interface>()
            };

            var result = BashCommands.Ifconfig.Bash();
            logger.Debug($"Result of '{BashCommands.Ifconfig}' command: '{result}'");

            var regex = new Regex(@"(?<name>\S+):\sflags=\d+<\S*RUNNING\S*>\s+mtu\s\d+\r?\n\s+inet\s(?<ip>\S+)\s+netmask\s(?<mask>\S+)\s+broadcast\s(?<gateway>\S+)");
            var matches = regex.Matches(result);
            foreach (Match match in matches)
            {
                GroupCollection groups = match.Groups;
                model.Interfaces.Add(
                new Interface()
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
