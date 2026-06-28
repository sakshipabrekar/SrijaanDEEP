using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace srijaanDEEP.Models
{
    [Table("Defence_Platform_Master")]
    public class DefencePlatformMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        [Column("Defence Platform")]
        public string DefencePlatform { get; set; }

        [Column("Manual_Bulk_upload")]
        public bool ManualBulkUpload { get; set; } = false;

        [StringLength(100)]
        public string Uploaded_by { get; set; }

        public DateTime? Uploaded_DateTime { get; set; }

        [StringLength(50)]
        public string Upload_IP { get; set; }

        [StringLength(100)]
        public string Last_Modified_by { get; set; }

        public DateTime? Last_Modified_DateTime { get; set; }

        [StringLength(50)]
        public string Modify_IP { get; set; }
    }
}