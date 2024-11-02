using System.Windows;
using System.Windows.Controls;

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
            // Logic to add an ingredient
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            CancelRequested?.Invoke(); // Raise the event to go back
        }

        private void RecipeQuery()
        {
            CancelRequested?.Invoke(); // Raise event to go back
        }
    }
}
