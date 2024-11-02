using System.Windows;
using System.Windows.Controls;
using System.Data.SQLite;

namespace RecipeGen
{
    public partial class AddIngredientScreen : UserControl
    {
        public event Action CancelRequested;
        public event Action AddIngredient;

        public AddIngredientScreen()
        {
            InitializeComponent();
            this.AddIngredient += IngredientQuery;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            CancelRequested?.Invoke(); // Raise the event to go back
        }

        private void AddIngredientButton_Click(object sender, RoutedEventArgs e)
        {
            AddIngredient?.Invoke(); 
        }

        private void IngredientQuery()
        {
            string connectionStr = $"Data Source=;Version =3;";// C: \Users\adr3wb\Source\Repos\RecipeGen\RecipeGen\Data\database.db
            using (SQLiteConnection connection = new SQLiteConnection(connectionStr))
            {
                connection.Open();
                string input = IngredientNameTextBox.Text;
                input = input.ToLower();
                string[] ingredients = input.Split(',');
                for (int i = 0; i < ingredients.Length; i++)
                {
                    string checkQuery = "SELECT COUNT(*) FROM ingredients WHERE name = @name;";
                    using (SQLiteCommand checkCommand = new SQLiteCommand(checkQuery, connection))
                    {
                        input = ingredients[i].Trim();
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
                        }
                    }
                }

                connection.Close();
            }
            CancelRequested?.Invoke();
        }
    }
}
