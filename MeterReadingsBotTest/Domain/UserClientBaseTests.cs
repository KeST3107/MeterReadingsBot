using System;
using System.Linq;
using MeterReadingsBot.Entities;
using MeterReadingsBot.Models;
using NUnit.Framework;

namespace MeterReadingsBotTest.Domain;

public class UserClientBaseTests
{
    #region Public
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void UpdateLastMessage_TimeUpdated()
    {
        var client = new WaterReadingsUserClient(15151515);
        var datetimeNow = DateTime.Now;
        Assert.AreEqual(client.TimeLastMessage.ToLocalTime().Hour, datetimeNow.Hour);
        Assert.AreEqual(client.TimeLastMessage.Minute, datetimeNow.Minute);
        Assert.AreEqual(client.TimeLastMessage.Second, datetimeNow.Second);
    }

    [Test]
    public void AddPersonalNumber_MultipleNumbers_DifferentNumbersSaved()
    {
        var client = new WaterReadingsUserClient(15151515);
        var personalNumber = "123123123";
        client.AddPersonnelNumber(personalNumber);
        client.AddPersonnelNumber(personalNumber);
        client.AddPersonnelNumber(personalNumber);
        Assert.AreEqual(personalNumber, client.PersonalNumbers.Single());
        Assert.AreEqual(1, client.PersonalNumbers.Count);
    }

    [Test]
    public void UpdateTempClient_ClientUpdated()
    {
        var client = new WaterReadingsUserClient(15151515);
        var clientDto = new ClientDto("Address", "FullName", "15351565");
        client.UpdateTempClient(clientDto);
        Assert.AreEqual(client.TempClient.Address, clientDto.Address);
        Assert.AreEqual(client.TempClient.FullName, clientDto.FullName);
        Assert.AreEqual(client.TempClient.PersonalNumber, clientDto.PersonnelNumber);
    }
    #endregion
}
