using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.ToDoList_Module
{
    public class ToDoListModel : BaseModelClass
    {
        [Key]
        public Guid TodolistId { get; set; } = Guid.NewGuid();
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }

        public bool IsComplete { get; set; }
        public string Image1 { get; set; }
        public string Image2 { get; set; }
        public string Image3 { get; set; }
        public string Image4 { get; set; }
        public string Image5 { get; set; }

    }
}