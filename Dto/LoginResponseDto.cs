﻿public class LoginResponseDto
{
    public required string Id { get; set; }
    public string Username;
    public string Roles;
    public string? RefreshToken;
    public string? AccessToken;
}