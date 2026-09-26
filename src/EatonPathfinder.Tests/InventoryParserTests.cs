namespace EatonPathfinder.Tests
{
    public sealed class InventoryParserTests
    {
        private static readonly string[] _sampleLines =
        [
            "+ Walk to the end of the hall.",
            "├──+ Turn left.",
            "|  └──+ Go through the first door on the right.",
            "|     ├──+ Open the cabinet on the left.",
            "|     |  └── Item: Cookies",
            "|     ├──+ Open the cabinet above the sink.",
            "|     |  └── Item: Coffee Mug",
            "|     └──+ Open the refrigerator.",
            "|        └── Item: Carton of milk",
            "└──+ Turn right.",
            "   └──+ Go through the door at the end of the hall.",
            "      ├──+ Look on top of the desk.",
            "      |  └── Item: Mobile phone",
            "      └──+ Open the desk drawer.",
            "         └── Item: Pencils"
        ];

        [Fact]
        public void ParseLines_SampleFile_ReturnsItemsInAlphabeticalOrder()
        {
            var inventory = InventoryParser.ParseLines(_sampleLines);

            Assert.Equal(
                ["Carton of milk", "Coffee Mug", "Cookies", "Mobile phone", "Pencils"],
                inventory.Items.Select(i => i.Name)
            );
        }

        [Fact]
        public void ParseLines_SampleFile_ReturnsCorrectPath()
        {
            var inventory = InventoryParser.ParseLines(_sampleLines);

            var coffeeMug = inventory.Items.Single(i => i.Name == "Coffee Mug");
            Assert.Equal(
                ["Walk to the end of the hall.", "Turn left.", "Go through the first door on the right.", "Open the cabinet above the sink."],
                coffeeMug.Path.Steps
            );
        }

        [Fact]
        public void ParseLines_BacktrackingMultipleLevels_DropsVisitedSteps()
        {
            var inventory = InventoryParser.ParseLines(_sampleLines);

            var mobilePhone = inventory.Items.Single(i => i.Name == "Mobile phone");
            Assert.Equal(
                ["Walk to the end of the hall.", "Turn right.", "Go through the door at the end of the hall.", "Look on top of the desk."],
                mobilePhone.Path.Steps
            );
        }

        [Theory]
        [InlineData("+ A", "         └── Item: Too deep")]
        [InlineData("+ A", "├──+ B", "|        └──+ Skips depth levels")]
        [InlineData("+ A", "noSeparator")]
        [InlineData("+ A", "├──+ B", "|  └── Item: Duplicate", "└──+ C", "   └── Item: Duplicate")]
        [InlineData("+ A", "└──+ B")]
        public void ParseLines_InvalidFile_Throws(params string[] lines)
        {
            Assert.Throws<InvalidDataException>(() => InventoryParser.ParseLines(lines));
        }
    }
}
