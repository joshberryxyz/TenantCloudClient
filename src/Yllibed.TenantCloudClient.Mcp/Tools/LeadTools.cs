using System.Text.Json.Nodes;
using Repl;

namespace Yllibed.TenantCloudClient.Mcp.Tools;

internal sealed class LeadTools(ITcClient client)
{
	public IReplPageSource<JsonObject> ListLeads(IReplPagingContext paging) =>
		TenantCloudPages.Create(client.Leads, paging, McpJsonContext.Default.TcLead);
}
