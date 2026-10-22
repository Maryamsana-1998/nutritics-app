using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace NutriticsApp;

public class FoodEntry
{
    public string Name { get; set; } = string.Empty;
    public int Calories { get; set; }
}

public partial class MainWindow : Window
{
    private readonly ObservableCollection<FoodEntry> _entries = new();

    public MainWindow()
    {
        InitializeComponent();
        FoodList.ItemsSource = _entries;
    }

    private void AddButton_Click(object sender, RoutedEventArgs e) => AddEntry();

    private void InputBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            AddEntry();
        }
    }

    private void AddEntry()
    {
        var name = FoodNameBox.Text.Trim();
        if (name.Length == 0 || !int.TryParse(CaloriesBox.Text.Trim(), out var calories))
        {
            return;
        }

        _entries.Add(new FoodEntry { Name = name, Calories = calories });
        FoodNameBox.Clear();
        CaloriesBox.Clear();
        FoodNameBox.Focus();

        TotalText.Text = $"{_entries.Sum(x => x.Calories)} kcal";
    }
}
