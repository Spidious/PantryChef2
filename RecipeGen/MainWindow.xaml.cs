using System.Windows;

namespace RecipeGen
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainContent mainContent = new MainContent(); // Create an instance of MainContent
            MainContentArea.Content = mainContent; // Set MainContent as the content of the window
        }
    }
}
