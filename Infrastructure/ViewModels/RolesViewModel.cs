﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Resources;

namespace Infrastructure.ViewModels
{
    public class RolesViewModel
    {
        public List<IdentityRole> Roles { get; set; }
        public NewRole NewRole { get; set; }
    }
    public class NewRole
    {
        public string RoleId { get; set; }
        [Required(ErrorMessageResourceType =typeof(ResourceData),ErrorMessageResourceName = "RoleName")]
        public string RoleName { get; set; }
    }
}
