using CommunityToolkit.Mvvm.Input;
using MauiApp2.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
namespace MauiApp2.ViewModel;

class DishViewModel :INotifyPropertyChanged
{
    internal DataBase db = new DataBase();

    public event PropertyChangedEventHandler PropertyChanged;

    public ICommand ClearPortionSize { get; }

    public ICommand AddToShoppingList { get; }

    public Dish SelectedDish { get; set; }

    public string PortionSize { get; set; }

    public ObservableCollection<Dish> Dishes { get; set; } = new ObservableCollection<Dish>();

    public DishViewModel()
    {
        AddToShoppingList = new AsyncRelayCommand(AddToShopList);
        ClearPortionSize = new AsyncRelayCommand(PortionClear);
        var allDishes = db.GetAllDishes();
        foreach(var dish in allDishes)
        {
            Dishes.Add(dish);
        }
    }

    public async Task PortionClear()
    {
        PortionSize = "";
        PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs("SelectedDish"));
        PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs("PortionSize"));
    }

    public async Task AddToShopList()
    {
        try
        {
            if (SelectedDish == null) 
                throw new Exception("You didn't choose a dish to add to your list");
            var temp = Convert.ToDouble(PortionSize);
            db.AddShoppingList(new ShoppingList(SelectedDish.DishId,temp));
            SelectedDish = null;
            PortionSize = "";
        }
        catch (Exception ex)
        {
            PortionSize = ex.Message;
        }
        PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs("SelectedDish"));
        PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs("PortionSize"));
    }
}
