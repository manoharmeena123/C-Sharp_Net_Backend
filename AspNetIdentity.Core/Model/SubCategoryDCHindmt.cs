using AngularJSAuthentication.Model;
using System.Collections.Generic;

namespace AngularJSAuthentication.DataContracts.Masters
{
    public class SubCategoryDCHindmt
    {
        public class resSubCategoryDTO
        {
            public string Message { get; set; }
            public bool Flag { get; set; }
            public List<SubCategoryHindmt> data { get; set; }
        }

        public class SubCategoryvenDTO
        {
            public string Message { get; set; }
            public bool Flag { get; set; }
            public List<SubCategoryvenDTOlist> List { get; set; }
        }

        public class SubCategoryvenDTOlist
        {
            public int SubCategoryId { get; set; }
            public string SubCategoryName { get; set; }
            public string SubCategoryHindiName { get; set; }
            public int CategoryId { get; set; }
            public string LogoUrl { get; set; }
        }
    }
}