using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model;
using System.Collections.Generic;
using System.Linq;
using static AspNetIdentity.WebApi.Controllers.ProjectController;

namespace AspNetIdentity.WebApi.Services
{
    public class CategoryService
    {
        public ApplicationDbContext db;
        public QuestionService questionService;

        public CategoryService()
        {
            db = new ApplicationDbContext();
            questionService = new QuestionService();
        }

        public bool AddCategory(Category category)
        {
            try
            {
                db.Category.Add(category);
                db.SaveChanges();
                return true;
            }
            catch
            {
                throw;
                //  return BadRequest(ex.Message);
            }
        }

        public Category GetCategoryById(int id)
        {
            try
            {
                return db.Category.Where(c => c.CategoryId == id).FirstOrDefault();
            }
            catch
            {
                throw;
            }
        }

        public List<Category> GetAllCategories()
        {
            try
            {
                return db.Category.ToList();
            }
            catch
            {
                throw;
            }
        }

        public CategoryType GetCategoryTypeById(int id)
        {
            try
            {
                return db.CategoryType.Where(c => c.CategoryTypeId == id).FirstOrDefault();
            }
            catch
            {
                throw;
            }
        }

        public bool RemoveCategoryById(int id)
        {
            Category category = db.Category.Where(c => c.CategoryId == id).FirstOrDefault();
            if (category != null)
            {
                db.Category.Remove(category);
                db.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<DataCategory> GetCategoryByRoleId(int roleid)
        {
            List<DataCategory> DataCategorylistArray = new List<DataCategory>();

            var category = db.Category.Where(c => c.UsertypeId == roleid).ToList();
            foreach (var item in category)
            {
                DataCategory category1 = new DataCategory();
                category1.CategoryId = item.CategoryId;
                category1.CategoryName = item.CategoryName;
                category1.CategoryTypeId = item.CategoryTypeId;
                category1.RoleId = item.UsertypeId;

                var Questionlist = db.Questions.Where(c => c.CategoryId == item.CategoryId).ToList();
                List<Questions> QuestionlistArray = new List<Questions>();
                foreach (var list in Questionlist)
                {
                    Questions questions = new Questions();
                    questions.QuestionId = list.QuestionId;
                    questions.CategoryId = list.CategoryId;
                    questions.Question = list.Question;
                    questions.ScoreRating = 5;
                    QuestionlistArray.Add(questions);
                }
                category1.quenstions = QuestionlistArray;
                DataCategorylistArray.Add(category1);
            }

            return DataCategorylistArray;
        }

        public List<DataCategory> GetCategoryByCtype(int CategoryTypeId)
        {
            List<DataCategory> DataCategorylistArray = new List<DataCategory>();

            var category = db.Category.Where(c => c.CategoryTypeId == CategoryTypeId).ToList();
            foreach (var item in category)
            {
                List<Questions> QuestionlistArray = new List<Questions>();
                DataCategory category1 = new DataCategory();
                category1.CategoryId = item.CategoryId;
                category1.CategoryName = item.CategoryName;
                category1.CategoryTypeId = item.CategoryTypeId;
                category1.RoleId = item.UsertypeId;

                var Questionlist = db.Questions.Where(c => c.CategoryId == item.CategoryId).ToList();
                foreach (var list in Questionlist)
                {
                    Questions questions = new Questions();
                    questions.QuestionId = list.QuestionId;
                    questions.CategoryId = list.CategoryId;
                    questions.Question = list.Question;
                    questions.ScoreRating = 5;
                    QuestionlistArray.Add(questions);
                }
                category1.quenstions = QuestionlistArray;
                DataCategorylistArray.Add(category1);
            }
            return DataCategorylistArray;
        }

        /// <summary>
        /// Remove Teams
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool RemoveTeamsById(int id)
        {
            TeamType teamType = db.TeamType.Where(c => c.TeamTypeId == id).FirstOrDefault();
            if (teamType != null)
            {
                db.TeamType.Remove(teamType);
                db.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<CategoryType> GetAllCategoryType()
        {
            return db.CategoryType.ToList();
        }

        public Role GetRoleByCategoryId(int id)
        {
            return db.Role.Where(r => r.CategoryTypeId == id).FirstOrDefault();
        }

        public List<Category> GetCategoriesByTypeId(int id)
        {
            return db.Category.Where(c => c.CategoryTypeId == id).ToList();
        }

        public Category GetCategoryByTypeId(int id)
        {
            return db.Category.Where(c => c.CategoryTypeId == id).FirstOrDefault();
        }

        public List<CategoryFilterDTO> GetAllCategory()
        {
            var catType = GetAllCategoryType();

            List<CategoryFilterDTO> CategoryDatas = new List<CategoryFilterDTO>();
            foreach (var re in catType)
            {
                var rol = GetRoleByCategoryId(re.CategoryTypeId);
                var cats = GetCategoriesByTypeId(re.CategoryTypeId);
                foreach (var c in cats)
                {
                    var x = new CategoryFilterDTO();
                    x.CategoryId = c.CategoryId;
                    x.CategoryName = c.CategoryName;
                    x.CategoryTypeId = re.CategoryTypeId;
                    x.Category_Type = re.Category_Type;
                    if (rol != null)
                    {
                        x.RoleId = rol.RoleId;
                        x.RoleType = rol.RoleType;
                    }

                    var ques = questionService.GetQuestionsByCatgoryId(c.CategoryId);

                    if (ques.Count != 0)
                    {
                        x.UpdatedDate = ques.Max(m => m.UpdatedDate);
                        x.Questions = ques;
                    }
                    CategoryDatas.Add(x);
                }
            }
            return CategoryDatas;
        }

        public List<CategoryFilterDTO> FilterCategoryById(List<CategoryFilterDTO> Categories, int id)
        {
            List<CategoryFilterDTO> data = new List<CategoryFilterDTO>();
            foreach (var cats in Categories)
            {
                if (cats.CategoryId == id)
                {
                    data.Add(cats);
                }
            }
            return data;
        }

        public List<CategoryFilterDTO> FilterCategoryByTypeId(List<CategoryFilterDTO> Categories, int id)
        {
            List<CategoryFilterDTO> data = new List<CategoryFilterDTO>();
            foreach (var cats in Categories)
            {
                if (cats.CategoryTypeId == id)
                {
                    data.Add(cats);
                }
            }
            return data;
        }

        public List<CategoryFilterDTO> FilterCategoryByRoleId(List<CategoryFilterDTO> Categories, int id)
        {
            List<CategoryFilterDTO> data = new List<CategoryFilterDTO>();
            foreach (var cats in Categories)
            {
                if (cats.RoleId == id)
                {
                    data.Add(cats);
                }
            }
            return data;
        }

        public List<CategoryFilterDTO> FilterCategory(List<CategoryFilterDTO> Categories)
        {
            var dataCategories = GetAllCategory();
            List<CategoryFilterDTO> data = new List<CategoryFilterDTO>();
            foreach (var c in Categories)
            {
                if (c.CategoryId != 0 && c.CategoryTypeId != 0 && c.RoleId != 0)
                {
                    data = FilterCategoryById(dataCategories, c.CategoryId);
                }
            }
            return data;
        }
    }
}