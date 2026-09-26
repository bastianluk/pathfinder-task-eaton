using EatonPathfinder.Entities;

namespace EatonPathfinder
{
    internal class Program
    {
        public static int Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            try
            {
                return Run(args);
            }
            catch (ArgumentException e)
            {
                Console.Error.WriteLine($"Invalid input. Details: {e.Message}");
                return 1;
            }
            catch (FileNotFoundException e)
            {
                Console.Error.WriteLine($"File not found. Details: {e.Message}");
                return 1;
            }
            catch (InvalidDataException e)
            {
                Console.Error.WriteLine($"Invalid data. Details: {e.Message}");
                return 1;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"An unexpected error occurred. Details: {e.Message}");
                return 1;
            }
        }

        private static int Run(string[] args)
        {
            var lines = GetLinesFromFile(args);

            var inventory = InventoryParser.ParseLines(lines);

            PrintInventory(inventory);
            var selection = GetUserSelection(inventory.Items.Count);

            if (!selection.HasValue)
            {
                Console.WriteLine("Empty input. Exiting.");
                return 0;
            }

            var selectedItem = inventory.Items[selection.Value - 1];
            ShowPathToItem(selectedItem);

            return 0;
        }

        private static string[] GetLinesFromFile(string[] args)
        {
            var filePath = GetFilePath(args);
            var lines = File.ReadAllLines(filePath);

            if (lines.Length == 0)
            {
                throw new InvalidDataException("The provided file is empty.");
            }

            return lines;
        }

        private static string GetFilePath(string[] args)
        {
            var isValidInput = args.Length == 1;
            if (!isValidInput)
            {
                throw new ArgumentException("Please provide a single file path as an argument.");
            }

            var filePath = args[0];
            var fileExists = File.Exists(filePath);
            if (!fileExists)
            {
                throw new FileNotFoundException($"The specified file was not found at path {filePath}.", filePath);
            }

            return filePath;
        }

        private static void PrintInventory(Inventory inventory)
        {
            Console.WriteLine("Available items:");
            Console.WriteLine();

            foreach ((var item, var inputIndex) in inventory.Items.Select((item, index) => (item, index + 1)))
            {
                Console.WriteLine($"[{inputIndex}] - {item.Name}");
            }
        }

        private static int? GetUserSelection(int maxIndex)
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("What item would you like to search for?");

                var input = Console.ReadLine();

                var emptyInput = string.IsNullOrEmpty(input);
                if (emptyInput)
                {
                    return null;
                }

                if (int.TryParse(input, out var selection) && selection >= 1 && selection <= maxIndex)
                {
                    return selection;
                }

                Console.WriteLine();
                Console.WriteLine($"Invalid selection. Please try again (1-{maxIndex}).");
            }
        }

        private static void ShowPathToItem(Item item)
        {
            Console.WriteLine();
            Console.WriteLine(string.Join("\n", item.Path.Steps));
        }
    }
}
