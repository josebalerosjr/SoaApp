using System;

namespace SoaApp.Models
{
    public class Customer
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string StreetAddress { get; set; }
        public string CityAddress { get; set; }
        public string AttentionTo { get; set; }
        public DateTime AsOfDate { get; set; }
        public double PreviousBalance { get; set; }
    }
}