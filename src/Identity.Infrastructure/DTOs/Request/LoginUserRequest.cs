﻿using System.ComponentModel.DataAnnotations;

namespace Identity.Infrastructure.DTOs.Request
{
    public class LoginUserRequest
    {
        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        public string Password { get; set; }
    }
}
