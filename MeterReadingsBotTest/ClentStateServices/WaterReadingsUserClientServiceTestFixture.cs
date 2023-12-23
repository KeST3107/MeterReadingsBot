using System;
using System.Threading;
using System.Threading.Tasks;
using MeterReadingsBot.Services.ClientStateServices;
using NUnit.Framework;
using Telegram.Bot.Types;

namespace MeterReadingsBotTest.ClentStateServices;

[TestFixture]
public class WaterReadingsUserClientServiceTestFixture : UserClientServiceBaseTestFixture
{
    private IUserClientService _waterReadingsUserClientService;

    public async Task Test()
    {
        var message = await _waterReadingsUserClientService.GetStartUserTaskMessageAsync(new Message
            {
                Animation = null,
                Audio = null,
                AuthorSignature = null,
                Caption = null,
                CaptionEntities = null,
                ChannelChatCreated = false,
                Chat = new Chat
                {
                    Id = 35555,
                    Bio = null,
                    CanSetStickerSet = null,
                    Description = "",
                    FirstName = "",
                    InviteLink = "",
                    LastName = "",
                    LinkedChatId = 15,
                    Location = null,
                    Permissions = null
                },
                ConnectedWebsite = "",
                Contact = null,
                Date = DateTime.Now
            },
            CancellationToken.None);
    }

    protected override void SetUp()
    {
        base.SetUp();
        _waterReadingsUserClientService = GetService<IUserClientService>();
    }
}
