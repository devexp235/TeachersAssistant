﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TeachersAssistant.Domain.Model.Models;

namespace TeacherAssistant.Models
{
    public class ClassroomViewModel
    {
        public int? ClassroomId { get; set; }
        [Required]
        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; }
        [Required]
        public int CalendarBookingId { get; set; }
        [Required]
        public  string StudentType { get; set; }
        [Required]
        public int SubjectId { get; set; }
        public Subject Subject { get; set; }
        public string Select { set; get; }
        public string Create { set; get; }
        public string Update { set; get; }
        public string Delete { set; get; }
    }
}