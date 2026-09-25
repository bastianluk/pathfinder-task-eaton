namespace EatonPathfinder.Entities
{
    internal sealed class Inventory
    {
        private Inventory(IReadOnlyList<Item> items)
        {
            Items = items;
        }

        public IReadOnlyList<Item> Items { get; }

        public static Inventory Create(IReadOnlyList<Item> items)
        {
            if (items is null || items.Count is 0)
            {
                throw new InvalidDataException("Inventory contains no items.");
            }

            var duplicateItems = items.GroupBy(i => i.Name).Where(g => g.Count() > 1).ToList();
            if (duplicateItems.Count > 0)
            {
                throw new InvalidDataException($"Duplicate item names found: \"{string.Join("\", \"", duplicateItems.Select(g => g.Key))}\".");
            }

            var orderedItems = items.OrderBy(i => i.Name, StringComparer.InvariantCulture).ToList();

            return new Inventory(orderedItems);
        }
    }
}
