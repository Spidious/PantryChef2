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
            string connectionStr = $"Data Source= {MainContent.database_path};Version =3;";// C: \Users\adr3wb\Source\Repos\RecipeGen\RecipeGen\Data\database.db
            using (SQLiteConnection connection = new SQLiteConnection(connectionStr))
            {
                connection.Open();
                string RecipeTitle = RecipeTitleTextBox.Text;
                RecipeTitle= RecipeTitle.ToLower();
                string RecipeURL = RecipeURLTextBox.Text;
                string RecipeIngredients = RecipeIngredientsTextBox.Text ;
                RecipeIngredients = RecipeIngredients.ToLower();
                string[] ingredients = RecipeIngredients.Split(',');
                string insert_recipe = "INSERT INTO recipe (title,url) VALUES (@title,@url)";
                using (SQLiteCommand command = new SQLiteCommand(insert_recipe, connection))
                {
                    command.Parameters.AddWithValue("@title", RecipeTitle);
                    command.Parameters.AddWithValue("@url", RecipeURL);
                    command.ExecuteNonQuery();
                }
                long recipe_id = connection.LastInsertRowId;

                for (int i = 0; i < ingredients.Length; i++)
                {
                    long ingredient_id;
                    string checkQuery = "SELECT COUNT(*) FROM ingredients WHERE name = @name;";
                    using (SQLiteCommand checkCommand = new SQLiteCommand(checkQuery, connection))
                    {
                        
                        string input = ingredients[i].Trim();
                        checkCommand.Parameters.AddWithValue("@name", input);

                        long count = (long)checkCommand.ExecuteScalar(); // Execute the query and get the count

                        if (count == 0) //if ingredient is not in table insert
                        {
                            string insertQuery = "INSERT INTO ingredients (name) VALUES (@name);";
                            using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
                            {
                                command.Parameters.AddWithValue("@name", input);
                                command.ExecuteNonQuery();
                            }
                            ingredient_id = connection.LastInsertRowId;
                        }
                        else
                        {
                            using (var command = new SQLiteCommand("SELECT iid FROM ingredients WHERE name = @name;", connection))
                            {
                                command.Parameters.AddWithValue("@name", input);
                                //var result = command.ExecuteScalar();
                                ingredient_id = (long)command.ExecuteScalar();
                            }
                        }
                    }

                    string insert_recipe_ingredient = "INSERT INTO recipe_ingredient (ingredient_id, recipe_id) VALUES(@iid,@rid)";
                    using (SQLiteCommand insertCommand = new SQLiteCommand(insert_recipe_ingredient, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@iid", ingredient_id);
                        insertCommand.Parameters.AddWithValue("@rid", recipe_id);
                        insertCommand.ExecuteNonQuery();
                    }
                }
                connection.Clone();
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
