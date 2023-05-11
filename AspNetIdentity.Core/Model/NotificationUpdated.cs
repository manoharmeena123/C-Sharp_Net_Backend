using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AngularJSAuthentication.Model
{
    public class NotificationUpdated
    {
        [Key]
        public int Id { get; set; }

        public int CompanyId { get; set; }
        public string title { get; set; }
        public string Message { get; set; }
        public string Pic { get; set; }
        public string NotifiedTo { get; set; }
        public string CityName { get; set; }
        public string ClusterName { get; set; }
        public int WarehouseID { get; set; }
        public int GroupID { get; set; }
        public string WarehouseName { get; set; }
        public string GroupName { get; set; }
        public string GroupAssociation { get; set; }
        public string NotificationType { get; set; }
        public string NotificationName { get; set; }
        public string SentVia { get; set; }
        public int TotalSent { get; set; }
        public int TotalViews { get; set; }
        public int TotalAction { get; set; }
        public DateTime? NotificationTime { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public DateTime? From { get; set; }
        public DateTime? TO { get; set; }
        public bool Deleted { get; set; }
        public int ItemCode { get; set; }
        public string ItemName { get; set; }
        public int BrandCode { get; set; }
        public string BrandName { get; set; }
        public string NotificationCategory { get; set; }
        public int NotificationId { get; set; }
        public bool Sent { get; set; }

        // [NotMapped]
        // public dynamic ids { get; set; }
        [NotMapped]
        public List<int> ids { get; set; }

        [NotMapped]
        public string priority { get; set; }

        [NotMapped]
        public string ErrorMessage { get; set; }
    }
}