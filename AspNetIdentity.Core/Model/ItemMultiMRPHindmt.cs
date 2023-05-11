using System;
using System.ComponentModel.DataAnnotations;

namespace AngularJSAuthentication.Model
{
    public class ItemMultiMRPHindmt
    {
        [Key]
        public int ItemMultiMRPId { get; set; }

        public int CompanyId { get; set; }
        public string ItemNumber { get; set; }//relogic change after Mrp on sellingsku failed above one commented
        public double MRP { get; set; }
        public string UnitofQuantity { get; set; }
        public string UOM { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdateBy { get; set; }
        public bool Deleted { get; set; }

        //after 19/05/2019
        public string itemname { get; set; }

        public string itemBaseName { get; set; }
    }
}