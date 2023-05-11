using System;
using System.Collections.Generic;

namespace AspNetIdentity.WebApi.Model
{
    public class ImportInvestmentDeclaration
    {
        public List<ImportInvestmentDeclaration> ImportInvestmentDeclarationList { get; set; }
        public string FinancialYear { get; set; }
        public int EmployeeId { get; set; }
        public double PPF { get; set; }
        public double SeniorCitizenSavingsScheme { get; set; }
        public double Housingloan { get; set; }
        public double MutualFund { get; set; }
        public double NationalSavingCertificate { get; set; }
        public double UnitLinkInsurancePlan { get; set; }
        public double LifeInsurancePolicy { get; set; }
        public double EducationTuitionFees { get; set; }
        public double ScheduleBankFD { get; set; }
        public double PostOfficeTimeDeposit { get; set; }
        public double DeferredAnnuity { get; set; }
        public double SuperAnnuation { get; set; }
        public double NABARDnotifiedbonds { get; set; }
        public double SukanyaSamriddhiYojna { get; set; }
        public double Other { get; set; }
        public double MutualFundPension { get; set; }
        public double CCD1_NPSEmployeeContribution { get; set; }
        public double CCD1B_NPSEmployeeContribution { get; set; }
        public double CCD2_NPSEmployerContribution { get; set; }
        public double RajivGandhiEquitySavingsScheme { get; set; }
        public double MedicalInsurancePremium { get; set; }
        public double ParentsMedicalInsurancePremium { get; set; }
        public double PreventiveHealthCheckup { get; set; }
        public double MedicalExpenditureforaHandicappedRelative { get; set; }
        public double MedicalExpenditureonSelforDependent { get; set; }
        public double RepaymentofInterestonHigherEducationLoan { get; set; }
        public double HomeLoanInterestforFirstTimeHomeOwners { get; set; }
        public double DonationTowardsSocialCauses { get; set; }
        public double DonationforResearchorRuraldevelopment { get; set; }
        public double Donationstopoliticalparties { get; set; }
        public double Royaltyonbook { get; set; }
        public double Royaltyonpatent { get; set; }
        public double DeductionPersonsufferingPhysicalDisability { get; set; }
        public double MedicalAllowance { get; set; }
        public double TravelReimbursementLTA { get; set; }
        public double DailyAllowance { get; set; }
        public double HouseRentPaid { get; set; }
        public string LandlordName { get; set; }
        public string LandlordAddress { get; set; }
        public string LandlordPAN { get; set; }
        public double HomeLoanInterest { get; set; }
        public double AnnualRentReceived { get; set; }
        public double MunicipalTaxesPaid { get; set; }
        public double UnrealizedRent { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public string Message { get; set; }
    }
}