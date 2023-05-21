﻿using Identity.Infrastructure.Models;

namespace MyLifeApp.Application.Dtos.Responses.Profile
{
    public class GetProfileResponse
    {
        public Guid Id { get; set; }
        public string? UserName { get; set; }
        public string? UserUsername { get; set; }
        public bool IsPrivate { get; set; }
    }
}
