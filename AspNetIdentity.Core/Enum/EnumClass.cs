namespace AspNetIdentity.WebApi.Model
{
    public class EnumClass
    {
        /// <summary>
        /// For Identify Job Category
        /// Created By Harshit Mitra on 01-02-2022
        /// </summary>
        public enum JobCategory
        {
            Active_Job = 1,
            Archived_job = 2,
        }

        /// <summary>
        /// For Identify Job Type
        /// Created By Harshit Mitra on 01-02-2022
        /// </summary>
        public enum JobType
        {
            Full_Time = 1,
            Part_Time = 2,
            Internships = 3,
        }

        /// <summary>
        /// For Identify Gender
        /// Created By Harshit Mitra on 01-02-2022
        /// </summary>
        public enum GenderConstants
        {
            Male = 1,
            Female = 2,
            Other = 3,
        }

        /// <summary>
        /// Its For Identify Staged Of Job
        /// Its For Identify Staged Of Job
        /// Created By Harshit Mitra on 02-02-2022
        /// </summary>
        public enum StageFlowType
        {
            Sourced = 1,
            Screening = 2,
            Interview = 3,
            Preboarding = 4,
            Hired = 5,
            Archived = 6,
        }

        /// <summary>
        /// Its For Identify Interview Type Of Job
        /// Created By Harshit Mitra on 02-02-2022
        /// </summary>
        public enum InterviewType
        {
            Phone_or_Video_Interview = 1,
            Face_To_Face_Interview = 2,
        }
        /// <summary>
        /// Its For Identify Interview Comment By 
        /// Created By Harshit Mitra on 02-02-2022
        /// </summary>
        public enum InterviewCommentBy
        {
            Recruiter = 1,
            Interviewer = 2,
        }

        /// <summary>
        /// Its For Identify Stages in Preboarding Flow
        /// Created By Harshit Mitra on 08-02-2022
        /// </summary>
        public enum PreboardingStages
        {
            Start = 1,
            Verfiy_Info = 2,
            Release_Offer = 3,
            Offer_Acceptance = 4,
            Hired = 5,
            Joined = 6,
            Collect_Info = 8,
            Archived = 7,
        }

        /// <summary>
        /// Its For Identify Hiring Team Designation
        /// </summary>
        public enum HiringTeamConstants
        {
            Recruiters = 1,
            Hiring_Managers = 2,
            Interview_Panel = 3,
        }

        /// <summary>
        /// Its For Identify Hiring Module Job Priority
        /// </summary>
        public enum JobPriorityHelperConstants
        {
            Low = 1,
            Medium = 2,
            High = 3,
            Urgent = 4,
        }

        /// <summary>
        /// Its For Identify Different Type Of Document
        /// Created By Harshit Mitra on 10-02-2022
        /// </summary>
        public enum DocType
        {
            Degrees_And_Certificates = 1,
            Identity_Docs = 2,
            Previous_Experience = 3,
        }

        /// <summary>
        /// Its For Identify Different Type Of Document Status Before Verifying And
        /// Created By Harshit Mitra on 10-02-2022
        /// </summary>
        public enum DocumentStatus
        {
            Not_Uploaded = 1,
            Uploaded = 2,
            Approved = 3,
            Rejected = 4,
        }



        /// <summary>
        /// Its For Identify Multiple Frequency of Pay Type Used in PayRoll Module
        /// Created By Harshit Mitra on 23-02-2022
        /// </summary>
        public enum PayFrequency
        {
            Monthly = 1,
            Weekly = 2,
        }



        /// <summary>
        /// Its For Identify Pf Type in Pf Contribution
        /// Created By Harshit Mitra on 15-03-2022
        /// </summary>
        public enum PfCalculationType
        {
            Not_Selected = 0,
            Limit_PF_amount_to_statutory_minimum_salary_for_all_employees = 1,
            Allow_PF_calculated_as_percentage_of_basic_salary_beyond_statutory_minimum = 2,
        }

        /// <summary>
        /// Its For Identify Ticket Status
        /// Created By Harshit Mitra on 22-03-2022
        /// </summary>
        public enum TicketStatus
        {
            Pending = 1,
            Progress = 2,
            Completed = 3,
        }

        /// <summary>
        /// Created By Harshit Mitra on 04-04-2022
        /// Its For Identify Category Type Used In Feedback
        /// </summary>
        public enum LoginRolesConstants
        {
            SuperAdmin = 0,
            Administrator = 1,
            Developer = 2,
            Account = 3,
            HR = 4,
            IT = 5,
            PM = 6,
            EmossyUser = 7,
            Client = 8,
        }



        /// <summary>
        /// Created By Harshit Mitra on 22-04-2022
        /// Its For Identify Employee Type
        /// </summary>
        public enum EmployeeTypeConstants
        {
            Pre_Confirmed_Employee = 1,
            Confirmed_Employee = 2,
            Ex_Employee = 3,
            Notice_Period = 4,
            Employee_On_Probation_Period = 5,
            Intern = 6,
        }

        /// <summary>
        /// Created By Harshit Mitra on 25-04-2022
        /// Its For Identify Blood Group Type
        /// </summary>
        public enum BloodGroupConstants
        {
            Unknown = 0,
            A_pos = 1,
            A_neg = 2,
            B_pos = 3,
            B_neg = 4,
            O_pos = 5,
            O_neg = 6,
            AB_pos = 7,
            AB_neg = 8
        }

        /// <summary>
        /// Created By Harshit Mitra on 02-05-2022
        /// Its For Identify Goal Types and In PT State Duration
        /// </summary>
        public enum GoalTypeEnum_PTStateDuration
        {
            Monthly = 1,
            Quarterly = 2,
            Half_Yearly = 3,
            Anually = 4,
        }

        /// <summary>
        /// Created By Harshit Mitra on 02-05-2022
        /// Its For Identify Goal Types and In PT State Duration
        /// </summary>
        public enum FrequenceConstants
        {
            Monthly = 1,
            Quarterly = 2,
            Half_Yearly = 3,
            Anually = 4,
        }

        /// <summary>
        /// Created by Ankit
        /// Its For Identify GoalStatus of Goal
        /// </summary>
        public enum GoalStatusConstants
        {
            Pending = 1,
            Reject = 2,
            InProgress = 3,
            InReview = 4,
            Complete = 5,
            ExtendRequest = 6,
            Approved = 7,
            DeleteRequest = 8,
            Deleted = 9,
            Extended = 10,
        }

        ///<summary>
        ///Created by Shriya
        ///Created on 13-05-2022
        /// Its For Identify Relationship Type
        ///</summary>
        public enum RelationShipTypeConstants
        {
            Others = 1,
            Spouse = 2,
            Father = 3,
            Mother = 4,
            Child = 5,
            Self = 6,
            Sibling = 7,
        }

        /// <summary>
        /// Created by Suraj Bundel on 26/05/2022
        /// Modified by Suraj Bundel on 04/07/2022
        /// Enum class for Expense Category
        /// </summary>
        public enum ExpenseTypeConstants
        {
            IT_courses_and_certification = 1,
            Travel_expenses = 2,
            Accomodation = 3,
            Food = 4,
            Other = 5
        }

        /// <summary>
        /// Created by Suraj Bundel on 27/05/2022
        /// Enum class for Mode of Payments
        /// </summary>
        public enum ModeofPaymentConstants
        {
            Cash = 0,
            Cheque = 1,
            InSalary = 2,
            Online = 3,
            Bank_Trasfer = 4,
        }

        /// <summary>
        /// Created By Harshit Mitra On 27-05-2022
        /// Its Used In Give FeedBack For Getting Feedback Type
        /// </summary>
        public enum FeedbackTypeConstants
        {
            //Others = 0,
            Employee = 1,
            //Department = 2,
        }

        /// <summary>
        /// This is used for TravelVia
        /// Create by Shriya , Created on Shriya
        /// </summary>
        public enum TravelViaConstants
        {
            By_RoadWay = 1,
            By_RailWay = 2,
            By_AirWay = 3,
        }

        /// <summary>
        /// Created by Shriya Created on 30-05-2022
        /// This is use for Expense treval in expense
        /// </summary>
        public enum ExpenseTravalStatusConstants
        {
            Pending = 1,
            Inprogress = 2,
            Hold = 3,
            Rejected = 4,
            Approved = 5,
        }

        /// <summary>
        /// Created by Ankit Jain Created on 31-05-2022
        /// This Is Use For Purches Order
        /// </summary>
        public enum PurchesItem
        {
            Computer = 1,
            Laptop = 2,
            Accessories = 3,
            Others = 4,
        }

        /// <summary>
        /// Created by Ankit Jain Created on 31-05-2022
        /// This Is Use For Purches Order
        /// </summary>
        public enum PaidByName
        {
            UPI = 1,
            MasterCard = 2,
            Cash = 3,
            Cheque = 4,
            Rupay = 5,
        }


        /// <summary>
        /// Created By Harshit Mitra on 15-06-2022
        /// Its For Checking Employee Pay Roll Status On Pay Roll Central Controller
        /// </summary>
        public enum AssignPayRollConstants
        {
            Assign = 1,
            Unassign = 2,
        }

        /// <summary>
        /// Created By Harshit Mitra on 17-06-2022
        /// Its For Checking Status Of Payroll When Its Run
        /// </summary>
        public enum RunPayRollStatusConstants
        {
            Pending = 1,
            Pre_Run_Complete = 2,
            Preview_Completed = 3,
            Completed = 4,
        }

        /// <summary>
        /// Created By Harshit Mitra on 17-06-2022
        /// Its For Checking Component Is For Incresing Salery OR Decreasing Salery Of Employee
        /// </summary>
        public enum RunPayRollComponentStatus
        {
            Increasing = 1,
            Decreasing = 2,
        }

        /// <summary>
        /// Created By Harshit Mitra on 06-07-2022
        /// Its For Checking Employee Utilization In Project Controller OR Project Master
        /// </summary>
        public enum EmployeeUtilizationConstants
        {
            Over_Utilized = 1,
            Under_Utilized = 2,
            Free_Employee = 3,
            No_Project_Assign = 4,
        }

        /// <summary>
        /// Create By Shriya Malvi on 11-07-2022
        /// Its For Assets Condition in Assets
        /// </summary>
        public enum AssetConditionConstants
        {
            Good = 1,
            Fair = 2,
            Damage = 3,
            UnderRepair = 4,
        }

        /// <summary>
        /// Create By Shriya Malvi on 11-07-2022
        /// Its For Assets Reason for mark as not avilable in Assets
        /// </summary>
        public enum AssetsReasonForMarkNotAvailConstants
        {
            Lost = 1,
            Theft = 2
        }

        /// <summary>
        /// Create By Suraj Bundel on 12-07-2022
        /// Its For Assets Reason for Asset Status in Assets
        /// </summary>
        public enum AssetStatusConstants
        {
            Available = 1,
            Assigned = 2,
            Damage = 3,
            UnderRepair = 4,
            //Notavailable = 5,
        }

        /// <summary>
        /// Created By Harshit Mitra on 22-07-2022
        /// Its For Leave Advance Filter For Checking Range Of Month Till Joining
        /// </summary>
        public enum RangedEnumOnLeave
        {
            IsGreterThan = 1,
            IsLessThan = 2,
            IsBetween = 3,
        }

        /// <summary>
        /// Created By Harshit Mitra on 25-07-2022
        /// Its For Leave Staus Used In Leave Request
        /// </summary>
        public enum LeaveStatusConstants
        {
            Pending = 1,
            Partially_Approved = 2,
            Approved = 3,
            Rejected = 4,
            Cancel = 5,
        }

        /// <summary>
        /// Created By Harshit Mitra on 25-07-2022
        /// Its For Leave Case Used In Leave Request (Apply Leave)
        /// </summary>
        public enum LeaveCase
        {
            First_Half = 1,
            Second_Half = 2,
            Full_Day = 3,
        }

        /// <summary>
        /// Created By Shriya Malvi on 05-08-2022
        /// Its for AssetsItemType used in assets module
        /// </summary>
        public enum AssetsItemType
        {
            Physical = 1,
            Digital = 2,
        }

        /// <summary>
        /// Created By Harshit Mitra on 08-08-2022
        /// Its For Notice Period Duration Enum In Employee Exits
        /// </summary>
        public enum NoticePeriodDurationConstants
        {
            Days = 1,
            Week = 2,
            Month = 3,
        }

        /// <summary>
        /// Created By Ravi Vyas On 09-08-2022
        /// its is use for exit employee resignation reason
        /// </summary>
        public enum ExitInitingType
        {
            Employee_Want_To_Resign = 1,
            Company_Decideds_To_Terminate = 2,
        }

        /// <summary>
        /// Created By Shriya Malvi On 09-08-2022
        /// its is use for exit employee resignation reason
        /// </summary>
        public enum ExitEmpReasonConstants
        {
            Other = 1,
            Explore_Other_Careers = 2,
            Personal_Reason = 3,
            Relocating = 4,
            Resignation = 5,
            Retirement = 6
        }

        /// <summary>
        /// Created By Ravi vyas On 09-08-2022
        /// its is use for exit employee resignation reason
        /// </summary>
        public enum ExitStatusConstants
        {
            Pending = 1,
            Approved = 2,
            Retain = 3,
            Assets = 4,
            Final_Settled = 5,
            Exit = 6,
        }

        /// <summary>
        /// Created By Harshit Mitra On 29-08-2022
        /// Its Use For geting Source Of Job Hirring in Job Hirring Module And Add Candidate
        /// </summary>
        public enum JobHiringSourceConstants
        {
            Indeed = 1,
            Linkedin = 2,
            Agency = 3,
            Website = 4,
            Referral = 5,
            Others = 6,
        }

        /// <summary>
        /// Created By Ravi Vyas On 30-08-2022
        /// For Identify AboutUs Status
        /// </summary>
        public enum AboutUsStatusConstants
        {
            Publish = 1,
            Draft = 2,
        }



        /// <summary>
        /// Created By Ravi Vyas On 03-09-2022
        /// For Identify Reaction Type
        /// </summary>
        public enum ReactionTypeConstants
        {
            Like = 1,
            Other = 2,
        }

        /// <summary>
        /// Created By Harshit Mitra On 02-09-2022
        /// For Defining Range Of Proffesional Tax Enum
        /// </summary>
        public enum PTRangeConstants
        {
            Zero_To_Range = 1,
            Range_To_Range = 2,
            Range_To_Infinity = 3,
        }




        public enum AttendenceArrivalStatusConstants
        {
            NotAvailable = 0,
            OnTime = 1,
            BeforeTime = 2,
            Late = 3,
            Halfday = 4,
        }

        public enum NavigationConstants
        {
            About_Us = 0,
            Benefits_And_Services = 1,
            Working_With_Us = 2,
            Policies = 3
            //Tools = 3,
            //Quick_Links = 4,
        }

        public enum ReviewCycleConstants
        {
            AllEmployeeInGroup = 1,
            JoiningDateEmployee = 2,
        }


        public enum AttendenceTypeConstants
        {
            NotAvailable = 0,
            Present = 1,
            HalfDay = 2,
            Leave = 3,
            Holiday = 4,
            WeekOf = 5,
        }

        /// <summary>
        /// Created By Ankit On 20-10-2022
        /// </summary>
        public enum RequestCandidatePriority
        {
            Low = 1,
            Medium = 2,
            High = 3,
        }

        /// <summary>
        /// Created By Ankit On 20-10-2022
        /// </summary>
        public enum RequestCandidatStatus
        {
            Accepted = 1,
            Rejected = 2,
            Pending = 3,
        }

        /// <summary>
        /// Created By Ankit On 08-11-2022
        /// </summary>
        public enum InterviewStatus
        {
            Canceled = 1,
            Rescheduled = 2,
        }
        public enum MailTypeInHiring
        {
            Sechedule = 1,
            Resechedule = 2,
            Cancled = 3,
        }




        public enum TaskRequestConstants
        {
            Not_Selected = 0,
            Approved = 1,
            Pending = 2,
        }
        public enum CompetencyTypeConstants
        {
            Core = 1,
            Common_Success = 2,
            Job_Specific = 3,
        }
        public enum ReviewsTypeConstants
        {
            Self = 1,
            managers = 2,
            Peers_Subordinates = 3,
            All_Reviewers = 4,
        }
        public enum OptionTypeConstants
        {
            Comment = 1,
            Single_Select = 2,
            Multi_Select = 3,

        }
        /// <summary>
        /// Created By Harshit Mitra 08-11-2022
        /// </summary>
        public enum LeaveAllocationType
        {
            Allocated_ones_a_year = 0,
            Accrued_periodically = 1,
        }

        /// <summary>
        /// Created By Suraj Bundel 14-11-2022
        /// </summary>
        public enum ComponentTypeConstants
        {
            Not_Select = 0,
            Basic = 1,
            Gross = 2,
        }
        /// <summary>
        /// Its For status of declaration 
        /// Created By Suraj Bundel on 15-11-2022
        /// </summary>
        public enum DeclarationStatusConstants
        {
            Not_Sublimated = 0,
            Pending = 1,
            Approved = 2,
            Rejected = 3,
        }
        #region Pay Roll Enum Classes
        public enum PayRollSetupConstants
        {
            Company_Info = 1,
            General_Setting = 2,
            PF_ESI_Setting = 3,
            Salary_Components = 4,
            Tax_Deduction_Components = 5,
            Salary_Structure = 6,
            Finance_Setting = 7,
            Statutory_Filling = 8,
        }
        public enum PayRollComponentTypeConstants
        {
            Fixed = 0,
            Allowance = 1,
            Reimbursable_Component = 2,
            Recurring_Deduction = 3,
            Reimbursement = 4,
            Other = 5,
        }
        public enum AdHocComponentTypeConstants
        {
            Ad_Hoc_Component = 1,
            Bonus_Component = 2,
            Deduction_Component = 3,
        }
        public enum DeductionForConstants
        {
            Undefined = 0,
            Employee = 1,
            Employers = 2,
        }
        public enum ComponentTypeInPGConstants
        {
            RecurringComponent = 1,
            AdHocComponent = 2,
            BonusComponent = 3,
            DeductionComponent = 4,
            TaxDeductionComponent = 5,
        }
        public enum PayRollRunTypeConstants
        {
            OnPreRun = 1,
            CheckReview = 2,
            RunSuccessFull = 3,
        }
        #endregion
        public enum JobLevelsConstants
        {
            Use_Multiple_Levels_Of_Job_Function = 1,
            Assign_Weights_To_Competencies = 2,
        }

        public enum ObjectiveTypeConstants
        {
            Individual = 1,
            Department = 2,
            Company = 3,
        }
        public enum WhoCanSeeConstants
        {
            Managers = 1,
            EveryOne = 2,
        }
        public enum TagsConstants
        {

        }
        public enum ProgressConstants
        {
            Progress_Measured_As = 1,
            Percent_Complete = 2,
            Metric_Unit = 3,
        }




        public enum TransactionTypeConstants
        {
            Debit = 1,
            Credit = 2,
        }
        public enum PaymentModeConstants
        {
            Cash = 1,
            Cheque = 2,
            Online = 3,
        }

        public enum ProjectConstants
        {
            Bad = 1,
            Ok = 2,
            Good = 3,
        }


        /// <summary>
        /// Created By Suraj Bundel on 11-01-2023
        /// Its for todolist model for task to get task status 
        /// 
        /// </summary>
        public enum Todotaskstatus
        {
            Pending = 1,
            Done = 2,
        }
        public enum ProjectStatusConstants
        {
            Live = 1,
            Closed = 2,
            Hold = 3,
        }

        public enum DocumentTypeConstants
        {
            Sales_Document = 1,
            Pre_Sales_Document = 2,
            Agreement = 3,
            Contract = 4,
            SOW = 5,
            BRD = 6,
            SRD = 7,
            Architecture = 8,
            Other = 9
        }

    }
}