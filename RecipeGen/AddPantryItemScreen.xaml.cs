using System;
using System.CodeDom;
using System.Windows;
using System.Windows.Controls;
using System.Data.SQLite;

namespace RecipeGen
{
    public partial class AddPantryItemScreen : UserControl
    {
        public event Action? CancelRequested;
        public event Action AddPantryItem;

        public AddPantryItemScreen()
        {
            InitializeComponent();
            this.AddPantryItem += PantryItemQuery;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            CancelRequested?.Invoke(); // Raise the event to go back
        }

        private void AddPantryItemButton_Click(object sender, RoutedEventArgs e)
        {
            AddPantryItem?.Invoke();
        }

        private void PantryItemQuery()
        {
            // Get text entry
            string textEntry = IngredientNameTextBox.Text;

            // Break at commas, separating into an array, and trim each item
            string[] inputArray = textEntry.Split(", ").Select(item => item.Trim()).ToArray();

            // Back into one string formatted properly
            string formattedItems = string.Join(", ", inputArray.Select(item => $"'{item}'"));

            // Create the SQL query
            string query = $@"
                INSERT INTO recipe_ingredient (ingredient_id, recipe_id)
                SELECT iid, 1
                FROM ingredients
                WHERE name IN ({formattedItems});
            ";

            // Run the query in the Database
            using (var connection = new SQLiteConnection("Data Source=C:\\Users\\luked\\Documents\\recipegen\\RecipeGen\\Data\\database.db"))
            {
                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }

            CancelRequested?.Invoke();

        }
    }
}
