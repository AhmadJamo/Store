using System.Net;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using MiniStore.Application.Services;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Web.Services;

public sealed class SupplierOutboxWorker(IServiceScopeFactory scopes,IBackgroundJobMonitor monitor,ILogger<SupplierOutboxWorker> logger) : BackgroundService
{
    private const string JobKey="supplier-outbox";
    public static HttpMessageHandler CreateHandler()=>new SocketsHttpHandler
    {
        AllowAutoRedirect=false,
        UseProxy=false,
        ConnectCallback=async(context,cancellationToken)=>
        {
            var addresses=await Dns.GetHostAddressesAsync(context.DnsEndPoint.Host,cancellationToken);
            var address=addresses.FirstOrDefault(x=>!IsPrivate(x))??throw new InvalidOperationException("Supplier callback resolved only to private or local network addresses.");
            var socket=new Socket(address.AddressFamily,SocketType.Stream,ProtocolType.Tcp);try{await socket.ConnectAsync(new IPEndPoint(address,context.DnsEndPoint.Port),cancellationToken);return new NetworkStream(socket,ownsSocket:true);}catch{socket.Dispose();throw;}
        }
    };
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        monitor.Register(JobKey,"jobSupplierOutbox","SupplierIntegrations.View",TimeSpan.FromSeconds(30));
        using var timer=new PeriodicTimer(TimeSpan.FromSeconds(30));
        do{monitor.Started(JobKey);try{await DispatchBatch(stoppingToken);monitor.Succeeded(JobKey);}catch(OperationCanceledException)when(stoppingToken.IsCancellationRequested){break;}catch(Exception ex){monitor.Failed(JobKey);logger.LogError(ex,"Supplier outbox batch failed.");}}while(await timer.WaitForNextTickAsync(stoppingToken));
    }
    private async Task DispatchBatch(CancellationToken cancellationToken)
    {
        using var discovery=scopes.CreateScope();var repository=discovery.ServiceProvider.GetRequiredService<ISupplierIntegrationRepository>();var due=await repository.GetDueOutboxAcrossTenantsAsync(20,DateTime.UtcNow);
        foreach(var candidate in due){if(cancellationToken.IsCancellationRequested)return;await Dispatch(candidate.Message.Id,candidate.TenantId,cancellationToken);}
    }
    private async Task Dispatch(int messageId,int tenantId,CancellationToken cancellationToken)
    {
        using var scope=scopes.CreateScope();var tenantScope=scope.ServiceProvider.GetRequiredService<ITenantScope>();using var activeTenant=tenantScope.BeginScope(tenantId);
        var repository=scope.ServiceProvider.GetRequiredService<ISupplierIntegrationRepository>();var unit=scope.ServiceProvider.GetRequiredService<IUnitOfWork>();var protector=scope.ServiceProvider.GetRequiredService<ISupplierIntegrationSecretProtector>();
        if(!await repository.TryStartOutboxAsync(messageId,DateTime.UtcNow,DateTime.UtcNow.AddMinutes(5)))return;var message=await repository.GetOutboxMessageAsync(messageId);if(message is null)return;
        try
        {
            var uri=new Uri(message.Destination,UriKind.Absolute);await EnsurePublicHttps(uri,cancellationToken);
            var client=scope.ServiceProvider.GetRequiredService<IHttpClientFactory>().CreateClient("SupplierOutbox");using var request=new HttpRequestMessage(HttpMethod.Post,uri){Content=new StringContent(message.Payload,Encoding.UTF8,"application/xml")};
            request.Headers.Add("Idempotency-Key",message.IdempotencyKey);request.Headers.Add("X-MiniStore-Event-Type",message.MessageType);request.Headers.Add("X-MiniStore-Event-Id",message.Id.ToString(System.Globalization.CultureInfo.InvariantCulture));
            if(!string.IsNullOrWhiteSpace(message.ProtectedAuthorizationSecret))request.Headers.Authorization=new AuthenticationHeaderValue("Bearer",protector.Unprotect(message.ProtectedAuthorizationSecret));
            using var response=await client.SendAsync(request,cancellationToken);if(!response.IsSuccessStatusCode){var body=await response.Content.ReadAsStringAsync(cancellationToken);throw new HttpRequestException($"Callback returned {(int)response.StatusCode}: {body[..Math.Min(body.Length,500)]}");}
            await unit.ExecuteInTransactionAsync(()=>{message.MarkDelivered();return Task.CompletedTask;});
        }
        catch(Exception ex)when(ex is not OperationCanceledException)
        {
            logger.LogWarning(ex,"Supplier outbox message {MessageId} delivery attempt failed.",messageId);await unit.ExecuteInTransactionAsync(()=>{message.MarkFailed(ex.Message);return Task.CompletedTask;});
        }
    }
    private static async Task EnsurePublicHttps(Uri uri,CancellationToken cancellationToken)
    {
        if(uri.Scheme!=Uri.UriSchemeHttps||uri.IsLoopback)throw new InvalidOperationException("Supplier callback must use a public HTTPS destination.");
        var addresses=await Dns.GetHostAddressesAsync(uri.DnsSafeHost,cancellationToken);if(addresses.Length==0||addresses.Any(IsPrivate))throw new InvalidOperationException("Supplier callback resolved to a private or local network address.");
    }
    private static bool IsPrivate(IPAddress address)
    {
        if(IPAddress.IsLoopback(address)||address.IsIPv6LinkLocal||address.IsIPv6SiteLocal)return true;if(address.AddressFamily==System.Net.Sockets.AddressFamily.InterNetwork){var b=address.GetAddressBytes();return b[0]==10||b[0]==127||(b[0]==169&&b[1]==254)||(b[0]==172&&b[1]>=16&&b[1]<=31)||(b[0]==192&&b[1]==168);}return address.Equals(IPAddress.IPv6Loopback);
    }
}
