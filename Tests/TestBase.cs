using Xunit.Abstractions;

public class TestBase
{
    public TestBase(ITestOutputHelper output)
    {
        Output = output;
    }

    protected readonly ITestOutputHelper Output;
}