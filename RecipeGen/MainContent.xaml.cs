using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RecipeGen
{
    public partial class MainContent : UserControl
    {

        public static string database_path = $"{AppDomain.CurrentDomain.BaseDirectory}\\Data\\database.db";

        public MainContent()
        {
            InitializeComponent();
        }

        private void AddRecipeButton_Click(object sender, RoutedEventArgs e)
        {
            var addRecipeScreen = new AddRecipeScreen();
            this.Content = addRecipeScreen;
            addRecipeScreen.CancelRequested += ShowMainContent; // Subscribe to event
        }

        private void AddPantryItemButton_Click(object sender, RoutedEventArgs e)
        {
            var addPantryItemScreen = new AddPantryItemScreen();
            this.Content = addPantryItemScreen;
            addPantryItemScreen.CancelRequested += ShowMainContent; // Subscribe to event
        }

        private void AddIngredientButton_Click(object sender, RoutedEventArgs e)
        {
            var addIngredientScreen = new AddIngredientScreen();
            this.Content = addIngredientScreen;
            addIngredientScreen.CancelRequested += ShowMainContent; // Subscribe to event
        }

        public void ShowMainContent()
        {
            this.Content = new MainContent(); // Reset to the original MainContent
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl) // Ensure this event is for the TabControl, not nested controls
            {
                var selectedTab = (sender as TabControl)?.SelectedItem as TabItem;

                // Check the header of each TabItem and call the corresponding function
                if (selectedTab?.Header?.ToString() == "Recipes")
                {
                    LoadRecipes();
                }
                else if (selectedTab?.Header?.ToString() == "Pantry")
                {
                    LoadPantry();
                }
                else if (selectedTab?.Header?.ToString() == "Ingredients")
                {
                    LoadIngredients();
                }
            }
        }

        // Functions for loading data
        private void LoadRecipes()
        {
            string query = $@"
                    SELECT r.title, r.url
                    FROM recipe r
                    JOIN recipe_ingredient ri ON r.rid = ri.recipe_id
                    WHERE r.rid != 1
                    AND ri.ingredient_id IN (
                        SELECT ingredient_id
                        FROM recipe_ingredient
                        WHERE recipe_id = 1
                    )
                    GROUP BY r.rid, r.title, r.url
                    HAVING COUNT(DISTINCT ri.ingredient_id) = (
                        SELECT COUNT(*)
                        FROM recipe_ingredient
                        WHERE recipe_id = 1
                    );
            ";
        }

        private void LoadPantry()
        {
            string query = $@"
                    SELECT i.name 
                    FROM ingredients i 
                    JOIN recipe_ingredient ri ON i.iid = ri.ingredient_id
                    JOIN recipe r ON ri.recipe_id = r.rid
                    WHERE r.rid = 1;
            ";


        }

        private void LoadIngredients()
        {
            string query = @"
                    SELECT name FROM ingredients;
            ";

            // Clear previous items
            IngredientsListPlaceholder.Items.Clear();

            // Create a list to hold the ingredient names
            List<string> ingredients = new List<string>();

            // Run the query in the Database
            using (var connection = new SQLiteConnection($"Data Source={MainContent.database_path}"))
            {
                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    // Execute the command and get a reader
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        // Read the data
                        while (reader.Read())
                        {
                            // Get the ingredient name and add it to the list
                            var value = reader["name"].ToString(); // Ensure to convert to string
                            ingredients.Add(value);
                        }
                    }
                }
            }

            // Now populate the IngredientsListPlaceholder with ListBoxItems for each ingredient
            foreach (var ingredient in ingredients)
            {
                // Create a ListBoxItem for each ingredient
                ListBoxItem ingredientItem = new ListBoxItem
                {
                    Content = ingredient,
                    Foreground = (Brush)FindResource("TextColor"), // Use the TextColor from resources
                    Margin = new Thickness(0, 5, 0, 0) // Add some margin for spacing
                };

                // Add the ListBoxItem to the IngredientsListPlaceholder
                IngredientsListPlaceholder.Items.Add(ingredientItem);
            }
        }

        private void IngredientsListPlaceholder_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IngredientsListPlaceholder.SelectedItem is ListBoxItem selectedItem)
            {
                // Get the selected ingredient
                string selectedIngredient = selectedItem.Content.ToString();

                // Do something with the selected ingredient
                Console.WriteLine($"Selected Ingredient: {selectedIngredient}");
            }
        }



    }
}
