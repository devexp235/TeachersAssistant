﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace  TeacherAssistant.Models
{
    public class UserRoleViewModel
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string RoleName { get; set; }

    }
}