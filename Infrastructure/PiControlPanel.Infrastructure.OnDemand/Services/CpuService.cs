﻿namespace PiControlPanel.Infrastructure.OnDemand.Services
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
            logger.Info("Infra layer -> CpuService -> GetLoadStatusAsync");
            var averageLoad = this.GetLoadStatus(cores);
            return Task.FromResult(averageLoad);
        }

        public IObservable<CpuLoadStatus> GetLoadStatusObservable()
        {
            logger.Info("Infra layer -> CpuService -> GetLoadStatusObservable");
            return this.cpuLoadStatusSubject.AsObservable();
        }

        public void PublishLoadStatus(CpuLoadStatus loadStatus)
        {
            logger.Info("Infra layer -> CpuService -> PublishLoadStatus");
            this.cpuLoadStatusSubject.OnNext(loadStatus);
        }

        public Task<CpuTemperature> GetTemperatureAsync()
        {
            logger.Info("Infra layer -> CpuService -> GetTemperatureAsync");
            var temperature = this.GetTemperature();
            return Task.FromResult(temperature);
        }

        public IObservable<CpuTemperature> GetTemperatureObservable()
        {
            logger.Info("Infra layer -> CpuService -> GetTemperatureObservable");
            return this.cpuTemperatureSubject.AsObservable();
        }

        public void PublishTemperature(CpuTemperature temperature)
        {
            logger.Info("Infra layer -> CpuService -> PublishTemperature");
            this.cpuTemperatureSubject.OnNext(temperature);
        }

        public Task<CpuFrequency> GetFrequencyAsync()
        {
            logger.Info("Infra layer -> CpuService -> GetFrequencyAsync");
            var frequency = this.GetFrequency();
            return Task.FromResult(frequency);
        }

        public IObservable<CpuFrequency> GetFrequencyObservable()
        {
            logger.Info("Infra layer -> CpuService -> GetFrequencyObservable");
            return this.cpuFrequencySubject.AsObservable();
        }

        public void PublishFrequency(CpuFrequency frequency)
        {
            logger.Info("Infra layer -> CpuService -> PublishFrequency");
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

            return new Cpu()
            {
                Cores = cores,
                Model = model
            };
        }

        private CpuFrequency GetFrequency()
        {
            var result = BashCommands.MeasureClock.Bash();
            logger.Debug($"Result of '{BashCommands.MeasureClock}' command: '{result}'");

            var frequencyResult = result.Substring(result.IndexOf('=') + 1);
            logger.Debug($"Frequency substring: '{frequencyResult}'");

            if (double.TryParse(frequencyResult, out var frequency))
            {
                return new CpuFrequency()
                {
                    Frequency = Convert.ToInt32(frequency / 1000000),
                    DateTime = DateTime.Now
                };
            }
            logger.Warn($"Could not parse frequency: '{frequencyResult}'");
            return null;
        }

        private CpuTemperature GetTemperature()
        {
            var result = BashCommands.MeasureTemp.Bash();
            logger.Debug($"Result of '{BashCommands.MeasureTemp}' command: '{result}'");

            var temperatureResult = result.Substring(result.IndexOf('=') + 1, result.IndexOf("'") - (result.IndexOf('=') + 1));
            logger.Debug($"Temperature substring: '{temperatureResult}'");

            if (double.TryParse(temperatureResult, out var temperature))
            {
                return new CpuTemperature()
                {
                    Temperature = temperature,
                    DateTime = DateTime.Now
                };
            }
            logger.Warn($"Could not parse temperature: '{temperatureResult}'");
            return null;
        }

        private CpuLoadStatus GetLoadStatus(int cores)
        {
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

            return new CpuLoadStatus()
            {
                LastMinuteAverage = (100 * double.Parse(averageLoadGroups["lastMinute"].Value)) / cores,
                Last5MinutesAverage = (100 * double.Parse(averageLoadGroups["last5Minutes"].Value)) / cores,
                Last15MinutesAverage = (100 * double.Parse(averageLoadGroups["last15Minutes"].Value)) / cores,
                UserRealTime = double.Parse(realTimeLoadGroups["user"].Value),
                KernelRealTime = double.Parse(realTimeLoadGroups["kernel"].Value),
                Processes = this.GetProcesses(processLines, dateTime),
                DateTime = dateTime
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
