namespace PiControlPanel.Infrastructure.Persistence.Entities.Disk
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Disk : BaseEntity
    {
        [Key]
        [DefaultValue(0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }

        public ICollection<FileSystem> FileSystems { get; set; }
    }
}
