using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DevHub.DTOS.Auth;
public class HandleForgotPasswordRequest
{
    [EmailAddress]
    public string Email { get; set; }
    public string Password { get; set; }
    [Compare("Password")]
    public string ConfirmPassword { get; set; }
    [FromHeader(Name = "change-pass")]
    public string Token { get; set; }
}