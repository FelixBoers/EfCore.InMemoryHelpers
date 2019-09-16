using Xunit.Abstractions;

public class TestBase
{
    protected readonly ITestOutputHelper Output;

    public TestBase(ITestOutputHelper output)
    {
        Output = output;
    }
}