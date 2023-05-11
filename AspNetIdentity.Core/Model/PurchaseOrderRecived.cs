using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AngularJSAuthentication.Model
{
    /// <summary>
    ///  Model for GR detail
    ///  Note: containing multiple GR of single PO
    ///  created on 09/07/2019 by yogendra namdev
    /// </summary>
    public class PurchaseOrderMasterRecivedHindmt
    {
        [Key]
        public int PurchaseOrderMasterRecivedId { get; set; }

        public string GrNumber { get; set; }
        public int CompanyId { get; set; }
        public int PurchaseOrderId { get; set; }
        public int SupplierId { get; set; }
        public string SupplierName { get; set; }
        public int? WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public string Status { get; set; }
        public string Level { get; set; }
        public DateTime? GrDate { get; set; }
        public double? GrDiscount { get; set; }
        public double GrTotalAmount { get; set; }
        public string VehicleType { get; set; }
        public string VehicleNumber { get; set; }
        public int? GrPersonId { get; set; }
        public string GrPersonName { get; set; }
        public string ApprovedById { get; set; }
        public string ApprovedByName { get; set; }
        public string Comments { get; set; }
        public DateTime CreationDate { get; set; }
        public string CreatedBy { get; set; }
        public bool Acitve { get; set; }
    }

    /// <summary>
    /// Model for GR item detail
    /// </summary>
    public class PurchaseOrderDetailRecivedHindmt
    {
        [Key]
        public int PurchaseOrderDetailRecivedId { get; set; }

        public int? CompanyId { get; set; }
        public int PurchaseOrderMasterRecivedId { get; set; }
        public int PurchaseOrderDetailId { get; set; }
        public int PurchaseOrderId { get; set; }
        public int SupplierId { get; set; }
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; }

        [NotMapped]
        public bool isDeleted { get; set; }

        public string SupplierName { get; set; }
        public int ItemId { get; set; }
        public string SellingSku { get; set; }
        public string HSNCode { get; set; }
        public string SKUCode { get; set; }
        public string PurchaseSku { get; set; }
        public string ItemName { get; set; }
        public string Comment { get; set; }
        public double MRP { get; set; }
        public double Price { get; set; }
        public double PriceRecived { get; set; }
        public int MOQ { get; set; }
        public int TotalQuantity { get; set; }
        public double TaxAmount { get; set; }
        public double TotalTaxPercentage { get; set; }
        public int PurchaseQty { get; set; }
        public double TotalAmountIncTax { get; set; }
        public string Status { get; set; }
        public DateTime CreationDate { get; set; }
        public string CreatedBy { get; set; }
        public int? QtyRecived1 { get; set; }
        public int? QtyRecived2 { get; set; }
        public int? QtyRecived3 { get; set; }
        public int? QtyRecived4 { get; set; }
        public int? QtyRecived5 { get; set; }
        public double? Price1 { get; set; }
        public double? Price2 { get; set; }
        public double? Price3 { get; set; }
        public double? Price4 { get; set; }
        public double? Price5 { get; set; }
        public double? dis1 { get; set; }
        public double? dis2 { get; set; }
        public double? dis3 { get; set; }
        public double? dis4 { get; set; }
        public double? dis5 { get; set; }
        public double? CessTaxPercentage { get; set; }

        [NotMapped]
        public int? SupplierAcceptQty { get; set; }

        public double? QtyRecived
        {
            get
            {
                return QtyRecived1 + QtyRecived2 + QtyRecived3 + QtyRecived4 + QtyRecived5;
            }
        }

        public string BatchNo1 { get; set; }
        public string BatchNo2 { get; set; }
        public string BatchNo3 { get; set; }
        public string BatchNo4 { get; set; }
        public string BatchNo5 { get; set; }

        public DateTime? MFGDate1 { get; set; }
        public DateTime? MFGDate2 { get; set; }
        public DateTime? MFGDate3 { get; set; }
        public DateTime? MFGDate4 { get; set; }
        public DateTime? MFGDate5 { get; set; }

        //multimrp
        public int ItemMultiMRPId { get; set; }

        public int ItemMultiMRPId1 { get; set; }
        public int ItemMultiMRPId2 { get; set; }
        public int ItemMultiMRPId3 { get; set; }
        public int ItemMultiMRPId4 { get; set; }
        public int ItemMultiMRPId5 { get; set; }
        public string ItemName1 { get; set; }
        public string ItemName2 { get; set; }
        public string ItemName3 { get; set; }
        public string ItemName4 { get; set; }
        public string ItemName5 { get; set; }
        public int? DamagQtyRecived1 { get; set; }
        public int? DamagQtyRecived2 { get; set; }
        public int? DamagQtyRecived3 { get; set; }
        public int? DamagQtyRecived4 { get; set; }
        public int? DamagQtyRecived5 { get; set; }
        public int? ExpQtyRecived1 { get; set; }
        public int? ExpQtyRecived2 { get; set; }
        public int? ExpQtyRecived3 { get; set; }
        public int? ExpQtyRecived4 { get; set; }
        public int? ExpQtyRecived5 { get; set; }

        public DateTime? GRDate1 { get; set; }
        public DateTime? GRDate2 { get; set; }
        public DateTime? GRDate3 { get; set; }
        public DateTime? GRDate4 { get; set; }
        public DateTime? GRDate5 { get; set; }
        public string ItemNumber { get; set; }
    }
}