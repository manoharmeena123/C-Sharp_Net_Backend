using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.Core.ViewModel.TsfViewModel
{
    public class ReponseTsfDashboradClass
    {
        public class ResponseEventClass
        {
            public string EventTitle { get; set; } = String.Empty;
            public string EventVenue { get; set; } = String.Empty;
            public string EventTimeStart { get; set; } = string.Empty;
            public string EventTimeEnd { get; set; } = string.Empty;
            public string EventMonth { get; set; } = String.Empty;
            public string EventDate { get; set; } = string.Empty;
        }
        public class ResponseBdayEmployeeClass
        {
            public int EmployeeId { get; set; }
            public string EmployeeName { get; set; }
            public DateTime? DOB { get; set; }
            public bool HideDOB { get; set; }
            public string DepartmentName { get; set; }
            public string UserImage { get; set; }
        }
        public class ResponseCompanyNewsClass
        {
            public Guid NewsId { get; set; }
            public string News { get; set; }
            public string Image { get; set; }
            public string NewsHeading { get; set; }
        }
        public class ResponseTsfDashboradClass
        {
            public List<ResponseEventClass> EventList { get; set; }
            public List<ResponseBdayEmployeeClass> CurrentBirthdatList { get; set; }
            public List<ResponseCompanyNewsClass> CompanyNewsList { get; set; }
            public List<GetLatestBlogDashboardResponse> TopBlogList { get; set; }
            public List<GetLatestNewsOnDashboardResponse> TopNewsList { get; set; }
            public List<string> ProfileImageList { get; set; }
        }
        public class GetLatestBlogDashboardResponse
        {
            public Guid BlogId { get; set; }
            public string Title { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string Image { get; set; } = string.Empty;
            public string CategoryName { get; set; } = string.Empty;
            public DateTimeOffset ActionDate { get; set; } = DateTimeOffset.UtcNow;
            public long LikeCount { get; set; } = 0;
            public bool IsLikedByUser { get; set; } = false;
        }
        public class GetLatestNewsOnDashboardResponse
        {
            public Guid NewsId { get; set; } = Guid.Empty;
            public string Title { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string Image { get; set; } = string.Empty;
            public DateTimeOffset CreatedOn { get; set; }
            public string CategoryName { get; set; } = string.Empty;
            public long LikeCount { get; set; } = 0;
            public bool IsLikedByUser { get; set; } = false;
        }
    }

}
