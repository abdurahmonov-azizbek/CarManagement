// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

namespace CarManagement.Api.Services.Foundations.Authorizations.Models
{
    public sealed class JwtSettings
    {
        public bool ValidateIssuer { get; set; }
        public string? ValidIssuer { get; set; }
        public bool ValidateAudience { get; set; }
        public string? ValidAudience { get; set; }
        public bool ValidateLifeTime { get; set; }
        public int ExpirationTimeInMinutes { get; set; }
        public bool ValidateIssuerSigningKey { get; set; }
        public string SecretKey { get; set; } = default!;
    }
}
