﻿
namespace NerdCritica.Domain.DTOs.EventNotifications;

public record NotificationDTO(Guid NotificationId, string UserId, int Type,
    string Content
    );
