using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging; // Updated for BitmapImage
using System.IO;
using System.Net.Http;
using System.Threading.Tasks; // Added for async Task

namespace RecipeGen
{
    public partial class MainContent : UserControl
    {
        public static string database_path = $"{AppDomain.CurrentDomain.BaseDirectory}\\Data\\database.db";

        public MainContent()
        {
            InitializeComponent();
            LoadRecipes();
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

        private async void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl) // Ensure this event is for the TabControl, not nested controls
            {
                var selectedTab = (sender as TabControl)?.SelectedItem as TabItem;

                // Check the header of each TabItem and call the corresponding function
                if (selectedTab?.Header?.ToString() == "Recipes")
                {
                    await LoadRecipes(); // Await the async method
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
        private async Task LoadRecipes()
        {
            string query = $@"
            SELECT r.title, r.url
            FROM recipe r
            JOIN recipe_ingredient ri ON r.rid = ri.recipe_id
            WHERE r.rid != 1
            GROUP BY r.rid, r.title, r.url
            HAVING COUNT(DISTINCT ri.ingredient_id) = (
                SELECT COUNT(DISTINCT ri2.ingredient_id)
                FROM recipe_ingredient ri2
                WHERE ri2.recipe_id = r.rid
            ) 
            AND COUNT(DISTINCT ri.ingredient_id) = (
                SELECT COUNT(DISTINCT ri3.ingredient_id)
                FROM recipe_ingredient ri3
                WHERE ri3.recipe_id = 1
                AND ri3.ingredient_id IN (
                    SELECT ri4.ingredient_id
                    FROM recipe_ingredient ri4
                    WHERE ri4.recipe_id = r.rid
                )
            );";

            List<RecipeItem> recipes = new List<RecipeItem>();

            using (var connection = new SQLiteConnection($"Data Source={MainContent.database_path}"))
            {
                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string title = reader["title"].ToString();
                            string url = reader["url"].ToString();

                            // Fetch the first image from the recipe URL
                            var recipeImage = await FetchFirstImageFromUrl(url);

                            // Create a new RecipeItem and add it to the list
                            var recipeItem = new RecipeItem(title, url, recipeImage);
                            recipes.Add(recipeItem);
                        }
                    }
                }
            }

            // Set the ItemsSource to the new list of RecipeItems
            RecipeData.ItemsSource = recipes;
        }

        private async Task<ImageSource> FetchFirstImageFromUrl(string url)
        {
            // Ensure the URL is valid and absolute
            if (string.IsNullOrWhiteSpace(url) || !Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                Console.WriteLine("Invalid URL: " + url);
                return GetDefaultImage(); // Return the default image for invalid URLs
            }

            using (var httpClient = new HttpClient())
            {
                try
                {
                    // Get the HTML from the URL
                    var htmlResponse = await httpClient.GetStringAsync(url);
                    var htmlDoc = new HtmlAgilityPack.HtmlDocument();
                    htmlDoc.LoadHtml(htmlResponse);

                    // Attempt to find images using various XPath expressions
                    var xPaths = new List<string>
            {
                "//img[contains(@class, 'rec-photo')]",
                "//div[contains(@class, 'image-container')]//img",
                "//div[contains(@class, 'image')]//img",
                "//img[contains(@alt, 'recipe')]",
                "//img" // Fallback to any image if none of the above are found
            };

                    foreach (var xPath in xPaths)
                    {
                        var imgNode = htmlDoc.DocumentNode.SelectSingleNode(xPath);
                        if (imgNode != null)
                        {
                            string imgSrc = imgNode.GetAttributeValue("src", null);
                            if (!string.IsNullOrEmpty(imgSrc))
                            {
                                // Handle relative URLs if needed
                                if (!imgSrc.StartsWith("http"))
                                {
                                    Uri baseUri = new Uri(url);
                                    imgSrc = new Uri(baseUri, imgSrc).ToString();
                                }

                                // Download the image
                                var imageData = await httpClient.GetByteArrayAsync(imgSrc);
                                using (var ms = new MemoryStream(imageData))
                                {
                                    var bitmap = new BitmapImage();
                                    bitmap.BeginInit();
                                    bitmap.StreamSource = ms;
                                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                                    bitmap.EndInit();
                                    bitmap.Freeze(); // Make it cross-thread accessible
                                    return bitmap; // Return the first valid image found
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine($"No image found with XPath: {xPath}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error fetching image: " + ex.Message);
                }
            }

            return GetDefaultImage(); // Return the default image if no image is found or an error occurs
        }

        private ImageSource GetDefaultImage()
        {
            // Load the default image from the specified URL
            var uri = "https://thumb.ac-illust.com/13/13ad2992478f065de05c97423b5ef5e1_t.jpeg";
            var bitmap = new BitmapImage();

            try
            {
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(uri, UriKind.Absolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                // No need to freeze the bitmap for the default image
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading default image: " + ex.Message);
            }

            return bitmap; // Return the bitmap without freezing it
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
            // Clear previous items
            PantryData.Items.Clear();

            // Create a list to hold the ingredient names
            List<string> recipes = new List<string>();

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
                            recipes.Add(value);
                        }
                    }
                }
            }

            // Now populate the IngredientsListPlaceholder with ListBoxItems for each ingredient
            foreach (var recipe in recipes)
            {
                // Create a ListBoxItem for each ingredient
                ListBoxItem recipeItem = new ListBoxItem
                {
                    Content = recipe,
                    Foreground = (Brush)FindResource("TextColor"), // Use the TextColor from resources
                    Margin = new Thickness(0, 5, 0, 0) // Add some margin for spacing
                };

                // Add the ListBoxItem to the IngredientsListPlaceholder
                PantryData.Items.Add(recipeItem);
            }
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
                connection.Close();
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

        private void PantryData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PantryData.SelectedItem is ListBoxItem selectedItem)
            {
                // Get the selected ingredient
                string selectedIngredient = selectedItem.Content.ToString();

                // Do something with the selected ingredient
                Console.WriteLine($"Selected Ingredient: {selectedIngredient}");
            }
        }

        private void RecipeData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RecipeData.SelectedItem is ListBoxItem selectedItem)
            {
                // Get the selected ingredient
                string selectedIngredient = selectedItem.Content.ToString();

                // Do something with the selected ingredient
                Console.WriteLine($"Selected Ingredient: {selectedIngredient}");
            }
        }

        private void Ingredients_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string input = SearchIngredientsTextBox.Text;
                input.ToLower();

                string query = "SELECT * FROM ingredients WHERE name LIKE @name;";

                // Create a list to hold the ingredient names
                List<string> ingredients = new List<string>();

                IngredientsListPlaceholder.Items.Clear();
                // Run the query in the Database
                using (var connection = new SQLiteConnection($"Data Source={MainContent.database_path}"))
                {
                    connection.Open();

                    using (var command = new SQLiteCommand(query, connection))
                    {
                        // Execute the command and get a reader
                        command.Parameters.AddWithValue("@name", "%" + input + "%");
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            // Read the data
                            while (reader.Read())
                            {
                                // Get the ingredient name and add it to the list
                                string value = reader["name"].ToString(); // Ensure to convert to string
                                ingredients.Add(value);
                            }
                        }
                    }

                    connection.Close();
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
        }

        private void Pantry_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string query = $@"
                    SELECT i.name 
                    FROM ingredients i 
                    JOIN recipe_ingredient ri ON i.iid = ri.ingredient_id
                    JOIN recipe r ON ri.recipe_id = r.rid
                    WHERE r.rid = 1 AND i.name LIKE @name;";
                // Clear previous items
                string input = SearchPantryTextBox.Text;
                input.ToLower();

                PantryData.Items.Clear();

                // Create a list to hold the ingredient names
                List<string> recipes = new List<string>();

                // Run the query in the Database
                using (var connection = new SQLiteConnection($"Data Source={MainContent.database_path}"))
                {
                    connection.Open();
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@name", "%" + input + "%");
                        // Execute the command and get a reader
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            // Read the data
                            while (reader.Read())
                            {
                                // Get the ingredient name and add it to the list
                                var value = reader["name"].ToString(); // Ensure to convert to string
                                recipes.Add(value);
                            }
                        }
                    }
                    connection.Close();
                }



                // Now populate the IngredientsListPlaceholder with ListBoxItems for each ingredient
                foreach (var recipe in recipes)
                {
                    // Create a ListBoxItem for each ingredient
                    ListBoxItem recipeItem = new ListBoxItem
                    {
                        Content = recipe,
                        Foreground = (Brush)FindResource("TextColor"), // Use the TextColor from resources
                        Margin = new Thickness(0, 5, 0, 0) // Add some margin for spacing
                    };

                    // Add the ListBoxItem to the IngredientsListPlaceholder
                    PantryData.Items.Add(recipeItem);
                }
            }

        }

        private void Recipe_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string query = $@"
                    SELECT r.title, r.url
                    FROM recipe r
                    JOIN recipe_ingredient ri ON r.rid = ri.recipe_id
                    WHERE r.rid != 1
                    GROUP BY r.rid, r.title, r.url
                    HAVING COUNT(DISTINCT ri.ingredient_id) = (
                        SELECT COUNT(DISTINCT ri2.ingredient_id)
                        FROM recipe_ingredient ri2
                        WHERE ri2.recipe_id = r.rid
                    ) 
                    AND COUNT(DISTINCT ri.ingredient_id) = (
                        SELECT COUNT(DISTINCT ri3.ingredient_id)
                        FROM recipe_ingredient ri3
                        WHERE ri3.recipe_id = 1
                        AND ri3.ingredient_id IN (
                            SELECT ri4.ingredient_id
                            FROM recipe_ingredient ri4
                            WHERE ri4.recipe_id = r.rid
                        )
                    ) AND r.title LIKE @name;";

                string input = SearchRecipeTextBox.Text;
                input.ToLower();


            }
        }
    }
}
