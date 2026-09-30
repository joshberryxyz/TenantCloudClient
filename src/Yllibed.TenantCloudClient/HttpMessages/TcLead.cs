namespace Yllibed.TenantCloudClient.HttpMessages;

public class TcLead : IHasId
{
	public long Id { get; set; }

	public string? Name { get; set; }

	public string? Email { get; set; }

	public string? Phone { get; set; }

	public string? Category { get; set; }

	public string? Type { get; set; }

	public string? Source { get; set; }

	public string? Status { get; set; }

	[JsonPropertyName("last_action_at")]
	public DateTimeOffset? LastActionAt { get; set; }
}
