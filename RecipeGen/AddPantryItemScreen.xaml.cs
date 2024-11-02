using System;
using System.Windows;
using System.Windows.Controls;

namespace RecipeGen
{
    public partial class AddPantryItemScreen : UserControl
    {
        public event Action? CancelRequested;
        public event Action AddPantryItem;

        public AddPantryItemScreen()
        {
            InitializeComponent();
            this.AddPantryItem += PantryItemQuery;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            CancelRequested?.Invoke(); // Raise the event to go back
        }

        private void AddPantryItemButton_Click(object sender, RoutedEventArgs e)
        {
            string ingredientName = IngredientNameTextBox.Text;

            if (!string.IsNullOrWhiteSpace(ingredientName))
            {
                // Logic to add the ingredient (e.g., saving to a list or database)
                MessageBox.Show($"Ingredient '{ingredientName}' added!"); // Example feedback
            }
            else
            {
                MessageBox.Show("Please enter a valid ingredient name.");
            }
        }

        private void PantryItemQuery()
        {
            CancelRequested?.Invoke();
        }
    }
}
