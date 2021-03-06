﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.VisualBasic;
using Sora_Bot_1.SoraBot.Core;
using Sora_Bot_1.SoraBot.Services;
using Sora_Bot_1.SoraBot.Services.ChangelogService;
using Sora_Bot_1.SoraBot.Services.EPService;
using Sora_Bot_1.SoraBot.Services.StarBoradService;
using System.Collections.Concurrent;
using Sora_Bot_1.SoraBot.Services.ConfigService;

namespace Sora_Bot_1.SoraBot.Modules.OwnerModule
{
    [Group("o")]
    [RequireOwner]
    public class OwnerModule : ModuleBase<SocketCommandContext>
    {
        //PREFIX

        private readonly CommandHandlingService handler;
        private UserGuildUpdateService updateService;
        private StarBoardService starBoardService;
        private AnimeService _aniServ;
        private EPService epService;

        private ConcurrentDictionary<string, string> configDict = new ConcurrentDictionary<string, string>();
        private string _token;


        public OwnerModule(CommandHandlingService _handler, UserGuildUpdateService service, StarBoardService starboards,
            EPService _epService, AnimeService aniSer)
        {
            handler = _handler;
            updateService = service;
            starBoardService = starboards;
            epService = _epService;
            _aniServ = aniSer;
            /* //TODO FIX THIS SHIT UP
            configDict = ConfigService.getConfig();

#if DEBUG
            if (!configDict.TryGetValue("token2", out _token))
            {
                throw new Exception("FAILED TO GET TOKEN2");
            }
            else
            {
                Console.WriteLine("GOT DEBUG TOKEN");
            }
#else
            if (!configDict.TryGetValue("token1", out _token))
            {
                throw new Exception("FAILED TO GET TOKEN1");
            }
#endif
*/
        }

        [Command("auth")]
        [RequireOwner]
        public async Task AnimeAuth()
        {
            await _aniServ.RequestAuth();
        }

        [Command("prefix"), Summary("Changes the prefix of the bot")]
        [RequireOwner]
        public async Task ChangePrefixOwner([Summary("Prefix to change to")] string prefix)
        {
            await ReplyAsync($"Prefix in this Guild was changed to `{prefix}`");
            handler.UpdateDict(Context.Guild.Id, prefix);
            handler.SaveDatabase();
        }

        //END PREFIX
        //EPSERIVCE

        [Command("epCount")]
        [RequireOwner]
        public async Task CountUsersEP()
        {
            await ReplyAsync($"Currently {epService.GetUserCount()} users in Database");
        }
        //END EPSERVICE

        //DISCONNECT AND RECONNECT
        [Command("reconnect")]
        [RequireOwner]
        public async Task ReconnectToDiscord()
        {
            //TODO RECONNECT
            var client = Context.Client;
            await client.LogoutAsync();
            await client.StopAsync();



            /*
            await Task.Delay(20000);
            client = createClient().Result;
            await client.LoginAsync(TokenType.Bot, token);
            //await client.ConnectAsync(); TODO StartAsync();
            await client.StartAsync();
            commands = new CommandHandler();
            await commands.Install(client);
            client.Disconnected += Client_Disconnected;
            await SentryService.SendMessage($"**THE BOT HAS DISCONNECTED AND SUCCESFULLY RECONNECTED**");
        */
        }

        //ANNOUNCEMENTS

        /*
    [Command("here"), Summary("Sets the Channel in which the message was written as Channel to announce")]
    [RequireOwner]
    public async Task SetAnnounceChannel()
    {
        await updateService.SetChannel(Context);
    }*/

        [Command("loadChange")]
        [RequireOwner]
        public async Task ReloadChangelog()
        {
            ChangelogService.LoadChangelog();
            await ReplyAsync(":white_check_mark: Reloaded the Changelog");
        }

        [Command("redoChange")]
        [RequireOwner]
        public async Task RedoChange([Remainder] string changelog)
        {
            await ChangelogService.modifyChangelog(changelog);
            await ReplyAsync(":white_check_mark: Redone the Changelog");
        }

        /*
        [Command("remove"), Summary("Removes current Announcement channel to stop the bot from announcing")]
        [RequireOwner]
        public async Task RemoveAnnounceChannel()
        {
            await updateService.RemoveChannel(Context);
        }*/

        //END ANNOUNCEMENTS

        //STARBOARD
        [Command("star"), Summary("Sets the Channel in which Star reactions should be posted")]
        [RequireOwner]
        public async Task SetStarBoardChannel()
        {
            await starBoardService.SetChannel(Context);
        }

        [Command("starremove"), Summary("Removes current Starboard channel!")]
        [RequireOwner]
        public async Task RemoveStarBoardChannel()
        {
            await starBoardService.RemoveChannel(Context);
        }

        [Command("msgme")]
        [RequireOwner]
        public async Task SendSentryMessage([Remainder] string message)
        {
            await SentryService.SendMessage(message);
        }

        //END STARBOARD

        //CREATE EXCEPTION

        [Command("exc")]
        [RequireOwner]
        public async Task CreateException()
        {
            try
            {
                int i2 = 0;
                int i = 10 / i2;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await SentryService.SendError(e, Context);
            }
        }

        [Command("invite")]
        [RequireOwner]
        public async Task CreateInvite(ulong id)
        {
            try
            {
                var RequestedGuild = Context.Client.GetGuild(id);
                IInviteMetadata GuildDefault =
                    await (RequestedGuild.GetChannel(RequestedGuild.DefaultChannel.Id) as IGuildChannel)
                        .CreateInviteAsync();
                await Context.Channel.SendMessageAsync("Invite link: " + GuildDefault.Url);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await SentryService.SendError(e, Context);
            }
        }

        [Command("leaveGuild")]
        [RequireOwner]
        public async Task LeaveGuild(ulong id)
        {
            try
            {
                var client = Context.Client as DiscordSocketClient;
                await client.GetGuild(id).LeaveAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await SentryService.SendError(e, Context);
            }
        }

        [Command("writeline")]
        [RequireOwner]
        public async Task ConsoleWrite([Remainder] string write)
        {
            Console.WriteLine(write);
        }

        [Command("retry")]
        [RequireOwner]
        public async Task RetryTry()
        {
            await Context.Channel.SendMessageAsync("test", false, null, new RequestOptions { Timeout = 5000, RetryMode = RetryMode.AlwaysRetry });
        }

        [Command("shutdown")]
        [RequireOwner]
        public async Task ShutDown()
        {
            await Context.Channel.SendMessageAsync("I wanted to leave anyway :information_desk_person:");
            //await Context.Client.DisconnectAsync();
            //await ((DiscordSocketClient) Context.Client).LogoutAsync();
            await ((DiscordSocketClient) Context.Client).StopAsync();
        }

        [Command("ban")]
        [RequireOwner]
        public async Task BanUser(IUser user)
        {
            try
            {
                await Context.Guild.AddBanAsync(user, 0, null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await SentryService.SendError(e, Context);
            }
        }

        [Command("kick")]
        [RequireOwner]
        public async Task KickUser(IUser user)
        {
            try
            {
                await (user as IGuildUser).KickAsync(null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await SentryService.SendError(e, Context);
            }
        }

        [Command("guildlist")]
        [RequireOwner]
        public async Task GuildList()
        {
            try
            {
                string guildList = "";
                var guilds = (Context.Client as DiscordSocketClient).Guilds;
                foreach (var g in guilds)
                {
                    guildList += $"Name: {g.Name}\n ID: {g.Id} \n";
                }
                File.WriteAllText("guildlist.txt", guildList);
                await Context.Channel.SendFileAsync("guildlist.txt", null, false, null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await SentryService.SendError(e, Context);
            }
        }
    }
}