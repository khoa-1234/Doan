using System;
using System.Collections.Generic;

namespace QLNHWebAPI.Models;

public partial class Transaction
{
    public int TransactionId { get; set; }

    public int DonHangId { get; set; }

    public decimal Amount { get; set; }

    public string PaymentStatus { get; set; } = null!;

    public string? PaymentMethod { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? MoMoRequestId { get; set; }

    public string? MoMoOrderInfo { get; set; }

    public string? MoMoResponse { get; set; }

    public virtual DonHang DonHang { get; set; } = null!;
}
