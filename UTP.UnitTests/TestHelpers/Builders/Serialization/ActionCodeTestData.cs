namespace UTP.UnitTests.Helpers.Builders.Serialization;

internal class ActionCodeTestDataClass
{
    public static IEnumerable<object[]> GetActionCodeTestData()
    {
        yield return new object[] { (short)1, new byte[] { 0, 1 } };               // 2 bytes for short
        yield return new object[] { (short)258, new byte[] { 1, 2 } };             // 1*256 + 2 = 258
        yield return new object[] { (short)-1, new byte[] { 255, 255 } };          // 0xFFFF
        yield return new object[] { (short)32767, new byte[] { 127, 255 } };       // short.MaxValue
        yield return new object[] { (short)-32768, new byte[] { 128, 0 } };
    }
}
