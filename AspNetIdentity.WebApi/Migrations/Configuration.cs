namespace AspNetIdentity.WebApi.Migrations
{
    using AspNetIdentity.Core.Model.TsfModule;
    using DocumentFormat.OpenXml.Office2010.ExcelAc;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    using System.Globalization;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<AspNetIdentity.WebApi.Infrastructure.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(AspNetIdentity.WebApi.Infrastructure.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );

            //context.EmpHerachais.AddOrUpdate(x => x.Id,
            //    new EmployeeHerachai() { Id = 1, Top = true, Manager = 0 },
            //    new EmployeeHerachai() { Id = 2, Top = false, Manager = 1 },
            //    new EmployeeHerachai() { Id = 3, Top = false, Manager = 1 },
            //    new EmployeeHerachai() { Id = 4, Top = false, Manager = 1 },
            //    new EmployeeHerachai() { Id = 5, Top = false, Manager = 2 },
            //    new EmployeeHerachai() { Id = 6, Top = false, Manager = 2 },
            //    new EmployeeHerachai() { Id = 7, Top = false, Manager = 2 },
            //    new EmployeeHerachai() { Id = 8, Top = false, Manager = 3 },
            //    new EmployeeHerachai() { Id = 9, Top = false, Manager = 4 },
            //    new EmployeeHerachai() { Id = 10, Top = false, Manager = 4 },
            //    new EmployeeHerachai() { Id = 11, Top = false, Manager = 4 },
            //    new EmployeeHerachai() { Id = 12, Top = false, Manager = 4 },
            //    new EmployeeHerachai() { Id = 13, Top = false, Manager = 5 },
            //    new EmployeeHerachai() { Id = 14, Top = false, Manager = 5 },
            //    new EmployeeHerachai() { Id = 15, Top = false, Manager = 7 },
            //    new EmployeeHerachai() { Id = 16, Top = false, Manager = 7 },
            //    new EmployeeHerachai() { Id = 17, Top = false, Manager = 10 },
            //    new EmployeeHerachai() { Id = 18, Top = false, Manager = 10 },
            //    new EmployeeHerachai() { Id = 19, Top = false, Manager = 10 },
            //    new EmployeeHerachai() { Id = 20, Top = false, Manager = 10 }
            //    );
        }
    }
}