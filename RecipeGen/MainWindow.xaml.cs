using System.Windows;
using System.Windows.Controls;

namespace RecipeGen
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

<<<<<<< Updated upstream
        // Event handler for the "Add Recipe" button in the "Recipes" tab
        private void AddRecipeButton_Click(object sender, RoutedEventArgs e)
        {
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
            // Create a new TextBlock or custom control to represent an ingredient item
            TextBlock newIngredient = new TextBlock
            {
                Text = "New Ingredient",
                Margin = new Thickness(5),
                Foreground = (System.Windows.Media.Brush)Resources["TextColor"]
            };

            // Add it to the IngredientListPlaceholder StackPanel
            IngredientListPlaceholder.Children.Add(newIngredient);
=======
        private void AddRecipeButton_Click(object sender, RoutedEventArgs e)
        {
            var addRecipeScreen = new AddRecipeScreen();
            this.Content = addRecipeScreen;
            addRecipeScreen.CancelRequested += ShowMainContent; // Subscribe to event
        }

        private void AddIngredientButton_Click(object sender, RoutedEventArgs e)
        {
            var addIngredientScreen = new AddIngredientScreen();
            this.Content = addIngredientScreen;
            addIngredientScreen.CancelRequested += ShowMainContent; // Subscribe to event
        }

        public void ShowMainContent()
        {
            // Optionally, you can add logic to show the main content here.
            var mainContent = new MainContent(); // This assumes you have a MainContent UserControl
            this.Content = mainContent; // Or switch back to your original layout
>>>>>>> Stashed changes
        }
    }
}
