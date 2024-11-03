using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace RecipeGen
{
    public partial class AddRecipeScreen : UserControl
    {
        public event Action CancelRequested;
        public event Action AddRecipe;

        public AddRecipeScreen()
        {
            InitializeComponent();
            this.AddRecipe += RecipeQuery;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            CancelRequested?.Invoke(); // Raise the event to go back
        }

        private void AddRecipeButton_Click(object sender, RoutedEventArgs e)
        {
            AddRecipe?.Invoke();
        }

        private void RecipeQuery()
        {
            string connectionStr = $"Data Source={MainContent.database_path};Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionStr))
            {
                connection.Open();
                string RecipeTitle = RecipeTitleTextBox.Text.ToLower();
                string RecipeURL = RecipeURLTextBox.Text;
                string RecipeIngredients = RecipeIngredientsTextBox.Text.ToLower();
                string[] ingredients = RecipeIngredients.Split(',');

                // Insert recipe
                string insert_recipe = "INSERT INTO recipe (title, url) VALUES (@title, @url)";
                using (SQLiteCommand command = new SQLiteCommand(insert_recipe, connection))
                {
                    command.Parameters.AddWithValue("@title", RecipeTitle);
                    command.Parameters.AddWithValue("@url", RecipeURL);
                    command.ExecuteNonQuery();
                }

                long recipe_id = connection.LastInsertRowId;

                foreach (string ingredient in ingredients)
                {
                    long ingredient_id;
                    string trimmedIngredient = ingredient.Trim();

                    // Check if ingredient exists
                    string checkQuery = "SELECT iid FROM ingredients WHERE name = @name;";
                    using (SQLiteCommand checkCommand = new SQLiteCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@name", trimmedIngredient);
                        var result = checkCommand.ExecuteScalar();

                        if (result == null) // Ingredient not in table; insert it
                        {
                            string insertQuery = "INSERT INTO ingredients (name) VALUES (@name);";
                            using (SQLiteCommand insertCommand = new SQLiteCommand(insertQuery, connection))
                            {
                                insertCommand.Parameters.AddWithValue("@name", trimmedIngredient);
                                insertCommand.ExecuteNonQuery();
                            }
                            ingredient_id = connection.LastInsertRowId;
                        }
                        else // Ingredient exists; get its iid
                        {
                            ingredient_id = (long)result;
                        }
                    }

                    // Insert into recipe_ingredient, ignoring duplicates
                    string insert_recipe_ingredient = @"
                INSERT OR IGNORE INTO recipe_ingredient (ingredient_id, recipe_id) 
                VALUES (@iid, @rid);";
                    using (SQLiteCommand insertCommand = new SQLiteCommand(insert_recipe_ingredient, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@iid", ingredient_id);
                        insertCommand.Parameters.AddWithValue("@rid", recipe_id);
                        insertCommand.ExecuteNonQuery();
                    }
                }

                connection.Close();
            }

            CancelRequested?.Invoke(); // Raise event to go back
        }

        private void RecipeTitleTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (RecipeTitleTextBox.Text == "Title")
            {
                RecipeTitleTextBox.Text = string.Empty;
            }
        }

        private void RecipeTitleTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(RecipeTitleTextBox.Text))
            {
                RecipeTitleTextBox.Text = "Title";
            }
        }
        private void RecipeURLTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (RecipeURLTextBox.Text == "URL")
            {
                RecipeURLTextBox.Text = string.Empty;
            }
        }

        private void RecipeURLTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(RecipeURLTextBox.Text))
            {
                RecipeURLTextBox.Text = "URL";
            }
        }
        private void RecipeIngredientsTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (RecipeIngredientsTextBox.Text == "Ingredients")
            {
                RecipeIngredientsTextBox.Text = string.Empty;
            }
        }

        private void RecipeIngredientsTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(RecipeIngredientsTextBox.Text))
            {
                RecipeIngredientsTextBox.Text = "Ingredients";
            }
        }
    }
}
