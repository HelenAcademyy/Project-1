using System;
using System.ComponentModel.DataAnnotations;

namespace Helen.Domain.Invites.Response
{
    public class LocationNotificationData
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(250)]
        public string Name { get; set; }

        public DateTime? WeekdayOpenTime { get; set; }
        public DateTime? WeekdayCloseTime { get; set; }
        public DateTime? SaturdayOpenTime { get; set; }
        public DateTime? SaturdayCloseTime { get; set; }
        public DateTime? SundayOpenTime { get; set; }
        public DateTime? SundayCloseTime { get; set; }

        [MaxLength(250)]
        public string Type { get; set; } = string.Empty;

        [MaxLength(250)]
        public string Location { get; set; } = string.Empty;

        [MaxLength(500)]
        public string ExtraInformation { get; set; } = string.Empty;

        public decimal? Budget { get; set; }
        public bool AvailableForRent { get; set; }
        public decimal? RentPrice { get; set; }
        public DateTime? DateAdded { get; set; }
    }
}
