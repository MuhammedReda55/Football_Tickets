﻿using Microsoft.AspNetCore.Identity;

namespace Football_Tickets.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string Name { get; set; }
        public string Address { get; set; }

        public string? photo { get; set; }
    }
}