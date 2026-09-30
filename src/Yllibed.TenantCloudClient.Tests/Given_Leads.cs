using System.Net;
using System.Net.Http;
using AwesomeAssertions;
using Microsoft.Extensions.Time.Testing;

namespace Yllibed.TenantCloudClient.Tests;

[TestClass]
public sealed class Given_Leads
{
	// Trimmed copy of a captured GET https://api.tenantcloud.com/leads?take=20&sort=-last_action_at response.
	private const string CapturedPage = """
		{
			"data": [
				{
					"type": "lead",
					"id": "2479281",
					"attributes": {
						"name": null,
						"email": "lead1@example.com",
						"phone": null,
						"category": "tenant",
						"type": "hot",
						"source": "rental_application",
						"status": "new",
						"last_action_at": "2026-09-26T20:22:10.000000Z"
					}
				},
				{
					"type": "lead",
					"id": "2486358",
					"attributes": {
						"name": "Jane Doe",
						"email": "lead2@example.com",
						"phone": "13155550100",
						"category": "tenant",
						"type": "hot",
						"source": "rent_path",
						"status": "closed",
						"last_action_at": "2026-09-26T00:32:39.000000Z"
					}
				}
			],
			"meta": { "pagination": { "total": 477, "count": 2, "per_page": 20, "current_page": 1, "total_pages": 24 } }
		}
		""";

	[TestMethod]
	public async Task When_GettingLeadsPage_Then_RequestsSortedEndpointAndMapsAttributes()
	{
		using var handler = new CapturingHandler(CapturedPage);
		using var client = new TcClient(new StaticTokenProvider("test-token"), new(), handler, new FakeTimeProvider(), () => 0);

		var (entries, pageNo, total) = await client.Leads.GetPage(CancellationToken.None);

		handler.RequestUri!.PathAndQuery.Should().Be("/leads?page=1&sort=-last_action_at");
		pageNo.Should().Be(1);
		total.Should().Be(477);
		entries.Length.Should().Be(2);

		var first = entries.Span[0];
		first.Id.Should().Be(2479281);
		first.Name.Should().BeNull();
		first.Email.Should().Be("lead1@example.com");
		first.Phone.Should().BeNull();
		first.Category.Should().Be("tenant");
		first.Type.Should().Be("hot");
		first.Source.Should().Be("rental_application");
		first.Status.Should().Be("new");
		first.LastActionAt.Should().Be(new DateTimeOffset(2026, 9, 26, 20, 22, 10, TimeSpan.Zero));

		var second = entries.Span[1];
		second.Id.Should().Be(2486358);
		second.Name.Should().Be("Jane Doe");
		second.Phone.Should().Be("13155550100");
		second.Status.Should().Be("closed");
	}

	private sealed class CapturingHandler(string body) : HttpMessageHandler
	{
		public Uri? RequestUri { get; private set; }

		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			RequestUri = request.RequestUri;
			return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(body) });
		}
	}
}
