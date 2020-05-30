﻿namespace PiControlPanel.Domain.Contracts.Application
{
    using PiControlPanel.Domain.Models.Hardware;

    /// <summary>
    /// Application layer service for operations on GPU model.
    /// </summary>
    public interface IGpuService : IBaseService<Gpu>
    {
    }
}
