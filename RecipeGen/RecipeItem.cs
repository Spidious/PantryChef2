public class RecipeItem
{
    public string Name { get; set; } // Name of the recipe
    public string Url { get; set; }  // URL for more details (if needed)

    public RecipeItem(string name, string url)
    {
        Name = name;
        Url = url;
    }
}
