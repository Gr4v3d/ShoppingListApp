using CommunityToolkit.Mvvm.Input;
using MauiApp2.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Messaging;
using Windows.ApplicationModel.VoiceCommands;
using MauiApp2.Messages;
namespace MauiApp2.ViewModel;

class DishViewModel :INotifyPropertyChanged
{
    internal DataBase db = new DataBase();

    public event PropertyChangedEventHandler PropertyChanged;

    public ICommand ClearPortionSize { get; }

    public ICommand AddToShoppingList { get; }

    public Dish SelectedDish { get; set; }

    public string PortionSize { get; set; }

    public ObservableCollection<Dish> Dishes { get; set; }

    public DishViewModel()
    {
        WeakReferenceMessenger.Default.Register<NewDishAdded>(this, (r, m) =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                ReloadList();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Dishes)));
            });
        });
        AddToShoppingList = new AsyncRelayCommand(AddToShopList);
        ClearPortionSize = new AsyncRelayCommand(PortionClear);
        ReloadList();
    }

    private void ReloadList()
    {
        Dishes = new ObservableCollection<Dish>();
        var allDishes = db.GetAllDishes();
        foreach (var dish in allDishes)
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
        WeakReferenceMessenger.Default.Send(new NewElementsInShoppingList("Reload"));
    }
}
