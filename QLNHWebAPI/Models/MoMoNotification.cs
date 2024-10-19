using System;
using System.Collections.Generic;

namespace QLNHWebAPI.Models;

public partial class MoMoNotification
{
    public int NotificationId { get; set; }

    public int TransactionId { get; set; }

    public string NotifyData { get; set; } = null!;

    public DateTime ReceivedAt { get; set; }

    public virtual Transaction Transaction { get; set; } = null!;
}
