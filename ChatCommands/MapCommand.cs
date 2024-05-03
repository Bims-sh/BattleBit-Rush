﻿using System.Text;
using BattleBitApi.Enums;
using BattleBitApi.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace BattleBitApi.ChatCommands;

public class MapCommand : ChatCommand
{
    public MapCommand() : base(
        name: "map",
        description: "Add, remove or list current maps.",
        usage: "map <remove (r), add (a), list (ls), reload (rl)> [map name]",
        minimumRequiredRole: PlayerRoles.Admin
    )
    {
        Action = (args, player) =>
        {
            if (!CanExecute(player))
            {
                player.Message("You do not have permission to execute this command.");
                return;
            }
            
            if (args.Length < 1)
            {
                player.Message($"Invalid arguments. Usage: {Usage}");
                return;
            }
            
            string action = args[0];
            args = args.Skip(1).ToArray();
            
            switch (action)
            {
                case "a":
                case "add":
                    if (args.Length < 1)
                    {
                        player.Message($"Invalid arguments. Usage: {Usage}");
                        return;
                    }
                    
                    var mapToAdd = string.Join(" ", args);
                    if (MapHelper.IsValidMap(mapToAdd))
                    {
                        if (!Server.MapRotation.AddToRotation(mapToAdd))
                        {
                            player.Message("Map already exists in the map rotation.");
                            return;
                        }
                        
                        Program.ServerConfiguration.MapRotation.Add(mapToAdd);
                        Program.SaveConfiguration(Program.ServerConfiguration);
                        player.Message($"Added {RichTextHelper.Bold(true)}{RichTextHelper.FromColorName("Gold")}{mapToAdd}{RichTextHelper.Color()}{RichTextHelper.Bold(false)} to the rotation.");
                    } 
                    else
                    {
                        player.Message("Invalid map.");
                    }
                    break;
                case "r":
                case "remove": 
                    if (args.Length < 1)
                    {
                        player.Message($"Invalid arguments. Usage: {Usage}");
                        return;
                    }
                    
                    var mapToRemove = string.Join(" ", args);
                    if (MapHelper.IsValidMap(mapToRemove))
                    {
                        if (!Server.MapRotation.RemoveFromRotation(mapToRemove))
                        {
                            player.Message("Map does not exist in the map rotation.");
                            return;
                        }
                        
                        Program.ServerConfiguration.MapRotation.Remove(mapToRemove);
                        Program.SaveConfiguration(Program.ServerConfiguration);
                        player.Message($"Removed {RichTextHelper.Bold(true)}{RichTextHelper.FromColorName("Gold")}{mapToRemove}{RichTextHelper.Color()}{RichTextHelper.Bold(false)} from the rotation.");
                    }
                    else
                    {
                        player.Message("Invalid map.");
                    }
                    break;
                case "ls":
                case "list":
                    var maps = new StringBuilder();
                    foreach (var map in Server.MapRotation.GetMapRotation())
                    {
                        maps.Append($"{RichTextHelper.Bold(true)}{RichTextHelper.FromColorName("Gold")}{map}{RichTextHelper.Color()}{RichTextHelper.Bold(false)}, ");
                    }
                    
                    player.Message($"Current map rotation:{RichTextHelper.NewLine()}{maps.ToString().TrimEnd(',', ' ')}");
                    break;
                case "rl":
                case "reload":
                    Program.ReloadConfiguration();
                    
                    player.Message("Reloaded map rotation.");
                    break;
                default:
                    player.Message($"Invalid action. Usage: {Usage}");
                    break;
            }
        };
    }
}