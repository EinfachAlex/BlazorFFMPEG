using System.Net.WebSockets;
using System.Text;
using EinfachAlex.Utils.WebRequest;
using Microsoft.AspNetCore.Mvc;

namespace BlazorFFMPEG.Backend.Controllers.Get;

public class WebSocketController : ControllerBase
{
    public static WebSocket? websocketServer;
    
    private const string ENDPOINT = "/ws";
    private const ERequestTypes ENDPOINT_TYPE = ERequestTypes.GET;

    [HttpGet(ENDPOINT)]
    public async Task Get()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            websocketServer = await HttpContext.WebSockets.AcceptWebSocketAsync();
            await Echo(websocketServer);
        }
        else
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }

    private static async Task Echo(WebSocket webSocket)
    {
        var buffer = new byte[1024 * 4];
        var receiveResult = await webSocket.ReceiveAsync(
            new ArraySegment<byte>(buffer),
            CancellationToken.None);

        while (!receiveResult.CloseStatus.HasValue)
        {
            var hurensohn = Encoding.ASCII.GetBytes("Hallo du Hurensohn!");
            await webSocket.SendAsync(
                new ArraySegment<byte>(hurensohn, 0, hurensohn.Length),
                receiveResult.MessageType,
                receiveResult.EndOfMessage,
                CancellationToken.None);

            receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer),
                CancellationToken.None);
        }

        await webSocket.CloseAsync(
            receiveResult.CloseStatus.Value,
            receiveResult.CloseStatusDescription,
            CancellationToken.None);
    }
}