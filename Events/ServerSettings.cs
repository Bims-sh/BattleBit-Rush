using BattleBitApi.Api;
using Microsoft.Extensions.Logging;

namespace BattleBitApi.Events;

public class ServerSettings : Event
{
    public override Task OnConnected()
    {
        foreach (var Map in Server.MapRotation.GetMapRotation())
        {
            Server.MapRotation.RemoveFromRotation(Map);
        }
        
        foreach (var Map in Program.ServerConfiguration.MapRotation)
        {
            Server.MapRotation.AddToRotation(Map);
        }
        
        if (!Server.MapRotation.GetMapRotation().Any())
        {
            Program.ReloadConfiguration();
        }

        foreach (var GameMode in Server.GamemodeRotation.GetGamemodeRotation())
        {
            Server.GamemodeRotation.RemoveFromRotation(GameMode);
        }

        Server.GamemodeRotation.AddToRotation("RUSH");

        var serverRotation = Server.MapRotation.GetMapRotation();
        Program.Logger.Info($"Loaded Map Rotation: {string.Join(", ", serverRotation)}");
        
        return Task.CompletedTask;
    }
}