using SupportChat.Domain.Queues;

namespace SupportChat.UnitTests.Queues;

public class OfficeHoursPolicyTests
{
    private readonly OfficeHoursPolicy _policy = new(new TimeOnly(9, 0), new TimeOnly(17, 0));

    [TestCase("09:00", true)]
    [TestCase("12:30", true)]
    [TestCase("16:59", true)]
    [TestCase("08:59", false)]
    [TestCase("17:00", false)]
    [TestCase("22:00", false)]
    public void Should_correctly_identify_if_time_is_within_office_hours(string timeText, bool expected)
    {
        var time = TimeOnly.Parse(timeText);

        var result = _policy.IsWithinOfficeHours(time);

        Assert.That(result, Is.EqualTo(expected));
    }
}