export const ChartData = [
    {
        name: "CPU Frequency (MHz)",
        seriesMethod: "getOrderedAndMappedCpuNormalizedFrequencies"
    },
    {
        name: "CPU Temperature (°C)",
        seriesMethod: "getOrderedAndMappedCpuTemperatures"
    },
    {
        name: "CPU Real-Time Load (%)",
        seriesMethod: "getOrderedAndMappedCpuLoadStatuses"
    },
    {
        name: "RAM Usage (%)",
        seriesMethod: "getOrderedAndMappedRamStatuses"
    },
    {
        name: "Swap Memory Usage (%)",
        seriesMethod: "getOrderedAndMappedSwapMemoryStatuses"
    }
];
