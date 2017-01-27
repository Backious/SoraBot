﻿using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Sora_Bot_1.SoraBot.Core;

namespace Sora_Bot_1.SoraBot.Modules.InfoModule
{
    public class InfoModule : ModuleBase
    {

        //$say hello -> hello
        [Command("say"), Summary("Echos a message")]
        public async Task Say([Remainder, Summary("The text to echo")] string echo)
        {
            //ReplyAsync is a mtheod on modulebase
            await ReplyAsync(echo);
        }

        //$lenny lenny -> ( ͡° ͜ʖ ͡°)
        [Command("lenny"), Summary("Posts a lenny face")]
        public async Task Lenny()
        {
            await ReplyAsync("( ͡° ͜ʖ ͡°)");
        }

        [Command("google"), Summary("Will google for you")]
        public async Task Google([Summary("Subject to google"), Remainder]string google)
        {
            string search = google.Replace(" ", "%20");
            await Context.Channel.SendMessageAsync("<https://lmgtfy.com/?q=" + search + ">");
        }

        //$swag  ( ͡° ͜ʖ ͡°)>⌐■-■ -> ( ͡⌐■ ͜ʖ ͡-■)
        [Command("swag"), Summary("Swags the chat")]
        public async Task Swag()
        {
            var msg = await ReplyAsync("( ͡° ͜ʖ ͡°)>⌐■-■");
            await Task.Delay(1000);
            await msg.ModifyAsync(x =>
            {
                x.Content = "( ͡⌐■ ͜ʖ ͡-■)";
            });
        }

        [Command("about"), Summary("Gives an about page")]
        public async Task AboutInfo()
        {
            await ReplyAsync("This bot was made by Serenity using Discord.Net");
        }

        [Command("ping"), Summary("Gives the ping of the Server on which the bot resides and the discord servers")]
        public async Task Ping()
        {
            await ReplyAsync($"Pong! {(Context.Client as DiscordSocketClient).Latency} ms :ping_pong:");
        }

        [Command("invite"), Summary("Gives an invite link to invite Sora to your own Guild!")]
        [Alias("inv")]
        public async Task InviteAsync()
        {
            await ReplyAsync("To invite Me just open this link and choose the Server:\nhttps://discordapp.com/oauth2/authorize?client_id=270931284489011202&scope=bot&permissions=30720");
        }

    }

    //create a module with the 'sample' prefix
    [Group("info")]
    public class Sample : ModuleBase
    {
        /*
        //$sample square 20 -> 400
        [Command("square"), Summary("Squares a number.")]
        public async Task Square([Summary("The number to square.")]int num)
        {
            //We can also access the channel from the command context
            await Context.Channel.SendMessageAsync($"{num}^2 = {Math.Pow(num, 2)}");
        }
        */


        [Command(""), Summary("Gives infos about the bot")]
        public async Task BotInfo()
        {

            var proc = System.Diagnostics.Process.GetCurrentProcess();

            DiscordSocketClient _client = Context.Client as DiscordSocketClient;

            Func<double, double> formatRamValue = d =>
            {
                while (d > 1024)
                    d /= 1024;

                return d;
            };

            Func<long, string> formatRamUnit = d =>
            {
                var units = new string[] { "B", "kB", "mB", "gB" };
                var unitCount = 0;
                while (d > 1024)
                {
                    d /= 1024;
                    unitCount++;
                }

                return units[unitCount];
            };

            var eb = new EmbedBuilder()
            {
                Color = new Color(4, 97, 247)
            };

            

            
            eb.AddField((efb) =>
            {
                efb.Name = "System";
                efb.IsInline = true;
                efb.Value = $"os version:\t{System.Runtime.InteropServices.RuntimeInformation.OSDescription}\narchitecture:\t{System.Runtime.InteropServices.RuntimeInformation.OSArchitecture}\nframework:\t{System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription}";
            });

            eb.AddField((efb) =>
            {
                efb.Name = "Sora";
                efb.IsInline = true;
                efb.Value = $"architecture:\t{System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture}\nup time:\t{(DateTime.Now - proc.StartTime).ToString(@"d'd 'hh\:mm\:ss")}\nmemory:\t{formatRamValue(proc.PagedMemorySize64).ToString("f2")} {formatRamUnit(proc.PagedMemorySize64)}\nprocessor time:\t{proc.TotalProcessorTime.ToString(@"d'd 'hh\:mm\:ss")}";
            });

            eb.AddField((efb) =>
            {
                efb.Name = "Discord";
                efb.IsInline = true;

                var channelCount = 0;
                var userCount = 0;
                foreach (var g in _client.Guilds)
                {
                    channelCount += g.Channels.Count;
                    userCount += g.Users.Count;
                }

                efb.Value = $"state:\t{_client.ConnectionState}\nguilds:\t{_client.Guilds.Count}\nchannels:\t{channelCount}\nusers:\t{userCount}\nping:\t{_client.Latency} ms";
            });
            
            await Context.Channel.SendMessageAsync("", false, eb);
        }

        // $sample userinfo --> foxbot#0282
        // $sample userinfo @Khionu --> Khionu#8708
        // $sample userinfo Khionu#8708 --> Khionu#8708
        // $sample userinfo Khionu --> Khionu#8708
        // $sample userinfo 96642168176807936 --> Khionu#8708
        // $sample whois 96642168176807936 --> Khionu#8708
        [Command("user"), Summary("Returns info about the current user, or the user paramter, if one passed.")]
        [Alias("userinfo", "whois")]
        public async Task UserInfo([Summary("The (optional) user to get info for")] IUser user = null)
        {
            var userInfo = user ?? Context.Client.CurrentUser; // ?? if not null return left. if null return right

            var eb = new EmbedBuilder()
            {
                Color = new Color(4, 97, 247),
                ThumbnailUrl = userInfo.AvatarUrl
            };
            eb.AddField((efb) =>
            {
                efb.Name = "User";
                efb.IsInline = true;
                efb.Value = $"Name + Discriminator: {userInfo.Username}#{userInfo.Discriminator} \n" +
                            $"Created at: {userInfo.CreatedAt} \n" +
                            $"Status: {userInfo.Status}";
            });

            await Context.Channel.SendMessageAsync("", false, eb);
        }

        [Command("guild"), Summary("Returns info about the current Guild")]
        [Alias("server", "serverinfo")]
        public async Task ServerInfo()
        {
            var eb = new EmbedBuilder()
            {
                Color = new Color(4, 97, 247),
                ThumbnailUrl = Context.Guild.IconUrl
            };

            var GuildOwner = await Context.Guild.GetUserAsync(Context.Guild.OwnerId);

            eb.AddField((efb) =>
            {
                efb.Name = "Guild";
                efb.IsInline = true;
                efb.Value = $"Name: {Context.Guild.Name} \n" + $"ID: {Context.Guild.Id}\n" + $"Owner: {GuildOwner}\n" +
                            $"Voice Region ID: {Context.Guild.VoiceRegionId}\n" +
                            $"Created At: {Context.Guild.CreatedAt}";
            });

            await Context.Channel.SendMessageAsync("", false, eb);
        }
    }
}
