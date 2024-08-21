using System;
using System.ComponentModel.DataAnnotations;
using Helen.Domain.Enum;
using NetTopologySuite.Algorithm;
using NetTopologySuite.Geometries;

namespace Helen.Repository
{
    public class UserData
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(200)]
        public string? Username { get; set; }
        public DateTime DateOfBirth { get; set; }
        [MaxLength(20)]
        public string? PhoneNumber { get; set; }
        [MaxLength(100)]
        public string? Email { get; set; }
        public decimal Budget { get; set; }
        public bool IsSmoker { get; set; }
        public int Age => CalculateAge(DateOfBirth);
        public ReminderFrequency ReminderFrequency { get; set; }
        public ProfileStatus Status { get; set; }
        public DateTime ReminderTime { get; set; }
        public bool SendViaMail { get; set; }
        public bool SendViaPhone { get; set; }
        public string Location
        {
            get => _location;
            set => _location = value.ToLower();
        }
        private int CalculateAge(DateTime dateOfBirth)
        {
            var today = DateTime.Today;
            var age = today.Year - dateOfBirth.Year;

            if (dateOfBirth.Date > today.AddYears(-age)) age--;

            return age;
        }
        private string _location = string.Empty;

    }
}
