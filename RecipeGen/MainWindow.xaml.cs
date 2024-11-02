using System.Windows;
using System.Windows.Controls;
using System.Data.SQLite;
using System.Data.Entity;

namespace RecipeGen
{
    public partial class MainWindow : Window
    {
    
        public MainWindow()
        {
            InitializeComponent();
        }

        // Event handler for the "Add Recipe" button in the "Recipes" tab
        private void AddRecipeButton_Click(object sender, RoutedEventArgs e)
        {
            string connectionStr = "Data Source=C:\\Users\\adr3wb\\Source\\Repos\\RecipeGen\\RecipeGen\\Data\\database.db;Version =3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionStr))
            {
                connection.Open(); 
                string insertQuery = "INSERT INTO recipe (title,url) VALUES (@title,@url);";
                using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@title", "pantry");
                    command.Parameters.AddWithValue("@url", "pantry");
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
                // Create a new TextBlock or custom control to represent a recipe item
                TextBlock newRecipe = new TextBlock
            {
                Text = "New Recipe",
                Margin = new Thickness(5),
                Foreground = (System.Windows.Media.Brush)Resources["TextColor"]
            };

            // Add it to the RecipeWidgetsPlaceholder StackPanel
            RecipeWidgetsPlaceholder.Children.Add(newRecipe);
        }

        // Event handler for the "Add Ingredient" button in the "Pantry" tab
        private void AddIngredientButton_Click(object sender, RoutedEventArgs e)
        {
            string connectionStr = "Data Source=C:\\Users\\adr3wb\\Source\\Repos\\RecipeGen\\RecipeGen\\Data\\database.db;Version =3;";// C: \Users\adr3wb\Source\Repos\RecipeGen\RecipeGen\Data\database.db
            string inputText = ingredient_txtbx.Text;
            inputText = inputText.ToLower();
            using (SQLiteConnection connection = new SQLiteConnection(connectionStr))
            {
                connection.Open();

                /*string[] ingredients = {"salt","pepper","olive oil","vegtable oil","flour","sugar","chicken stock","beef stock","canned tomatoes","tomato paste",
                "marinara sauce","black beans","tuna","pasta","white rice","brown rice","lentils","split peas","bread crumbs","potatoes","onions","garlic","balsamice vinegar","soy sauce",
                "hot sauce","worcestershire sauce","dried basil","bay leaves","cayenne or","crushed red pepper flakes","curry powder","seasoned salt","chili powder","cumin","cinnamon",
                "garlic powder","onion powder","oregano","paprika","dried parsley","eggs", "milk","butter","ketchup","mustard","mayonnaise","parmesan cheese","american cheese","provalone cheese",
                "corn","spinach","peas","ground beef","chicken breasts","lemons","fresh ginger","vanilla extract","cornstarch","honey","brown sugar","baking powder","powdered sugar","carrots","limes",
                "bacon","parsley","heavy cream","vinegar"};*/

                /*for(int i = 0; i< ingredients.Length; i++)
                {
                    string checkQuery = "SELECT COUNT(*) FROM ingredients WHERE name = @name;";
                    using (SQLiteCommand checkCommand = new SQLiteCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@name", ingredients[i]);

                        long count = (long)checkCommand.ExecuteScalar(); // Execute the query and get the count

                        if (count == 0)
                        {
                            // Ingredient does not exist, proceed to insert
                            string insertQuery = "INSERT INTO ingredients (name) VALUES (@name);";
                            using (SQLiteCommand insertCommand = new SQLiteCommand(insertQuery, connection))
                            {
                                insertCommand.Parameters.AddWithValue("@name", ingredients[i]);
                                insertCommand.ExecuteNonQuery();
                                //Console.WriteLine("Ingredient inserted successfully.");
                            }
                        }
                        else
                        {
                            // Ingredient already exists
                            //Console.WriteLine("Ingredient already exists in the table.");
                        }
                    }*/

                    string checkQuery = "SELECT COUNT(*) FROM ingredients WHERE name = @name;";
                    using (SQLiteCommand checkCommand = new SQLiteCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@name", inputText);

                        long count = (long)checkCommand.ExecuteScalar(); // Execute the query and get the count

                        if (count == 0)
                        {
                            // Ingredient does not exist, proceed to insert
                            string insertQuery = "INSERT INTO ingredients (name) VALUES (@name);";
                            using (SQLiteCommand insertCommand = new SQLiteCommand(insertQuery, connection))
                            {
                                insertCommand.Parameters.AddWithValue("@name", inputText);
                                insertCommand.ExecuteNonQuery();
                                //Console.WriteLine("Ingredient inserted successfully.");
                            }

                            long ingredient_id = connection.LastInsertRowId;

                            string insert_rec_ingr = "INSERT INTO recipe_ingredient (ingredient_id,recipe_id) VALUES (@ingredient_id,1)";

                            using (SQLiteCommand command = new SQLiteCommand(insert_rec_ingr, connection))
                            {
                            command.Parameters.AddWithValue("@ingredient_id", ingredient_id);
                            command.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            
                        }
                    }

                //string checkQuery = "SELECT COUNT(1) FROM ingredient WHERE "

                /*string insertQuery = "INSERT INTO ingredients (name) VALUES (@name);";
                using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection)) {
                    command.Parameters.AddWithValue("@name", inputText);
                    command.ExecuteNonQuery();
                }

                long ingredient_id = connection.LastInsertRowId;

                string insert_rec_ingr = "INSERT INTO recipe_ingredient (ingredient_id,recipe_id) VALUES (@ingredient_id,1)";

                using (SQLiteCommand command = new SQLiteCommand(insert_rec_ingr, connection))
                {
                    command.Parameters.AddWithValue("@ingredient_id", ingredient_id);
                    command.ExecuteNonQuery();
                }*/
                connection.Close();

            }
                // Create a new TextBlock or custom control to represent an ingredient item
                TextBlock newIngredient = new TextBlock
                {
                    Text = inputText,
                    Margin = new Thickness(5),
                    Foreground = (System.Windows.Media.Brush)Resources["TextColor"]
                };

            // Add it to the IngredientListPlaceholder StackPanel
            //IngredientListPlaceholder.Children.Add(newIngredient);
        }

        
    }

}
