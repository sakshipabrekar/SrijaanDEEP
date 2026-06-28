using System;
using System.ComponentModel.DataAnnotations;

namespace srijaanDEEP.DTOs
{
    public class DefencePlatformCreateDto
    {
        [Required(ErrorMessage = "Defence Platform name is required.")]
        [StringLength(200, ErrorMessage = "Defence Platform name cannot exceed 200 characters.")]
        public string DefencePlatform { get; set; } = string.Empty;

        public bool ManualBulkUpload { get; set; } = false;
    }

    public class DefencePlatformUpdateDto
    {
        [Required(ErrorMessage = "Defence Platform name is required.")]
        [StringLength(200, ErrorMessage = "Defence Platform name cannot exceed 200 characters.")]
        public string DefencePlatform { get; set; } = string.Empty;

        public bool ManualBulkUpload { get; set; }
    }

    public class DefencePlatformResponseDto
    {
        public int Id { get; set; }
        public string DefencePlatform { get; set; } = string.Empty;
        public bool ManualBulkUpload { get; set; }
        public string? Uploaded_by { get; set; }
        public DateTime? Uploaded_DateTime { get; set; }
        public string? Upload_IP { get; set; }
        public string? Last_Modified_by { get; set; }
        public DateTime? Last_Modified_DateTime { get; set; }
        public string? Modify_IP { get; set; }
    }
}