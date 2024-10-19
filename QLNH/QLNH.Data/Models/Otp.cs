using System;
using System.Collections.Generic;

namespace QLNH.Data.Models;

public partial class Otp
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string Otpcode { get; set; } = null!;

    public DateTime ExpiryDate { get; set; }
}
