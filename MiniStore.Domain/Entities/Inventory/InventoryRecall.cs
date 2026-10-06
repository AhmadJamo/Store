namespace MiniStore.Domain.Entities;

public enum InventoryRecallStatus { Active = 1, Closed = 2 }
public enum RecallCommunicationChannel { Phone = 1, Email = 2, Sms = 3, InPerson = 4, Other = 5 }
public enum RecallCommunicationOutcome { Attempted = 1, Reached = 2, Confirmed = 3, Failed = 4 }

public sealed class InventoryRecall
{
    private InventoryRecall() { Reference = Identifier = Reason = CreatedByUserId = string.Empty; }
    public long Id { get; private set; }
    public string Reference { get; private set; }
    public int ProductId { get; private set; }
    public string Identifier { get; private set; }
    public string Reason { get; private set; }
    public InventoryRecallStatus Status { get; private set; }
    public string CreatedByUserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string? ClosedByUserId { get; private set; }
    public DateTime? ClosedAt { get; private set; }
    public string? ClosureNotes { get; private set; }
    public byte[] RowVersion { get; private set; } = [];

    public InventoryRecall(string reference, int productId, string identifier, string reason, string userId)
    {
        if (string.IsNullOrWhiteSpace(reference) || reference.Trim().Length > 50) throw new ArgumentException("Recall reference is required and cannot exceed 50 characters.");
        if (productId <= 0) throw new ArgumentException("Product is required.");
        if (string.IsNullOrWhiteSpace(identifier) || identifier.Trim().Length > 100) throw new ArgumentException("Lot or serial number is required.");
        if (string.IsNullOrWhiteSpace(reason) || reason.Trim().Length > 250) throw new ArgumentException("Recall reason is required and cannot exceed 250 characters.");
        EnsureUser(userId);
        Reference=reference.Trim().ToUpperInvariant();ProductId=productId;Identifier=identifier.Trim().ToUpperInvariant();Reason=reason.Trim();
        CreatedByUserId=userId.Trim();CreatedAt=DateTime.UtcNow;Status=InventoryRecallStatus.Active;
    }

    public void Close(string notes, string userId)
    {
        if(Status!=InventoryRecallStatus.Active)throw new InvalidOperationException("Only an active recall can be closed.");
        if(string.IsNullOrWhiteSpace(notes)||notes.Trim().Length>250)throw new ArgumentException("Recall closure notes are required and cannot exceed 250 characters.");
        EnsureUser(userId);Status=InventoryRecallStatus.Closed;ClosureNotes=notes.Trim();ClosedByUserId=userId.Trim();ClosedAt=DateTime.UtcNow;
    }
    private static void EnsureUser(string value){if(string.IsNullOrWhiteSpace(value)||value.Trim().Length>450)throw new ArgumentException("User is required.");}
}

public sealed class InventoryRecallCommunication
{
    private InventoryRecallCommunication(){PartyName=ChannelAddress=Notes=CreatedByUserId=string.Empty;}
    public long Id{get;private set;}public long InventoryRecallId{get;private set;}public string PartyName{get;private set;}public string ChannelAddress{get;private set;}
    public RecallCommunicationChannel Channel{get;private set;}public RecallCommunicationOutcome Outcome{get;private set;}public string Notes{get;private set;}
    public string CreatedByUserId{get;private set;}public DateTime CreatedAt{get;private set;}
    public InventoryRecallCommunication(long recallId,string partyName,string? channelAddress,RecallCommunicationChannel channel,RecallCommunicationOutcome outcome,string notes,string userId)
    {
        if(recallId<=0)throw new ArgumentException("Inventory recall is required.");if(string.IsNullOrWhiteSpace(partyName)||partyName.Trim().Length>200)throw new ArgumentException("Communication party is required and cannot exceed 200 characters.");
        if(channelAddress?.Trim().Length>200)throw new ArgumentException("Communication address cannot exceed 200 characters.");if(!Enum.IsDefined(channel)||!Enum.IsDefined(outcome))throw new ArgumentException("Communication channel or outcome is invalid.");
        if(string.IsNullOrWhiteSpace(notes)||notes.Trim().Length>500)throw new ArgumentException("Communication notes are required and cannot exceed 500 characters.");if(string.IsNullOrWhiteSpace(userId)||userId.Trim().Length>450)throw new ArgumentException("User is required.");
        InventoryRecallId=recallId;PartyName=partyName.Trim();ChannelAddress=channelAddress?.Trim()??string.Empty;Channel=channel;Outcome=outcome;Notes=notes.Trim();CreatedByUserId=userId.Trim();CreatedAt=DateTime.UtcNow;
    }
}
