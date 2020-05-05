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
    using PiControlPanel.Domain.Models.Hardware.Cpu;

    public class CpuService : BaseService<Cpu>, ICpuService
    {
        private readonly ISubject<CpuFrequency> cpuFrequencySubject;
        private readonly ISubject<CpuTemperature> cpuTemperatureSubject;
        private readonly ISubject<CpuLoadStatus> cpuLoadStatusSubject;

        public CpuService(ISubject<CpuFrequency> cpuFrequencySubject,
            ISubject<CpuTemperature> cpuTemperatureSubject,
            ISubject<CpuLoadStatus> cpuLoadStatusSubject,
            ILogger logger)
            : base(logger)
        {
            this.cpuFrequencySubject = cpuFrequencySubject;
            this.cpuTemperatureSubject = cpuTemperatureSubject;
            this.cpuLoadStatusSubject = cpuLoadStatusSubject;
        }

        public Task<CpuLoadStatus> GetLoadStatusAsync(int cores)
        {
            logger.Trace("Infra layer -> CpuService -> GetLoadStatusAsync");

            var result = BashCommands.Top.Bash();
            logger.Debug($"Result of '{BashCommands.Top}' command: '{result}'");
            string[] lines = result.Split(new[] { Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries);

            var averageLoadInfo = lines.First(l => l.Contains("load average:"));
            var averageLoadRegex = new Regex(@"load average: (?<lastMinute>\d+\.\d{2}), (?<last5Minutes>\d+\.\d{2}), (?<last15Minutes>\d+\.\d{2})$");
            var averageLoadGroups = averageLoadRegex.Match(averageLoadInfo).Groups;

            var realTimeLoadInfo = lines.First(l => l.StartsWith("%Cpu(s):"));
            var realTimeLoadRegex = new Regex(@"^%Cpu\(s\):\s*(?<user>\d{1,3}\.\d{1}) us,\s*(?<kernel>\d{1,3}\.\d{1}) sy, .*$");
            var realTimeLoadGroups = realTimeLoadRegex.Match(realTimeLoadInfo).Groups;

            var processLines = lines.SkipWhile(l => !l.Contains("PID")).ToList();
            processLines.RemoveAt(0);
            processLines = processLines.Take(10).ToList();
            var dateTime = DateTime.Now;

            return Task.FromResult(new CpuLoadStatus()
            {
                LastMinuteAverage = (100 * double.Parse(averageLoadGroups["lastMinute"].Value)) / cores,
                Last5MinutesAverage = (100 * double.Parse(averageLoadGroups["last5Minutes"].Value)) / cores,
                Last15MinutesAverage = (100 * double.Parse(averageLoadGroups["last15Minutes"].Value)) / cores,
                UserRealTime = double.Parse(realTimeLoadGroups["user"].Value),
                KernelRealTime = double.Parse(realTimeLoadGroups["kernel"].Value),
                Processes = this.GetProcesses(processLines, dateTime),
                DateTime = dateTime
            });
        }

        public IObservable<CpuLoadStatus> GetLoadStatusObservable()
        {
            logger.Trace("Infra layer -> CpuService -> GetLoadStatusObservable");
            return this.cpuLoadStatusSubject.AsObservable();
        }

        public void PublishLoadStatus(CpuLoadStatus loadStatus)
        {
            logger.Trace("Infra layer -> CpuService -> PublishLoadStatus");
            this.cpuLoadStatusSubject.OnNext(loadStatus);
        }

        public Task<CpuTemperature> GetTemperatureAsync()
        {
            logger.Trace("Infra layer -> CpuService -> GetTemperatureAsync");

            var result = BashCommands.MeasureTemp.Bash();
            logger.Debug($"Result of '{BashCommands.MeasureTemp}' command: '{result}'");

            var temperatureResult = result.Substring(result.IndexOf('=') + 1, result.IndexOf("'") - (result.IndexOf('=') + 1));
            logger.Debug($"Temperature substring: '{temperatureResult}'");

            if (double.TryParse(temperatureResult, out var temperature))
            {
                return Task.FromResult(new CpuTemperature()
                {
                    Temperature = temperature,
                    DateTime = DateTime.Now
                });
            }
            logger.Warn($"Could not parse temperature: '{temperatureResult}'");
            return null;
        }

        public IObservable<CpuTemperature> GetTemperatureObservable()
        {
            logger.Trace("Infra layer -> CpuService -> GetTemperatureObservable");
            return this.cpuTemperatureSubject.AsObservable();
        }

        public void PublishTemperature(CpuTemperature temperature)
        {
            logger.Trace("Infra layer -> CpuService -> PublishTemperature");
            this.cpuTemperatureSubject.OnNext(temperature);
        }

        public async Task<CpuFrequency> GetFrequencyAsync(int samplingInterval)
        {
            logger.Trace("Infra layer -> CpuService -> GetFrequencyAsync");

            var result = BashCommands.CatCpuFreqStats.Bash();
            logger.Debug($"Result of '{BashCommands.CatCpuFreqStats}' command: '{result}'");
            string[] lines = result.Split(new[] { Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries);

            var frequencyStats = new Dictionary<int, long>();
            foreach (var line in lines)
            {
                var state = line.Split(' ');
                if (int.TryParse(state[0], out var frequency) && long.TryParse(state[1], out var time))
                {
                    frequencyStats.Add(frequency, time);
                }
                else
                {
                    logger.Warn($"Could not parse frequency stats: '{line}'");
                }
            }

            await Task.Delay(samplingInterval);

            result = BashCommands.CatCpuFreqStats.Bash();
            logger.Debug($"Result of '{BashCommands.CatCpuFreqStats}' command: '{result}'");
            lines = result.Split(new[] { Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                var state = line.Split(' ');
                if (int.TryParse(state[0], out var frequency) && long.TryParse(state[1], out var time))
                {
                    var oldTime = frequencyStats.ContainsKey(frequency) ? frequencyStats[frequency] : 0;
                    frequencyStats[frequency] = time - oldTime;
                }
                else
                {
                    logger.Warn($"Could not parse frequency stats: '{line}'");
                    if (frequencyStats.ContainsKey(frequency))
                    {
                        frequencyStats.Remove(frequency);
                    }
                }
            }

            if (frequencyStats.Any())
            {
                var totalTime = frequencyStats.Values.Sum();
                var weightedAverage = frequencyStats.Select(f => f.Key * f.Value).Sum() / totalTime;
                return new CpuFrequency()
                {
                    Frequency = Convert.ToInt32(weightedAverage / 1000),
                    DateTime = DateTime.Now
                };
            }
            logger.Warn($"Could not get cpu frequency stats");
            return null;
        }

        public IObservable<CpuFrequency> GetFrequencyObservable()
        {
            logger.Trace("Infra layer -> CpuService -> GetFrequencyObservable");
            return this.cpuFrequencySubject.AsObservable();
        }

        public void PublishFrequency(CpuFrequency frequency)
        {
            logger.Trace("Infra layer -> CpuService -> PublishFrequency");
            this.cpuFrequencySubject.OnNext(frequency);
        }

        protected override Cpu GetModel()
        {
            var result = BashCommands.CatProcCpuInfo.Bash();
            logger.Debug($"Result of '{BashCommands.CatProcCpuInfo}' command: '{result}'");
            string[] lines = result.Split(new[] { Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries);

            var cores = lines.Count(line => line.StartsWith("processor"));
            logger.Debug($"Number of cores: '{cores}'");
            var model = lines.Last(line => line.StartsWith("model name"))
                .Split(':')[1].Trim();
            logger.Debug($"Cpu model: '{model}'");

            result = BashCommands.CatBootConfig.Bash();
            logger.Debug($"Result of '{BashCommands.CatBootConfig}' command: '{result}'");
            lines = result.Split(new[] { Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries);
            var frequencyLine = lines.FirstOrDefault(line => line.Contains("arm_freq="));
            var frequencyLineRegex = new Regex(@"^(?<commented>#?)\s*arm_freq=(?<frequency>\d+)$");
            logger.Debug($"Frequency line in config file: '{frequencyLine}'");
            var frequencyLineGroups = frequencyLineRegex.Match(frequencyLine).Groups;
            var frequency = !string.IsNullOrEmpty(frequencyLineGroups["commented"].Value) ?
                1500 : int.Parse(frequencyLineGroups["frequency"].Value);

            return new Cpu()
            {
                Cores = cores,
                Model = model,
                MaximumFrequency = frequency
            };
        }

        private IList<CpuProcess> GetProcesses(IList<string> processLines, DateTime dateTime)
        {
            var processes = new List<CpuProcess>();
            var regex = new Regex(@"^\s*(?<pid>\S*)\s*(?<user>\S*)\s*(?<pr>\S*)\s*(?<ni>\S*)\s*(?<virt>\d*)\s*(?<res>\d*)\s*(?<shr>\d*)\s*(?<s>\w)\s*(?<cpu>\d+\.\d)\s*(?<mem>\d+\.\d)\s*(?<time>\S*)\s*(?<command>.*)$");
            
            foreach (var line in processLines)
            {
                var groups = regex.Match(line).Groups;
                processes.Add(new CpuProcess()
                {
                    ProcessId = int.Parse(groups["pid"].Value),
                    User = groups["user"].Value,
                    Priority = groups["pr"].Value,
                    NiceValue = int.Parse(groups["ni"].Value),
                    TotalMemory = int.Parse(groups["virt"].Value),
                    Ram = int.Parse(groups["res"].Value),
                    SharedMemory = int.Parse(groups["shr"].Value),
                    State = groups["s"].Value,
                    CpuPercentage = double.Parse(groups["cpu"].Value),
                    RamPercentage = double.Parse(groups["mem"].Value),
                    TotalCpuTime = groups["time"].Value,
                    Command = groups["command"].Value,
                    DateTime = dateTime
                });
            }
            return processes;
        }
    }
}
