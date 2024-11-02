using System.Windows;
using System.Windows.Controls;

namespace RecipeGen
{
    public partial class MainContent : UserControl
    {
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
            addIngredientScreen.CancelRequested += ShowMainContent; // Subscribe to 
        }

        public void ShowMainContent()
        {
            this.Content = new MainContent(); // Reset to the original MainContent
        }
    }
}
