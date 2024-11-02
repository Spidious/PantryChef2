using System.Windows;
using System.Windows.Controls;

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
            CancelRequested?.Invoke();
        }
    }
}
