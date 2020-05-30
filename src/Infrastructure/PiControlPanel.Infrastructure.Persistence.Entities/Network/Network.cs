namespace PiControlPanel.Infrastructure.Persistence.Entities.Network
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <inheritdoc/>
    public class Network : BaseEntity
    {
        /// <summary>
        /// Gets or sets the entity identifier.
        /// </summary>
        [Key]
        [DefaultValue(0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the network interfaces.
        /// </summary>
        public ICollection<NetworkInterface> NetworkInterfaces { get; set; }
    }
}
