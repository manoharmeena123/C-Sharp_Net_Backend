using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GenricEcommers.Models
{
    public class RewardPointHindmt
    {
        [Key]
        public int Id { get; set; }

        public int CustomerId { get; set; }

        //public int CompanyId { get; set; }
        //public int Warehouseid { get; set; }
        public string OrderNumber { get; set; }

        public double? TotalPoint { get; set; }
        public double? EarningPoint { get; set; }
        public double? UsedPoint { get; set; }
        public double? MilestonePoint { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? TransactionDate { get; set; }
        public bool Deleted { get; set; }
        public DateTime? UpdatedDate { get; set; }

        [NotMapped]
        public string LScode { get; set; }

        [NotMapped]
        public string ShopName { get; set; }
    }

    public class RPConversion
    {
        [Key]
        public int Id { get; set; }

        public int? CompanyId { get; set; }

        //   public int Warehouseid { get; set; }
        public double point { get; set; }

        public double rupee { get; set; }
    }

    public class MilestonePoint
    {
        [Key]
        public int M_Id { get; set; }

        //public int CompanyId { get; set; }
        //public int Warehouseid { get; set; }
        public double rPoint { get; set; }

        public double mPoint { get; set; }
        public bool active { get; set; }
    }

    public class RetailerShare
    {
        [Key]
        public int S_Id { get; set; }

        public int? CompanyId { get; set; }

        //public int Warehouseid { get; set; }
        public int cityid { get; set; }

        public string cityName { get; set; }
        public double share { get; set; }
    }

    public class CashConversion
    {
        [Key]
        public int Id { get; set; }

        public double point { get; set; }
        public double rupee { get; set; }

        [NotMapped]
        public int? PeopleId { get; set; }
    }

    public class WarehousePoint
    {
        [Key]
        public int Id { get; set; }

        public int CompanyId { get; set; }
        public int WarehouseId { get; set; }

        //public int CustomerId { get; set; }
        public double availablePoint { get; set; }

        public double UsedPoint { get; set; }
        public double TotalPoint { get; set; }
    }

    public class WarehousePointLimit
    {
        [Key]
        public int Id { get; set; }

        public int CompanyId { get; set; }
        public double purchasePoint { get; set; }
        public int WarehouseId { get; set; }
        public double pointAvailable { get; set; }
        public double usedPoint { get; set; }
        public double totalPoint { get; set; }
        public double maxPointLimit { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }

    public class CashConversionHistory
    {
        [Key]
        public int Id { get; set; }

        public double point { get; set; }
        public double rupee { get; set; }
        public int? PeopleId { get; set; }
        public string PeopleName { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}