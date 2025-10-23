using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CMCS.Web.Models
{
    public enum ClaimStatus { Pending, Approved, Rejected }

    public class Claim
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Lecturer Name")]
        public string LecturerName { get; set; }

        [Required]
        [Display(Name = "Month")]
        public string Month { get; set; }

        [Required]
        [Range(0, 1000, ErrorMessage = "Hours must be between 0 and 1000")]
        [Display(Name = "Total Hours")]
        public double TotalHours { get; set; }

        [Required]
        [Range(0, 100000, ErrorMessage = "Rate must be between 0 and 100000")]
        [Display(Name = "Hourly Rate")]
        public decimal HourlyRate { get; set; }

        [Display(Name = "Total Amount (R)")]
        public decimal TotalAmount => Math.Round((decimal)TotalHours * HourlyRate, 2);

        public string Notes { get; set; }

        public ClaimStatus Status { get; set; } = ClaimStatus.Pending;

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        public List<UploadFile> Uploads { get; set; } = new();
    }
}
