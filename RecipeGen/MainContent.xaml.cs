using System.Windows;
using System.Windows.Controls;

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
            // Add code to load recipes
        }

        private void LoadPantry()
        {
            // Add code to load pantry items
        }

        private void LoadIngredients()
        {
            // Add code to load ingredients
        }

    }
}
