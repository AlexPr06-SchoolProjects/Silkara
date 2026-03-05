using UTP.UnitTests.Helpers.Payload;

namespace UTP.UnitTests.TestHelpers.UtpEngine;

internal class ComplexRoundTripData
{
    public static IEnumerable<object[]> GetComplexRoundTripData()
    {
        // Кейс 1: "Жирный" пакет (много заголовков + сложный Payload)
        yield return new object[]
        {
            (short)100,
            new Dictionary<string, string> {
                ["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64)",
                ["X-Trace-ID"] = Guid.NewGuid().ToString(),
                ["Content-Type"] = "application/json; charset=utf-8",
                ["Auth-Status"] = "Authorized"
            },
            new TestPayload(1, "Complex_System_Message_With_Under_Scores")
        };

        // Кейс 2: Спецсимволы в Payload и заголовках (UTF-16/UTF-8 Stress)
        yield return new object[]
        {
            (short)200,
            new Dictionary<string, string> { ["Lang"] = "日本語 / Українська", ["Tag"] = "☤" },
            new TestPayload(2, "Name with spaces and symbols: !@#$%^&*()_+{}[]|\\")
        };

        // Кейс 3: Минимальные данные (краевые значения)
        yield return new object[]
        {
            short.MinValue,
            new Dictionary<string, string>(), // Пустые заголовки
            new TestPayload(0, string.Empty)   // Пустая строка в Payload
        };

        // Кейс 4: Огромный Payload (проверка работы ArrayPool на больших размерах)
        yield return new object[]
        {
            (short)500,
            new Dictionary<string, string> { ["Size"] = "Huge" },
            new TestPayload(999, new string('A', 10000))
        };
    }
}
