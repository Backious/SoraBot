﻿using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace Sora_Bot_1.SoraBot.Services.Giphy
{
    public class GifService
    {

        public async Task GetGifBySearch(SocketCommandContext Context, string query)
        {
            try
            {
                var search = System.Net.WebUtility.UrlEncode(query);
                using (var http = new HttpClient())
                {
                    var response = await http.GetStringAsync($"http://api.giphy.com/v1/gifs/search?q={search}&api_key=dc6zaTOxFJmzC").ConfigureAwait(false);
                    var data = JsonConvert.DeserializeObject<GifData>(response);
                    var r = new Random();
                    if(data.data.Count < 1)
                    {
                        await Context.Channel.SendMessageAsync($":no_entry_sign: Couldn't find any Gifs :frowning:");
                        return;
                    }
                    var randomData = data.data[r.Next(data.data.Count - 1)];
                    int count = 0;
                    while (randomData.rating == "r" && count < 10)
                    {
                        randomData = data.data[r.Next(data.data.Count - 1)];
                        count++;
                    }
                    if (count > 9)
                    {
                        await Context.Channel.SendMessageAsync($":no_entry_sign: Couldn't find any Gifs that weren't SFW :frowning:");
                        Console.WriteLine("REMOVED R RATED");
                        return;
                    }
                    await Context.Channel.SendMessageAsync($"{randomData.images.original.url}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await SentryService.SendError(e, Context);
            }
           
        }

    }
}
