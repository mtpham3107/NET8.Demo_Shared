﻿using System.ComponentModel.DataAnnotations;

namespace NET8.Demo.GlobalAdmin.Models;

public class ResendConfirmationEmailViewModel : ExternalLoginViewModel
{
    [Required(ErrorMessage = "EmailRequired")]
    [EmailAddress(ErrorMessage = "EmailInvalid")]
    public string Email { get; set; }
}
