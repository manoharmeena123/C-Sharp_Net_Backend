using System;
using System.ComponentModel.DataAnnotations;

namespace AngularJSAuthentication.Model
{
    public class MultipleAddress
    {
        [Key]
        public int Id { get; set; }

        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string HouseNo { get; set; }
        public string SubLocality { get; set; }
        public string AddressType { get; set; }
        public int Zipcode { get; set; }
        public string GPSLocation { get; set; }

        //public bool Flag { get; set; }
        //public string Message { get; set; }
        public string LandMark { get; set; }

        public int CustomerId { get; set; }
        public int Cityid { get; set; }
        public string CustomerAddress { get; set; }
        public string Latitude { get; set; }
        public string Longtitude { get; set; }
        public int Addressid { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public string CustomerName { get; set; }
        public string Mobile { get; set; }
    }
}