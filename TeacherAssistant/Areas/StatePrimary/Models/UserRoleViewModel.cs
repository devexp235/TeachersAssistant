﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TeacherAssistant.Areas.StatePrimary.TeacherAssistant.Models
{
    public class UserRoleViewModel
    {
        public string UserId { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string RoleName { get; set; }

    }
}