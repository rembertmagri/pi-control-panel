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

    /// <inheritdoc/>
    public class CpuService : BaseService<Cpu>, ICpuService
    {
        private readonly ISubject<CpuFrequency> cpuFrequencySubject;
        private readonly ISubject<CpuSensorsStatus> cpuSensorsStatusSubject;
        private readonly ISubject<CpuLoadStatus> cpuLoadStatusSubject;

        /// <summary>
        /// Initializes a new instance of the <see cref="CpuService"/> class.
        /// </summary>
        /// <param name="cpuFrequencySubject">The CPU frequency subject.</param>
        /// <param name="cpuSensorsStatusSubject">The CPU sensors status subject.</param>
        /// <param name="cpuLoadStatusSubject">The CPU load status subject.</param>
        /// <param name="logger">The NLog logger instance.</param>
        public CpuService(
            ISubject<CpuFrequency> cpuFrequencySubject,
            ISubject<CpuSensorsStatus> cpuSensorsStatusSubject,
            ISubject<CpuLoadStatus> cpuLoadStatusSubject,
            ILogger logger)
            : base(logger)
        {
            this.cpuFrequencySubject = cpuFrequencySubject;
            this.cpuSensorsStatusSubject = cpuSensorsStatusSubject;
            this.cpuLoadStatusSubject = cpuLoadStatusSubject;
        }

        /// <inheritdoc/>
        public Task<CpuLoadStatus> GetLoadStatusAsync()
        {
            this.Logger.Debug("Infra layer -> CpuService -> GetLoadStatusAsync");

            var result = BashCommands.Top.Bash();
            this.Logger.Trace($"Result of '{BashCommands.Top}' command: '{result}'");
            string[] lines = result.Split(
                new[] { Environment.NewLine },
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
                LastMinuteAverage = double.Parse(averageLoadGroups["lastMinute"].Value),
                Last5MinutesAverage = double.Parse(averageLoadGroups["last5Minutes"].Value),
                Last15MinutesAverage = double.Parse(averageLoadGroups["last15Minutes"].Value),
                UserRealTime = double.Parse(realTimeLoadGroups["user"].Value),
                KernelRealTime = double.Parse(realTimeLoadGroups["kernel"].Value),
                Processes = this.GetProcesses(processLines, dateTime),
                DateTime = dateTime
            });
        }

        /// <inheritdoc/>
        public IObservable<CpuLoadStatus> GetLoadStatusObservable()
        {
            this.Logger.Debug("Infra layer -> CpuService -> GetLoadStatusObservable");
            return this.cpuLoadStatusSubject.AsObservable();
        }

        /// <inheritdoc/>
        public void PublishLoadStatus(CpuLoadStatus loadStatus)
        {
            this.Logger.Debug("Infra layer -> CpuService -> PublishLoadStatus");
            this.cpuLoadStatusSubject.OnNext(loadStatus);
        }

        /// <inheritdoc/>
        public Task<CpuSensorsStatus> GetSensorsStatusAsync()
        {
            this.Logger.Debug("Infra layer -> CpuService -> GetSensorsStatusAsync");

            var result = BashCommands.MeasureTemp.Bash();
            this.Logger.Trace($"Result of '{BashCommands.MeasureTemp}' command: '{result}'");
            var temperatureResult = result[(result.IndexOf('=') + 1) ..result.IndexOf("'")];
            this.Logger.Trace($"Temperature substring: '{temperatureResult}'");
            if (!double.TryParse(temperatureResult, out var temperature))
            {
                this.Logger.Warn($"Could not parse temperature: '{temperatureResult}'");
            }

            result = BashCommands.MeasureVolts.Bash();
            this.Logger.Trace($"Result of '{BashCommands.MeasureVolts}' command: '{result}'");
            var voltageResult = result[(result.IndexOf('=') + 1) ..result.IndexOf("V")];
            this.Logger.Trace($"Voltage substring: '{voltageResult}'");
            if (!double.TryParse(voltageResult, out var voltage))
            {
                this.Logger.Warn($"Could not parse voltage: '{voltageResult}'");
            }

            result = BashCommands.GetThrottled.Bash();
            this.Logger.Trace($"Result of '{BashCommands.GetThrottled}' command: '{result}'");
            var getThrottledResult = result[(result.IndexOf('x') + 1) ..result.Length];
            this.Logger.Trace($"Throttled substring: '{getThrottledResult}'");
            var getThrottledInBinary = Convert.ToString(Convert.ToInt32(getThrottledResult, 16), 2);
            var binaryLength = getThrottledInBinary.Length;

            return Task.FromResult(new CpuSensorsStatus()
            {
                Temperature = temperature,
                Voltage = voltage,
                UnderVoltageDetected = binaryLength > 0 && '1'.Equals(getThrottledInBinary[binaryLength - 1]),
                ArmFrequencyCapped = binaryLength > 1 && '1'.Equals(getThrottledInBinary[binaryLength - 2]),
                CurrentlyThrottled = binaryLength > 2 && '1'.Equals(getThrottledInBinary[binaryLength - 3]),
                SoftTemperatureLimitActive = binaryLength > 3 && '1'.Equals(getThrottledInBinary[binaryLength - 4]),
                UnderVoltageOccurred = binaryLength > 16 && '1'.Equals(getThrottledInBinary[binaryLength - 17]),
                ArmFrequencyCappingOccurred = binaryLength > 17 && '1'.Equals(getThrottledInBinary[binaryLength - 18]),
                ThrottlingOccurred = binaryLength > 18 && '1'.Equals(getThrottledInBinary[binaryLength - 19]),
                SoftTemperatureLimitOccurred = binaryLength > 19 && '1'.Equals(getThrottledInBinary[binaryLength - 20]),
                DateTime = DateTime.Now
            });
        }

        /// <inheritdoc/>
        public IObservable<CpuSensorsStatus> GetSensorsStatusObservable()
        {
            this.Logger.Debug("Infra layer -> CpuService -> GetSensorsStatusObservable");
            return this.cpuSensorsStatusSubject.AsObservable();
        }

        /// <inheritdoc/>
        public void PublishSensorsStatus(CpuSensorsStatus sensorsStatus)
        {
            this.Logger.Debug("Infra layer -> CpuService -> PublishSensorsStatus");
            this.cpuSensorsStatusSubject.OnNext(sensorsStatus);
        }

        /// <inheritdoc/>
        public async Task<CpuFrequency> GetFrequencyAsync(int samplingInterval)
        {
            this.Logger.Debug("Infra layer -> CpuService -> GetFrequencyAsync");

            var result = BashCommands.CatCpuFreqStats.Bash();
            this.Logger.Trace($"Result of '{BashCommands.CatCpuFreqStats}' command: '{result}'");
            string[] lines = result.Split(
                new[] { Environment.NewLine },
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
                    this.Logger.Warn($"Could not parse frequency stats: '{line}'");
                }
            }

            await Task.Delay(samplingInterval);

            result = BashCommands.CatCpuFreqStats.Bash();
            this.Logger.Trace($"Result of '{BashCommands.CatCpuFreqStats}' command: '{result}'");
            lines = result.Split(
                new[] { Environment.NewLine },
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
                    this.Logger.Warn($"Could not parse frequency stats: '{line}'");
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

            this.Logger.Warn($"Could not get cpu frequency stats");
            return null;
        }

        /// <inheritdoc/>
        public IObservable<CpuFrequency> GetFrequencyObservable()
        {
            this.Logger.Debug("Infra layer -> CpuService -> GetFrequencyObservable");
            return this.cpuFrequencySubject.AsObservable();
        }

        /// <inheritdoc/>
        public void PublishFrequency(CpuFrequency frequency)
        {
            this.Logger.Debug("Infra layer -> CpuService -> PublishFrequency");
            this.cpuFrequencySubject.OnNext(frequency);
        }

        /// <inheritdoc/>
        protected override Cpu GetModel()
        {
            var result = BashCommands.CatProcCpuInfo.Bash();
            this.Logger.Trace($"Result of '{BashCommands.CatProcCpuInfo}' command: '{result}'");
            string[] lines = result.Split(
                new[] { Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries);

            var cores = lines.Count(line => line.StartsWith("processor"));
            this.Logger.Trace($"Number of cores: '{cores}'");
            var model = lines.LastOrDefault(line => line.StartsWith("model name"));
            model = string.IsNullOrWhiteSpace(model) ? "N/A" :
                model.Split(':')[1].Trim();
            this.Logger.Trace($"Cpu model: '{model}'");

            result = BashCommands.CatScalingGovernor.Bash();
            this.Logger.Trace($"Result of '{BashCommands.CatScalingGovernor}' command: '{result}'");
            var scalingGovernor = result;

            result = BashCommands.CatBootConfig.Bash();
            this.Logger.Trace($"Result of '{BashCommands.CatBootConfig}' command: '{result}'");
            lines = result.Split(
                new[] { Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries);
            var frequencyLine = lines.FirstOrDefault(line => line.Contains("arm_freq="));
            var frequencyLineRegex = new Regex(@"^(?<commented>#?)\s*arm_freq=(?<frequency>\d+)$");
            this.Logger.Trace($"Frequency line in config file: '{frequencyLine}'");
            var frequencyLineGroups = frequencyLineRegex.Match(frequencyLine).Groups;
            var frequency = !string.IsNullOrEmpty(frequencyLineGroups["commented"].Value) ?
                1500 : int.Parse(frequencyLineGroups["frequency"].Value);

            return new Cpu()
            {
                Cores = cores,
                Model = model,
                MaximumFrequency = frequency,
                ScalingGovernor = scalingGovernor
            };
        }

        private IList<CpuProcess> GetProcesses(IList<string> processLines, DateTime dateTime)
        {
            var processes = new List<CpuProcess>();
            var regex = new Regex(@"^\s*(?<pid>\S*)\s*(?<user>\S*)\s*(?<pr>\S*)\s*(?<ni>\S*)\s*(?<virt>\S*)\s*(?<res>\S*)\s*(?<shr>\S*)\s*(?<s>\w)\s*(?<cpu>\d+\.\d)\s*(?<mem>\d+\.\d)\s*(?<time>\S*)\s*(?<command>.*)$");

            foreach (var line in processLines)
            {
                var groups = regex.Match(line).Groups;
                processes.Add(new CpuProcess()
                {
                    ProcessId = int.Parse(groups["pid"].Value),
                    User = groups["user"].Value,
                    Priority = groups["pr"].Value,
                    NiceValue = int.Parse(groups["ni"].Value),
                    TotalMemory = groups["virt"].Value,
                    Ram = groups["res"].Value,
                    SharedMemory = groups["shr"].Value,
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
