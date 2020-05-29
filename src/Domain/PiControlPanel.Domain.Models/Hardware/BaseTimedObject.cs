namespace PiControlPanel.Domain.Models.Hardware
{
    using System;

    /// <summary>
    /// A class representing a model with time information.
    /// </summary>
    public class BaseTimedObject
    {
        /// <summary>
        /// Gets or sets the object unique identifier.
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// Gets or sets the creation datetime.
        /// </summary>
        public DateTime DateTime { get; set; }
    }
}
