namespace IdentityServer.Application.DTOs;

public class UpdateUserDto
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string? Firstname { get; set; }
    public string? Lastname { get; set; }
    public string? EmailAddress { get; set; }
    public string Password { get; set; } = string.Empty;
    public int? AccessFailedCount { get; set; }
    //public DateTime? LockoutEnd { get; set; }
}