namespace MiniStore.Domain.Entities;

public enum SubscriptionStatus { Trial = 1, Active = 2, PastDue = 3, GracePeriod = 4, Suspended = 5, Cancelled = 6, Expired = 7 }
public enum BillingCycle { Monthly = 1, Annual = 2 }
public enum PlatformOperatorRole { Owner = 1, Administrator = 2, Support = 3, Billing = 4, Auditor = 5 }
