using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model;
using AspNetIdentity.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace AspNetIdentity.WebApi.Services
{
    public class ExpensesService
    {
        public ApplicationDbContext db;

        public ExpensesService()
        {
            db = new ApplicationDbContext();
        }

        public List<ExpenseEntry> EmployeeExpensesFilterByDate(int Id, DateTime startDate, DateTime endDate)
        {
            List<ExpenseEntry> expenseEntries = db.ExpenseEntry.Where(x => x.EmployeeId == Id &&
             x.ExpenseDate >= startDate && x.ExpenseDate <= endDate).ToList();
            return expenseEntries;
            //if (expenseEntries == null)
            //  return null;
        }

        public ExpensesDTOs GetEmployeeExpenses(int Id, DataHelperResponse dateHelperDTO)
        {
            ExpensesDTOs test = new ExpensesDTOs();
            var emp = EmployeeExpensesFilterByDate(Id, dateHelperDTO.startDate, dateHelperDTO.endDate);//db.ExpenseEntry.Where(x => x.EmployeeId == Id).ToList();
            var TotalSalaryMS = emp.GroupBy(p => p.ExpenseTitle, q => q.ExpenseAmount).Select(x => new
            {
                ExpenseTitle = x.Key,
                ExpenseAmount = x.Sum(s => s)
            }
            ).ToList();

            List<ExpensesDTOs> expenses = new List<ExpensesDTOs>();
            //  foreach (var x in TotalSalaryMS)
            //{
            //  ExpensesDTO expense = new ExpensesDTO();
            //  expense.ExpenseTitle = x.ExpenseTitle;
            // // expense.ExpenseAmount = x.ExpenseAmount;
            //  expenses.Add(expense);
            //}
            if (TotalSalaryMS.Count > 0)
            {
                foreach (var x in TotalSalaryMS)
                {
                    if (x.ExpenseAmount != 0)
                    {
                        test.expenseTitle.Add(x.ExpenseTitle);
                        test.expenseAmount.Add(x.ExpenseAmount);
                    }
                    else
                    {
                        test.expenseTitle.Add("No Expense");
                        test.expenseAmount.Add(1);
                    }
                }
            }
            else
            {
                test.expenseTitle.Add("No Expense");
                test.expenseAmount.Add(1);
            }

            //test.ExpenseAmount = x.ExpenseAmount;

            return test;
        }

        public class ExpensesDTOs
        {
            public List<string> expenseTitle { get; set; } = new List<string>();
            public List<double> expenseAmount { get; set; } = new List<double>();
        }

        public IEnumerable<ExpenseDTO> GetAllEmployeeExpenses()
        {
            var emp = db.ExpenseEntry.ToList();
            var TotalSalaryMS = emp.GroupBy(p => p.ExpenseTitle, q => q.ExpenseAmount).Select(x => new
            {
                ExpenseTitle = x.Key,
                ExpenseAmount = x.Sum(s => s)
            }
            ).ToList();
            List<ExpenseDTO> expenses = new List<ExpenseDTO>();
            foreach (var x in TotalSalaryMS)
            {
                ExpenseDTO expense = new ExpenseDTO();
                expense.name = x.ExpenseTitle;
                expense.value = x.ExpenseAmount;
                expenses.Add(expense);
            }
            return expenses;
        }

        public class ExpenseDTO
        {
            public string name { get; set; }
            public double value { get; set; }
        }

        public List<YearWeeklyDTO> SetYearLeaves(List<YearWeeklyDTO> data)
        {
            for (int i = 0; i < 12; i++)
            {
                string month = CultureInfo.CurrentUICulture.DateTimeFormat.MonthNames[i];
                var result = data.Find(x => x.Name == month);
                if (result == null)
                {
                    YearWeeklyDTO missingData = new YearWeeklyDTO();
                    missingData.Name = month;
                    missingData.value = 0;
                    data.Add(missingData);
                }
            }
            return data;
        }

        public List<YearWeeklyDTO> SetWeekLeaves(List<YearWeeklyDTO> data)
        {
            for (int i = 0; i < 7; i++)
            {
                string day = CultureInfo.CurrentUICulture.DateTimeFormat.DayNames[i];
                var result = data.Find(x => x.Name == day);
                if (result == null)
                {
                    YearWeeklyDTO missingData = new YearWeeklyDTO();
                    missingData.Name = day;
                    missingData.value = 0;
                    data.Add(missingData);
                }
            }
            return data;
        }
    }
}