using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model;
using System.Collections.Generic;
using System.Linq;

namespace AspNetIdentity.WebApi.Services
{
    public class QuestionService
    {
        public ApplicationDbContext db;

        public QuestionService()
        {
            db = new ApplicationDbContext();
        }

        public void RemoveQuestionCategoryId(int id)
        {
            List<Questions> ques = db.Questions.Where(c => c.CategoryId == id).ToList();
            foreach (var q in ques)
            {
                if (q != null)
                {
                    db.Questions.Remove(q);
                    db.SaveChanges();
                }
            }
        }

        public List<Questions> GetQuestionsByCatgoryId(int id)
        {
            return db.Questions.Where(q => q.CategoryId == id).ToList();
        }
    }
}