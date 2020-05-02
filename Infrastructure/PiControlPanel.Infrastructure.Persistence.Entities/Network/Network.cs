namespace PiControlPanel.Infrastructure.Persistence.Entities.Network
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Network : BaseEntity
    {
        [Key]
        [DefaultValue(0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }

        public ICollection<Interface> Interfaces { get; set; }
    }
}
