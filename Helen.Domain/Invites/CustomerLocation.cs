using System;
using Helen.Domain.Enum;

namespace Helen.Domain.Invites
{
	public class CustomerLocation
	{
        public string Name { get; set; }

        public DateTime? WeekdayOpenTime { get; set; }
        public DateTime? WeekdayCloseTime { get; set; }
        public DateTime? SaturdayOpenTime { get; set; }
        public DateTime? SaturdayCloseTime { get; set; }
        public DateTime? SundayOpenTime { get; set; }
        public DateTime? SundayCloseTime { get; set; }
        public string? Type { get; set; }
        public string? Location { get; set; }
        public string? Area { get; set; }
        public string? ExtraInformation { get; set; }
        public decimal? Budget { get; set; }
        public DateTime? DateAdded { get; set; }

    }
}

