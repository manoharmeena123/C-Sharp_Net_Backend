using AspNetIdentity.Core.Model.TicketsModel;
using AspNetIdentity.Core.Model.TicketsModel.NewTicketEntities;
using AspNetIdentity.Core.Model.TsfModule;
using AspNetIdentity.Core.Model.TsfModule.CircularsNotices;
using AspNetIdentity.Core.Model.TsfModule.NewsEntities;
using AspNetIdentity.Core.Model.UserAccesPermission;
using AspNetIdentity.WebApi.Controllers.Test;
using AspNetIdentity.WebApi.Controllers.TimeSheet;
using AspNetIdentity.WebApi.Model;
using AspNetIdentity.WebApi.Model.AssetsModel;
using AspNetIdentity.WebApi.Model.AttendanceModel;
using AspNetIdentity.WebApi.Model.ClientsModel;
using AspNetIdentity.WebApi.Model.CloudSofwareLicenes;
using AspNetIdentity.WebApi.Model.EmossyWallModel;
using AspNetIdentity.WebApi.Model.EmployeeModel;
using AspNetIdentity.WebApi.Model.EventModel;
using AspNetIdentity.WebApi.Model.ExitsModel;
using AspNetIdentity.WebApi.Model.ExtraMasterModel;
using AspNetIdentity.WebApi.Model.FaultyImportLog;
using AspNetIdentity.WebApi.Model.Feedback;
using AspNetIdentity.WebApi.Model.GiftsModel;
using AspNetIdentity.WebApi.Model.GoalManagement;
using AspNetIdentity.WebApi.Model.Header;
using AspNetIdentity.WebApi.Model.HiringQuestion;
using AspNetIdentity.WebApi.Model.HolidayModel;
using AspNetIdentity.WebApi.Model.Leave;
using AspNetIdentity.WebApi.Model.LeaveComponent;
using AspNetIdentity.WebApi.Model.LeaveMasterModel;
using AspNetIdentity.WebApi.Model.MailTemplate;
using AspNetIdentity.WebApi.Model.New_Pay_Roll;
using AspNetIdentity.WebApi.Model.New_Pay_Roll_Run_Model;
using AspNetIdentity.WebApi.Model.NewClientRequirement;
using AspNetIdentity.WebApi.Model.NewClientRequirement.Clienttask;
using AspNetIdentity.WebApi.Model.NewClientRequirement.FaultyLogs;
using AspNetIdentity.WebApi.Model.NewClientRequirement.TypeofWork;
using AspNetIdentity.WebApi.Model.NewDashboard;
using AspNetIdentity.WebApi.Model.NewUserAttendance;
using AspNetIdentity.WebApi.Model.Payment;
using AspNetIdentity.WebApi.Model.Performence;
using AspNetIdentity.WebApi.Model.ProjectMaster;
using AspNetIdentity.WebApi.Model.Reviews;
using AspNetIdentity.WebApi.Model.ShiftModel;
using AspNetIdentity.WebApi.Model.SkillModel;
using AspNetIdentity.WebApi.Model.SmtpModule;
using AspNetIdentity.WebApi.Model.Tax_Master;
using AspNetIdentity.WebApi.Model.Teams;
using AspNetIdentity.WebApi.Model.TicketsModel;
using AspNetIdentity.WebApi.Model.TimeSheet;
using AspNetIdentity.WebApi.Model.TimeSheet.History;
using AspNetIdentity.WebApi.Model.ToDoList_Module;
using AspNetIdentity.WebApi.Model.UserAccesPermission;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace AspNetIdentity.WebApi.Infrastructure
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IDisposable
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            Configuration.ProxyCreationEnabled = false;
            Configuration.LazyLoadingEnabled = false;
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        #region New Ticket Db Entity Model
        public DbSet<TicketCategoryEntity> TicketCategoryEntities { get; set; }
        public DbSet<PrioritiesEntity> PrioritiesEntities { get; set; }
        public DbSet<CategoriesEmployeeEntity> CategoriesEmployeeEntities { get; set; }

        #endregion

        public DbSet<TestCKEditor> testCKEditors { get; set; }
        public DbSet<EmployeeExitsHistory> EmployeeExitsHistorys { get; set; }
        public DbSet<ResignationResigon> ResignationResigons { get; set; }
        public DbSet<UserComment> UserComments { get; set; }
        public DbSet<AssetsItemMaster> AssetsItemMasters { get; set; }
        public DbSet<AssetImportFaultyLogsGoups> AssetImportFaultieGroups { get; set; }
        public DbSet<AssetImportFaultyLogs> AssetImportFaultieLogs { get; set; }
        public DbSet<DesignationImportFaultyLogsGoups> DesignationImportFaultieLogsGoups { get; set; }
        public DbSet<HomeHeader> HomeHeaders { get; set; }
        public DbSet<SecondHeader> SecondHeaders { get; set; }
        public DbSet<ThirdHeader> ThirdHeaders { get; set; }
        public DbSet<Attendencereport> Attendencereports { get; set; }
        public DbSet<AttendanceImportFaultyLogs> AttendanceImportFaultiesLogs { get; set; }
        public DbSet<AttendanceImportFaultyLogsGroup> AttendanceImportFaultyLogsGoups { get; set; }
        public DbSet<DepartmentImportFaultyLogsGoups> DepartmentImportFaultyLogsGoups { get; set; }

        public DbSet<CloudSoftwareLicense> CloudSoftwareLicenses { get; set; }
        public DbSet<Group> Groups { get; set; }

        public DbSet<GroupUser> GPSUser { get; set; }
        public DbSet<Postwall> PostWalls { get; set; }
        public DbSet<EmployeeImages> EmpImages { get; set; }

        // project Expense Master open
        public DbSet<ProjectExpenseMaster> ProjectExpenseMasters { get; set; }

        public DbSet<ProjectExpCostAmt> ProjectExpCostAmts { get; set; }

        //project Expense Master close
        public DbSet<ClientLead> ClientLeads { get; set; }
        public DbSet<CompanyNews> CompanyNews { get; set; }

        public DbSet<AddUserRatingSkill> AddUserRatingSkill { get; set; }
        public DbSet<GSeatCost> GSeatCosts { get; set; }
        public DbSet<GiftBaseCategory> GiftBaseCategories { get; set; }
        public DbSet<GiftCategory> GiftCategories { get; set; }
        public DbSet<GiftBaseBaseCategory> GiftBaseBaseCategories { get; set; }
        public DbSet<GiftItemCategory> GiftItemCategories { get; set; }
        public DbSet<GiftItem> GiftItems { get; set; }
        public DbSet<GiftAssign> GiftAssigns { get; set; }
        public DbSet<TravelExpense> TravelExpenses { get; set; }
        public DbSet<RelationOfEmp> RelationOfEmp { get; set; }
        public DbSet<FaultyAssetsData> FaultyAssetsData { get; set; }
        public DbSet<SkillRequest> SkillRequests { get; set; }
        public DbSet<TeamType> TeamType { get; set; }
        public DbSet<PolicyGroup> PolicyGroups { get; set; }
        public DbSet<EmployeePolicyGroup> EmployeePolicyGroups { get; set; }
        public DbSet<Region> Region { get; set; }
        public DbSet<FeedbackScore> FeedbackScore { get; set; }
        public DbSet<BillType> BillType { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<CategoryType> CategoryType { get; set; }
        public DbSet<CompanyPolicy> CompanyPolicys { get; set; }
        public DbSet<Questions> Questions { get; set; }
        public DbSet<FeedbackMaster> Feedbacks { get; set; }
        public DbSet<Endorsement> Endorsements { get; set; }
        public DbSet<EmployeeBadges> EmpBadges { get; set; }
        public DbSet<CandidateDoc> candidateDocs { get; set; }
        public DbSet<CredentilData> CredentilDatas { get; set; }
        public DbSet<City> City { get; set; }
        public DbSet<Country> Country { get; set; }
        public DbSet<Currency> Currency { get; set; }
        public DbSet<Education> Education { get; set; }
        public DbSet<Department> Department { get; set; }

        public DbSet<Holiday> Holidays { get; set; }
        public DbSet<EmployeeExits> EmployeeExits { get; set; }
        public DbSet<ExpenseEntry> ExpenseEntry { get; set; }
        public DbSet<PostWallHistory> PostWallHistorys { get; set; }
        public DbSet<QuickLinks> QuickLinks { get; set; }
        public DbSet<OrgMaster> OrgMaster { get; set; }
        public DbSet<Designation> Designation { get; set; }
        public DbSet<MenuItem> MenuItem { get; set; }
        public DbSet<WorkFromHome> WorkFromHomes { get; set; }
        public DbSet<CompanyPolicyHistory> CompanyPolicyHistorys { get; set; }
        public DbSet<WfhNofifyByEmployee> WfhNotify { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<AssignMasterData> AssignMasterData { get; set; }
        public DbSet<AboutUs> AboutUs { get; set; }
        public DbSet<Benefits> Benefits { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Reaction> Reactions { get; set; }
        public DbSet<WorkingAt> WorkingAt { get; set; }
        public DbSet<Tools> Tools { get; set; }
        public DbSet<Condition> Condition { get; set; }
        public DbSet<Goal> Goals { get; set; }
        public DbSet<CommentHistory> CommentHistorys { get; set; }
        public DbSet<Experience> Experience { get; set; }
        public DbSet<NewProjectMaster> NewProjectMasters { get; set; }
        public DbSet<NewAssignProject> NewAssignProjects { get; set; }
        public DbSet<ProjectList> ProjectLists { get; set; }
        public DbSet<AssignProject> AssignProjects { get; set; }
        public DbSet<Project> Project { get; set; }
        public DbSet<NewTimeSheet> NewTimeSheets { get; set; }
        public DbSet<ProjectType> ProjectTypes { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<State> State { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<Team> Team { get; set; }
        public DbSet<Technology> Technology { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<ClientContact> ClientContacts { get; set; }
        public DbSet<Resource> Resource { get; set; }
        public DbSet<ResourceCompany> ResourceCompany { get; set; }
        public DbSet<ProjectTechnology> ProjectTechnology { get; set; }
        public DbSet<InterviewRound> InterviewRound { get; set; }
        public DbSet<Company> Company { get; set; }
        public DbSet<CompanyLegalEntity> CompanyLegalEntities { get; set; }
        public DbSet<Area> Area { get; set; }
        public DbSet<URLTracking> URLTrackings { get; set; }
        public DbSet<CategoryURL> CategoryURLs { get; set; }
        public DbSet<CoreValue> CoreValues { get; set; }
        public DbSet<EmployeeTaskReminderHistory> TaskReminder { get; set; }
        public DbSet<EventModel> EventModels { get; set; }

        #region Blogs DB Set 
        public DbSet<Blogs> Blogs { get; set; }
        public DbSet<BlogCategories> BlogCategories { get; set; }
        public DbSet<BlogsLikesEntity> BlogsLikesEntities { get; set; }
        #endregion

        #region News DB Set
        public DbSet<NewsEntity> NewsEntities { get; set; }
        public DbSet<NewsCategoryEntity> NewsCategories { get; set; }
        public DbSet<NewsLikeEntity> NewsLikeEntities { get; set; }
        public DbSet<CircularsNoticesEntities> CircularsNoticesEntities { get; set; }
        #endregion

        #region POST WALL DB SET 
        public DbSet<UserWall> UserWalls { get; set; }
        public DbSet<PostMention> PostMentions { get; set; }
        public DbSet<PostLike> PostLikes { get; set; }
        public DbSet<PostComment> PostComments { get; set; }
        #endregion

        #region Employee DB Set

        public DbSet<Employee> Employee { get; set; }
        public DbSet<EmployeeDocument> EmpDoc { get; set; }


        #endregion Employee DB Set

        #region All Ticket DB Set

        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketComment> TicketComments { get; set; }
        public DbSet<TicketCategory> TicketCategories { get; set; }
        public DbSet<TicketCategoryPriority> TicPriorities { get; set; }
        public DbSet<TicketCategoryEmployee> TicketCategoryEmployees { get; set; }

        #endregion All Ticket DB Set

        #region All Tax Db Set

        public DbSet<IncomeSlab> IncomeSlabs { get; set; }
        public DbSet<ProfessionalTax> ProfessionalTaxes { get; set; }
        public DbSet<ProfessionalTaxGroup> PTGroup { get; set; }
        public DbSet<ProfessionalTaxState> PTState { get; set; }
        public DbSet<ProfessionalTaxRange> PTRange { get; set; }

        #endregion App Tax Db Set

        #region All Feedback DB Set

        public DbSet<FeedbackCategory> FeedbackCategories { get; set; }
        public DbSet<FeedbackQuestions> FeedbackQuestions { get; set; }
        public DbSet<EmployeeFeedback> EmployeeFeedbacks { get; set; }
        public DbSet<EmployeeFeeedBackQuestion> EmployeeFeeedBackQuestions { get; set; }

        #endregion All Feedback DB Set

        #region All Team DB Set

        public DbSet<TeamMaster> TeamMasters { get; set; }
        public DbSet<TeamMembers> TeamMembers { get; set; }

        #endregion All Team DB Set

        #region All Extra Master DB Set 
        public DbSet<Sector> Sectors { get; set; }
        public DbSet<TypeOfBusiness> TypeOfBusinesses { get; set; }
        public DbSet<NatureOfBusiness> NatureOfBusinesses { get; set; }
        #endregion

        #region New Goal DB Set

        public DbSet<GoalManagement> GoalManagements { get; set; }
        public DbSet<GoalsPermission> GoalsPermissions { get; set; }
        public DbSet<ReviewersInGoal> ReviewersInGoals { get; set; }
        public DbSet<GoalsDocument> GoalsDocuments { get; set; }
        public DbSet<GoalComment> GoalComments { get; set; }

        #endregion New Goal DB Set

        #region All DB Set For Pay Group And Pay Group Setting
        public DbSet<PayGroup> PayGroups { get; set; }
        public DbSet<PayGroupSetup> PayGroupSetups { get; set; }

        /// <summary> ----------------- Settings ------------------ </summary>
        public DbSet<CompanyInformation> CompanyInfos { get; set; }
        public DbSet<CompanyInfoBank> CompanyInfoBanks { get; set; }
        public DbSet<CompanyInfoLocation> CompanyInfoLocations { get; set; }
        public DbSet<GeneralPayrollSetting> GeneralPayrollSettings { get; set; }
        public DbSet<PFSettings> PFSettings { get; set; }
        public DbSet<ESISetting> ESISettings { get; set; }
        public DbSet<ComponentInPayGroup> ComponentInPays { get; set; }
        public DbSet<StatutoryFlling> StatutoryFllings { get; set; }

        #endregion

        #region All DB Set For Pay Roll Structure 
        public DbSet<SalaryStructure> SalaryStructures { get; set; }
        public DbSet<SalaryStructureConfig> SalaryStructureConfigs { get; set; }

        #endregion

        #region ALL DB Set for Pay Roll Component
        public DbSet<RecuringComponent> RecuringComponents { get; set; }
        public DbSet<AdHocComponent> AdHocComponents { get; set; }
        public DbSet<TaxDeductionComponent> TaxDeductions { get; set; }

        #endregion

        #region Pay Roll Calculation DB Set
        public DbSet<SalaryBreakDown> SalaryBreakDowns { get; set; }
        public DbSet<RunPayRoll> RunPayRolls { get; set; }
        public DbSet<RunPayRollGroup> RunPayRollGroups { get; set; }

        #endregion


        #region LEAVE DB SET MODEL CLASS

        //public DbSet<LeaveTypeModel> LeaveTypeModels { get; set; }

        #endregion

        #region Db Set For Holidays

        public DbSet<HolidayGroup> HolidayGroups { get; set; }
        public DbSet<HolidayModel> HolidayModels { get; set; }
        public DbSet<HolidayInGroup> HolidayInGroups { get; set; }
        #endregion

        #region UI Access DB Set



        #endregion UI Access DB Set

        #region All Asesets DB Set

        public DbSet<AssetIcon> AssetIcons { get; set; }
        public DbSet<AssetsBaseCategory> AssetsBaseCategories { get; set; }
        public DbSet<AssetsCategory> AssetsCategories { get; set; }

        //public DbSet<AssetsItemMaster> AssetsItemMaster { get; set; }
        public DbSet<AssetsHistory> AssetsHistories { get; set; }
        public DbSet<AsstesAssignHistory> AsstesAssignHistories { get; set; }

        public DbSet<AssetsWarehouse> AssetsWarehouses { get; set; }
        public DbSet<Compliance> Compliances { get; set; }

        #endregion All Asesets DB Set

        #region Leave DB Set

        public DbSet<LeaveGroup> LeaveGroups { get; set; }
        public DbSet<LeaveType> LeaveTypes { get; set; }
        public DbSet<LeaveComponent> LeaveComponents { get; set; }
        public DbSet<GlobalLeaveYearHistory> GlobalLeave { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<LeaveRequestHistory> LeaveReqHistories { get; set; }


        #endregion Leave DB Set

        #region Shift And Attendence DB Set
        public DbSet<ShiftGroup> ShiftGroups { get; set; }
        public DbSet<ShiftTiming> ShiftTimings { get; set; }

        #endregion Shift And Attendence DB Set

        #region Employee Exits

        public DbSet<NoticePeriodSetting> NoticePeriods { get; set; }

        #endregion Employee Exits

        #region Notification DB Set

        public DbSet<FireBase> FireBases { get; set; }

        #endregion Notification DB Set

        #region SmtpModel Class
        public DbSet<CompanySmtpMailModel> CompanySmtpMailModels { get; set; }
        #endregion

        #region Hiring DB Set 
        public DbSet<JobPost> JobPosts { get; set; }
        public DbSet<JobPostTemplate> JobTemplates { get; set; }
        public DbSet<HiringTeam> HiringTeams { get; set; }
        public DbSet<StageStatus> StageStatuses { get; set; }
        public DbSet<Source> Sources { get; set; }
        public DbSet<StageMaster> Stages { get; set; }
        public DbSet<HiringFlowMaster> HiringFlow { get; set; }
        public DbSet<DocTypeMaster> DocumentTypes { get; set; }
        public DbSet<RequiredDocuments> RequiredDocs { get; set; }
        public DbSet<RequiredDocMaster> RequiredDocMaster { get; set; }
        public DbSet<CandidateDocuments> CandidateDocuments { get; set; }
        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<HiringStage> HiringStages { get; set; }
        public DbSet<PrebordingDocument> prebordingDocuments { get; set; }

        public DbSet<RequestCandidate> RequestCandidates { get; set; }
        public DbSet<RevokeReason> RevokeReasons { get; set; }

        #endregion

        #region This Db Use To Add Question In Job
        public DbSet<HiringQuestionsBank> HiringQuestionsBanks { get; set; }
        public DbSet<HiringQuesionsAndAnsBank> HiringQuesionsAndAnsBanks { get; set; }
        #endregion

        #region Faulty Logs DB Set
        public DbSet<EmployeeImportFaultyLog> EmployeeFaultyLogs { get; set; }
        public DbSet<EmployeeImportFaultyLogGroup> EmployeeFaultyLogGroups { get; set; }

        #endregion

        #region All UserAccess Permission DB Set
        public DbSet<ModuleAndSubmodule> ModuleAndSubmodules { get; set; }
        public DbSet<CompaniesModuleAccess> CompaniesModuleAccesses { get; set; }
        public DbSet<RoleInUserAccessPermission> RoleInUserAccessPermissions { get; set; }
        public DbSet<PermissionInUserAccess> PermissionInUserAccesses { get; set; }
        public DbSet<EmployeeInRole> EmployeeInRoles { get; set; }
        #endregion

        #region DB Set For New User Attendance
        public DbSet<UserAttendanceModel> UserAttendances { get; set; }

        #endregion

        //Mayank ---------------------------------------------------------

        #region All DB Set For Performance 
        public DbSet<AddMultipleEmployee> AddMultipleEmployees { get; set; }
        public DbSet<Behaviour> Behaviours { get; set; }
        public DbSet<CommonSuccessCompetency> CommonSuccessCompetencys { get; set; }
        public DbSet<CoreCompetency> CoreCompetencys { get; set; }
        public DbSet<DepartmentJobFuntion> DepartmentJobFuntions { get; set; }
        public DbSet<EmployeeReview> EmployeeReviews { get; set; }
        public DbSet<FeedBackResponse> FeedBackFroms { get; set; }
        public DbSet<JobFunctionCompencies> JobFunctionCompencies { get; set; }
        public DbSet<JobFuntion> JobFuntions { get; set; }
        public DbSet<JobSpecificCompetency> JobSpecificCompetencys { get; set; }
        public DbSet<ObjectiveModel> ObjectiveModels { get; set; }
        public DbSet<OptionSelect> OptionSelects { get; set; }
        public DbSet<RatingScales> RatingScalies { get; set; }
        public DbSet<RatingScore> RatingScores { get; set; }
        public DbSet<RequestFeedback> RequestFeedbacks { get; set; }
        public DbSet<ReviewCoreValue> ReviewCoreValues { get; set; }
        public DbSet<ReviewCycleGroup> ReviewCycleGroups { get; set; }
        public DbSet<ReviewQuestion> ReviewQuestions { get; set; }
        public DbSet<ReviewsGroup> ReviewsGroups { get; set; }
        public DbSet<WeightAge> WeightAges { get; set; }
        #endregion
        public DbSet<PaymentModel> PaymentModels { get; set; }
        public DbSet<ProjectFeedback> ProjectFeedbacks { get; set; }
        public DbSet<PMO> PMOs { get; set; }
        public DbSet<ClientTecnology> ClientTecnologys { get; set; }

        public DbSet<CtTecnology> CtTecnologys { get; set; }
        //Harshit ---------------------------------------------------------
        public DbSet<Location> Locations { get; set; }
        public DbSet<Notification> NotificationDb { get; set; }
        public DbSet<FAQCategories> FAQCategoriess { get; set; }
        public DbSet<FAQCategoriesQAns> FAQCategoriesQAnss { get; set; }
        public DbSet<UserAttendanceLog> UserAttendancesLog { get; set; }
        public DbSet<WeekOffDaysCases> WeekOffDaysCases { get; set; }
        public DbSet<WeekOffDaysGroup> WeekOffDays { get; set; }
        public DbSet<SkillMaster> Skills { get; set; }
        public DbSet<SkillGroup> SkillGroups { get; set; }
        public DbSet<SkillsInSkillsGroup> SkillIn { get; set; }
        public DbSet<Competency> Competencies { get; set; }
        public DbSet<CompetencyType> CompetencyTypes { get; set; }
        public DbSet<CompentencyBehaviours> CompentencyBehaviours { get; set; }
        public DbSet<CoreValueBehaviour> CoreValueBehaviours { get; set; }
        public DbSet<CandidateInterview> CandidateInterviews { get; set; }
        public DbSet<MeetingScheduleInterview> meetingScheduleInterviews { get; set; }
        public DbSet<HiringTemplate> HiringTemplates { get; set; }

        //Harshit -----------------------------------------------------------
        public DbSet<BankMaster> BankMaster { get; set; }


        #region TimeSheet Db

        public DbSet<TaskModel> TaskModels { get; set; }
        public DbSet<Sprint> Sprints { get; set; }
        public DbSet<TaskLog> TaskLogs { get; set; }
        public DbSet<TaskApprovel> TaskApprovels { get; set; }
        public DbSet<TaskImportFaultyLogs> TaskImportFaultyLogs { get; set; }
        public DbSet<TaskImportFaultyLogsGroup> TaskImportFaultyLogsGroups { get; set; }
        public DbSet<TaskPermissions> TaskPermissions { get; set; }
        public DbSet<TaskComment> TaskComments { get; set; }
        public DbSet<TaskMentionEmployee> TaskMentionEmployees { get; set; }
        public DbSet<ProjectDocument> ProjectDocuments { get; set; }
        public DbSet<TaskModelHistory> TaskModelHistories { get; set; }
        public DbSet<EmployeeRoleInProject> EmployeeRoleInProjects { get; set; }
        #endregion

        #region Helper Functions

        /// <summary>
        /// Created By Harshit Mitra On 15-07-2022
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        public string GetEmployeeNameById(int employeeId)
        {
            if (employeeId == 0)
            {
                return "Default Created";
            }
            return Employee.Where(x => x.EmployeeId == employeeId).Select(x => x.DisplayName).FirstOrDefault();
        }

        #endregion Helper Functions

        public DbSet<ItemMasterBaseCategory> ItemMasterBaseCategory { get; set; }
        public DbSet<ItemMasterCategory> ItemMasterCategory { get; set; }
        public DbSet<ItemMasterSubCategory> itemMasterSubCategory { get; set; }
        public DbSet<ItemMasterWarehouse> ItemMasterWarehouses { get; set; }
        public DbSet<ItemMaster> ItemMaster { get; set; }
        public DbSet<ItemHistory> ItemHistoryes { get; set; }
        public DbSet<Purches> Purches { get; set; }

        #region ToDoListModule
        public DbSet<ToDoListModel> ToDoListModels { get; set; }


        #endregion

        #region New Client Requirement

        public DbSet<NewClientModel> NewClientModels { get; set; }
        public DbSet<TypeofWork> TypeofWorks { get; set; }
        public DbSet<TypeofWorkHistory> TypeofWorkHistories { get; set; }
        public DbSet<NewClientHistory> NewClientHistories { get; set; }
        public DbSet<TypeofWorkImportFaultyLogsGroup> TypeofWorkImportFaultyLogsGroups { get; set; }
        public DbSet<TypeofWorkImportFaultyLog> TypeofWorkImportFaultyLogs { get; set; }
        public DbSet<NewclientImportFaultyLog> NewclientImportFaultyLogs { get; set; }
        public DbSet<NewclientImportFaultyLogGroup> NewclientImportFaultyLogGroups { get; set; }
        public DbSet<AssignWorktype> AssignWorktypes { get; set; }
        public DbSet<clientTaskModel> clientTaskModels { get; set; }
        public DbSet<clientTaskModelHistory> clientTaskModelHistorys { get; set; }
        public DbSet<clientTaskImportFaultyLog> clientTaskImportFaultyLogs { get; set; }
        public DbSet<clientTaskImportFaultyLogGroup> clientTaskImportFaultyLogGroups { get; set; }
        public DbSet<ClientTaskApproval> ClientTaskApprovals { get; set; }


        #endregion

        public ItemMasterCategory AddItemMasterCategory(ItemMasterCategory category)
        {
            List<ItemMasterCategory> cat = ItemMasterCategory.Where(c => c.IsDeleted == false && c.CategoryName.Trim().Equals(category.CategoryName.Trim())).ToList();
            ItemMasterCategory objcat = new ItemMasterCategory();
            if (cat.Count == 0)
            {
                category.CreatedBy = objcat.CreatedBy;
                category.CreatedOn = DateTime.Now;
                category.UpdatedOn = DateTime.Now;
                category.IsActive = true;
                category.IsDeleted = false;
                ItemMasterCategory.Add(category);
                this.SaveChanges();
                this.Entry(category).State = System.Data.Entity.EntityState.Modified;
                int id = this.SaveChanges();
                return category;
            }
            else
            {
                return objcat;
            }
        }

        public ItemMasterSubCategory AddItemMasterSubCategory(ItemMasterSubCategory category)
        {
            List<ItemMasterSubCategory> cat = itemMasterSubCategory.Where(c => c.IsDeleted == false && c.SubcategoryName.Trim().Equals(category.SubcategoryName.Trim())).ToList();
            ItemMasterSubCategory objcat = new ItemMasterSubCategory();
            if (cat.Count == 0)
            {
                category.CreatedBy = objcat.CreatedBy;
                category.CreatedOn = DateTime.Now;
                category.UpdatedOn = DateTime.Now;
                category.IsActive = true;
                category.IsDeleted = false;
                itemMasterSubCategory.Add(category);
                this.SaveChanges();
                this.Entry(category).State = System.Data.Entity.EntityState.Modified;
                int id = this.SaveChanges();
                return category;
            }
            else
            {
                return objcat;
            }
        }

    }
}