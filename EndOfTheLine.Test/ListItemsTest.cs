
using Xunit;

namespace EndOfTheLine.Test
{
    public class ListItemsTest
    {
        [Theory]
        [InlineData("d", "c")]
        [InlineData("a", null)]
        [InlineData("b", "a")]
        public void CreateView(string item, string expected)
        {
            string[] lines = {"a", "b", "c", "d"};
            var prev = ListItems.PreviousItemOrDefault(lines, item);
            Assert.Equal(expected, prev);
        }
    }
}
