using System;
using System.ComponentModel.DataAnnotations;

namespace AngularJSAuthentication.Model
{
    #region GrowthModuleMasters

    public class GMLegalDocMaster
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }

        //public ICollection<GMInfrastructureLegal> GMInfrastructureLegals { get; set; }
    }

    public class GMTaskListMaster
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
        public int Points { get; set; }
        public int Quantity { get; set; }
        public bool IsUploaded { get; set; }
        public string ImagePath { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public bool? IsRequireddoc { get; set; }
        //public ICollection<GMInfrastructureTask> GMInfrastructureTasks { get; set; }
    }

    public class GMDivisionMaster
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
        public bool IsTrainingRequired { get; set; }
        public int RequiredQty { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        //public ICollection<GMPeople> GMPeoples { get; set; }
        //public ICollection<GMTrainingDevlopment> GMTrainingDevlopment { get; set; }
    }

    public class GMProductPartnerMaster
    {
        [Key]
        public int Id { get; set; }

        public string Type { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        //public ICollection<GMProductPartners> GMProductPartners { get; set; }
    }

    public class GMCityMappingMaster
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
        public int RequiredQuantity { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        //public ICollection<GMCityMapping> GMCityMappings { get; set; }
    }

    public class GMWarehouseProgress
    {
        [Key]
        public int Id { get; set; }

        public int WarehouseID { get; set; }
        public string WarehouseName { get; set; }
        public int Progress { get; set; }
        public bool IsLaunched { get; set; }
    }

    #endregion GrowthModuleMasters

    #region GrowthModuleTransaction

    public class GMInfrastructure
    {
        [Key]
        public int Id { get; set; }

        public int WarehouseId { get; set; }
        public string TaskDocument { get; set; }
        public decimal LegalPercent { get; set; }
        public decimal TaskPercent { get; set; }
        public int WarehouseLaunchDays { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }

        //public ICollection<GMInfrastructureLegal> GMInfrastructureLegals { get; set; }
        //public ICollection<GMInfrastructureTask> GMInfrastructureTasks { get; set; }
    }

    public class GMInfrastructureLegal
    {
        [Key]
        public int Id { get; set; }

        public int GMInfrastructureId { get; set; }
        public int LegalDocId { get; set; }
        public bool IsUploaded { get; set; }

        //public string ImagePath { get; set; }
        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        //public GMLegalDocMaster GMLegalDocMaster { get; set; }
        //public GMInfrastructure GMInfrastructure { get; set; }
    }

    public class GMInfrastructureTask
    {
        [Key]
        public int Id { get; set; }

        public int GMInfrastructureId { get; set; }
        public int TaskListId { get; set; }
        public bool IsInstalled { get; set; }
        public bool? IsUploaded { get; set; }

        //public string ImagePath { get; set; }
        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        //public GMTaskListMaster GMTaskListMaster { get; set; }
        //public GMInfrastructure GMInfrastructure { get; set; }
    }

    public class GMInfraTaskImages
    {
        [Key]
        public int ID { get; set; }

        public int GMInfrastructureTaskID { get; set; }
        public string ImagePath { get; set; }
    }

    public class GMInfraLegalImages
    {
        [Key]
        public int ID { get; set; }

        public int GMInfrastructureLegalID { get; set; }
        public string ImagePath { get; set; }
    }

    public class GMPeople
    {
        [Key]
        public int Id { get; set; }

        public int DivisionId { get; set; }
        public int RequiredQuantity { get; set; }
        public int FilledQuantity { get; set; }
        public int WarehouseId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        // public GMDivisionMaster GMDivisionMaster { get; set; }
    }

    public class GMProductPartners
    {
        [Key]
        public int Id { get; set; }

        public int ProductPartnersId { get; set; }
        public int RequiredQuantity { get; set; }
        public int FilledQuantity { get; set; }
        public int WarehouseId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        //public GMProductPartnerMaster GMProductPartnerMaster { get; set; }
    }

    public class GMTrainingDevlopment
    {
        [Key]
        public int Id { get; set; }

        public int DivisionId { get; set; }

        //public string UploadOnboardSheetPath { get; set; }
        public int WarehouseId { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsUploaded { get; set; }
        public bool IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        //public GMDivisionMaster GMDivisionMaster { get; set; }
    }

    public class GMTrainingDevlopmentImages
    {
        [Key]
        public int ID { get; set; }

        public int GMTrainingDevlopmentId { get; set; }
        public string ImagePath { get; set; }
    }

    public class GMCityMapping
    {
        [Key]
        public int Id { get; set; }

        public int CityMappingMasterId { get; set; }
        public int RequiredQuantity { get; set; }
        public int FilledQuantity { get; set; }
        public int WarehouseId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }

        //public GMCityMappingMaster GMCityMappingMaster { get; set; }
    }

    #endregion GrowthModuleTransaction
}