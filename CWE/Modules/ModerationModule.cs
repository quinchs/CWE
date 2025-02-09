﻿namespace CWE.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using CWE.Common;
    using CWE.Data.Models;
    using CWE.Handlers;
    using CWE.Services;
    using Discord;
    using Discord.Commands;
    using Discord.WebSocket;
    using Interactivity;
    using Interactivity.Pagination;
    using Microsoft.Extensions.Configuration;

    public class ModerationModule : CWEModuleBase
    {
        private ModerationHandler modHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModerationModule"/> class.
        /// </summary>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/> to inject.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/> to inject.</param>
        /// <param name="interactivityService">The <see cref="InteractivityService"/> to inject.</param>
        public ModerationModule(IServiceProvider serviceProvider, IConfiguration configuration, InteractivityService interactivityService)
                : base(serviceProvider, configuration, interactivityService)
        {
        }

        private ModerationHandler ModerationHandler
        {
            get
            {
                if (this.modHandler == null)
                {
                    this.modHandler = HandlerService.GetHandlerInstance<ModerationHandler>();
                }

                return this.modHandler;
            }
        }

        /// <summary>
        /// Gets a success embed for a moderation action.
        /// </summary>
        /// <param name="type">The type of the infraction.</param>
        /// <param name="target">The target user for this infraction</param>
        /// <param name="staff">The staff member executing the infraction</param>
        /// <param name="reason">The reason for the infraction</param>
        /// <param name="mute">The optional mute parameter for mute cases</param>
        /// <returns>An embed that sumarizes the infraction.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:Parameter should not span multiple lines", Justification = "Styling of embed.")]
        public Embed GetSuccessEmbed(InfractionType type, SocketGuildUser target, SocketUser staff, string reason, TimeSpan? mute = null)
        {
            return new EmbedBuilder()
                .WithAuthor(target.GetAuthorEmbed())
                .WithTitle($"Successfully {ModerationHandler.FormatType(type)} {target}")
                .WithFields(
                    new EmbedFieldBuilder()
                        .WithIsInline(true)
                        .WithName("Staff member")
                        .WithValue(staff),
                    new EmbedFieldBuilder()
                        .WithIsInline(true)
                        .WithName("Reason")
                        .WithValue(reason),
                    mute.HasValue
                        ? new EmbedFieldBuilder()
                            .WithIsInline(true)
                            .WithName("Mute Duration")
                            .WithValue(mute.Value.ToReadableFormat())
                        : null)
                .WithCurrentTimestamp()
                .WithColor(Color.Green)
                .Build();
        }

        /// <summary>
        /// Warns a user.
        /// </summary>
        /// <param name="user">The user to warn.</param>
        /// <param name="reason">The reason to warn this user.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [RequireStaff]
        [Command("warn", RunMode = RunMode.Async)]
        public Task Warn(SocketGuildUser user, [Remainder] string reason)
            => this.ExecuteActionInternal(user, reason, InfractionType.Warn);

        /// <summary>
        /// Kicks a user.
        /// </summary>
        /// <param name="user">The user to kick.</param>
        /// <param name="reason">The reason to kick this user.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [RequireStaff]
        [Command("kick", RunMode = RunMode.Async)]
        public Task Kick(SocketGuildUser user, [Remainder] string reason)
            => this.ExecuteActionInternal(user, reason, InfractionType.Kick);

        /// <summary>
        /// Bans a user.
        /// </summary>
        /// <param name="user">The user to ban.</param>
        /// <param name="reason">The reason to ban this user.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [RequireStaff]
        [Command("ban", RunMode = RunMode.Async)]
        public Task Ban(SocketGuildUser user, [Remainder] string reason)
            => this.ExecuteActionInternal(user, reason, InfractionType.Kick);

        /// <summary>
        /// Mutes a user.
        /// </summary>
        /// <param name="user">The user to mute.</param>
        /// <param name="duration">The duration to mute the user.</param>
        /// <param name="reason">The reason to mute this user.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [RequireStaff]
        [Command("mute", RunMode = RunMode.Async)]
        public async Task Mute(SocketGuildUser user, string duration, [Remainder] string reason)
        {
            TimeSpan timespan;

            try
            {
                timespan = duration.ToTimespan();
            }
            catch
            {
                var errorEmbed = new CWEEmbedBuilder()
                    .WithStyle(EmbedStyle.Error)
                    .WithTitle("Invalid Timespan")
                    .WithDescription($"Please enter a valid timespan, ex: 1h15m");

                await this.ReplyAsync(embed: errorEmbed.Build());
                return;
            }

            await this.ExecuteActionInternal(user, reason, InfractionType.Mute);
        }

        /// <summary>
        /// Gets a users' infractions.
        /// </summary>
        /// <param name="user">The user whos infractions will be displayed.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [RequireStaff]
        [Command("infractions")]
        public async Task Infractions(SocketGuildUser user)
        {
            var infracs = await this.DataAccessLayer.GetUserInfractions(user.Id);

            var paginator = new LazyPaginatorBuilder()
                .WithUsers(this.Context.User)
                .WithMaxPageIndex((int)Math.Ceiling(infracs.Count / 20d))
                .WithPageFactory((page) =>
                {
                    List<EmbedFieldBuilder> fields = new List<EmbedFieldBuilder>();

                    foreach (var item in infracs.Skip(20 * page).Take(20))
                    {
                        fields.Add(new EmbedFieldBuilder()
                            .WithName(ModerationHandler.FormatType(item.Type))
                            .WithValue($"Date: {item.Date}\n" +
                                       $"Reason: {item.Reason}\n" +
                                       $"Staff member: {item.StaffUsername}\n" +
                                       $"Id: {item.InfractionId}"));
                    }

                    return Task.FromResult(new PageBuilder()
                        .WithColor(Color.Green)
                        .WithFields(fields)
                        .WithTitle($"{user}'s infractions"));
                });

            await this.Interactivity.SendPaginatorAsync(paginator.Build(), this.Context.Channel);
        }

        private async Task ExecuteActionInternal(SocketGuildUser user, string reason, InfractionType type, TimeSpan? time = null)
        {
            var result = await this.ModerationHandler.CreateInfractionAsync(type, user.Id, this.Context.User, reason, time);

            if (result)
            {
                await this.ReplyAsync(embed: this.GetSuccessEmbed(type, user, this.Context.User, reason, time));
            }
            else
            {
                var errorEmbed = new CWEEmbedBuilder()
                    .WithStyle(EmbedStyle.Error)
                    .WithTitle("Action failed")
                    .WithDescription($"Failed to {type} {user}");

                await this.ReplyAsync(embed: errorEmbed.Build());
            }
        }
    }
}
