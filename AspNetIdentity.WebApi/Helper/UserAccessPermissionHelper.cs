using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model.UserAccesPermission;
using AspNetIdentity.WebApi.Models;
using System;
using System.Collections.Generic;
using static AspNetIdentity.WebApi.Controllers.UIAccess.UserAccessPermissionController;

namespace AspNetIdentity.WebApi.Helper
{
    public class UserAccessPermissionHelper
    {
        #region Add Module List

        /// <summary>
        /// Created by Ankit jain on 16-11-2022
        /// </summary>
        /// <returns></returns>
        public static object AddModuleLisDatat()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            ApplicationDbContext db = new ApplicationDbContext();
            List<AddGetModuleClass> getmoduleList = GetSuperAdminModuleClass();
            for (int i = 0; i < getmoduleList.Count; i++)
            {
                List<AddGetSubModuleClass> getsubmodule = getmoduleList[i].SubModule;
                if (getsubmodule.Count > 0)
                {
                    for (int j = 0; j < getsubmodule.Count; j++)
                    {
                        ModuleAndSubmodule obj = new ModuleAndSubmodule();
                        obj.Id = Guid.NewGuid();
                        obj.ModuleName = getmoduleList[i].ModuleName;
                        obj.SubModuleName = getsubmodule[j].SubModuleName;
                        obj.ModuleCode = getmoduleList[i].ModuleCode;
                        obj.SubModuleCode = getsubmodule[j].SubModuleCode;
                        obj.IsSuperAdmin = true;
                        db.ModuleAndSubmodules.Add(obj);
                        db.SaveChanges();
                        res.Message = "method run";
                        res.Status = true;
                        res.Data = obj;
                    }
                }
                else
                {
                    ModuleAndSubmodule obj = new ModuleAndSubmodule();
                    obj.Id = Guid.NewGuid();
                    obj.ModuleName = getmoduleList[i].ModuleName;
                    obj.SubModuleName = null;
                    obj.ModuleCode = getmoduleList[i].ModuleCode;
                    obj.SubModuleCode = null;
                    obj.IsSuperAdmin = true;
                    db.ModuleAndSubmodules.Add(obj);
                    db.SaveChanges();
                    res.Message = "method run";
                    res.Status = true;
                    res.Data = obj;

                }
            }
            return res.Data;
        }

        #endregion Add Module List
    }
}