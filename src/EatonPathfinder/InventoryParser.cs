using EatonPathfinder.Entities;

namespace EatonPathfinder
{
    internal sealed record ParserOptions(string ItemSeparator = "─ Item: ", string StepSeparator = "+ ", int PerStepIndentation = 3, int PerItemIndentation = 2);

    internal static class InventoryParser
    {
        public static Inventory ParseLines(IReadOnlyList<string> lines, ParserOptions? options = null)
        {
            var parserOptions = options ?? new ParserOptions();
            var items = new List<Item>();
            var currentPath = new List<string>();

            foreach (var line in lines)
            {
                ParseLine(parserOptions, line, items, currentPath);
            }

            var inventory = Inventory.CreateNonEmpty(items);

            return inventory;
        }

        private static void ParseLine(ParserOptions options, string line, List<Item> items, List<string> currentPath)
        {
            var itemIndex = line.IndexOf(options.ItemSeparator, StringComparison.Ordinal);
            var stepIndex = line.IndexOf(options.StepSeparator, StringComparison.Ordinal);

            if (itemIndex >= 0)
            {
                items.Add(ParseItem(options, line, itemIndex, currentPath));

                return;
            }

            if (stepIndex >= 0)
            {
                AdjustPath(options, line, stepIndex, currentPath);

                return;
            }

            throw new InvalidDataException($"Invalid line in file: \"{line}\". Missing expected separator ({nameof(ParserOptions.ItemSeparator)}: \"{options.ItemSeparator}\"; {nameof(ParserOptions.StepSeparator)}: \"{options.StepSeparator}\").");
        }

        private static Item ParseItem(ParserOptions options, string line, int itemIndex, IReadOnlyList<string> currentPath)
        {
            var isValidDepth = itemIndex == (currentPath.Count - 1) * options.PerStepIndentation + options.PerItemIndentation;
            if (!isValidDepth)
            {
                throw new InvalidDataException($"Invalid line in file: \"{line}\". Formatting error: Item is not at the expected depth.");
            }

            var itemName = GetValue(line, itemIndex, options.ItemSeparator);

            return new Item(itemName, new Entities.Path([.. currentPath]));
        }

        private static void AdjustPath(ParserOptions options, string line, int stepIndex, List<string> currentPath)
        {
            var stepDepth = stepIndex / options.PerStepIndentation;
            var isValidDepth = stepIndex % options.PerStepIndentation == 0 && stepDepth <= currentPath.Count;
            if (!isValidDepth)
            {
                throw new InvalidDataException($"Invalid line in file: \"{line}\". Formatting error: Step is not at the expected depth.");
            }

            currentPath.RemoveRange(stepDepth, currentPath.Count - stepDepth);
            currentPath.Add(GetValue(line, stepIndex, options.StepSeparator));
        }

        private static string GetValue(string line, int separatorIndex, string separator)
        {
            return line[(separatorIndex + separator.Length)..].Trim();
        }
    }
}
