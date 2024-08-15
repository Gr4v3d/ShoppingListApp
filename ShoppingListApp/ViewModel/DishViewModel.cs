using CommunityToolkit.Mvvm.Input;
using MauiApp2.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Messaging;
using MauiApp2.Messages;
using MauiApp2.View;
using CommunityToolkit.Maui.Views;
namespace MauiApp2.ViewModel;

partial class DishViewModel :INotifyPropertyChanged
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
        WeakReferenceMessenger.Default.Register<DeletionMessage>(this, (r, m) =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                RemoveDishPermanently();
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
        PortionSize = "1";
        PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs("SelectedDish"));
        PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs("PortionSize"));
    }

    public async Task AddToShopList()
    {
        try
        {
            if (SelectedDish == null) 
                throw new Exception("Nie wybrałeś dania do dodania");
            if (!Double.TryParse(PortionSize, out var d))
                throw new Exception("Rozmiar porcji musi być liczbą");
            if (Convert.ToDouble(PortionSize) <=0 )
                throw new Exception("Nie da się fizycznie zrobić ujemnej ilości jedzenia");
            if (PortionSize == null)
                PortionSize = "1";
            var temp = Convert.ToDouble(PortionSize);
            db.AddShoppingList(new ShoppingList(SelectedDish.DishId,temp));
            SelectedDish = null;
            PortionSize = "";
        }
        catch (Exception ex)
        {
            var popup = new AlertPopUp(ex.Message);
            Shell.Current.ShowPopup(popup);
        }
        PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs("SelectedDish"));
        PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs("PortionSize"));
        WeakReferenceMessenger.Default.Send(new NewElementsInShoppingList("Reload"));
    }
    [RelayCommand]

    public void RemoveDishButtonPressed()
    {
        var popup = new DeletionConfirmationPopUp();
        Shell.Current.ShowPopup(popup);
    }
    public void RemoveDishPermanently()
    {
        db.RemoveDishDB(SelectedDish.DishId);
        ReloadList();
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Dishes)));
        WeakReferenceMessenger.Default.Send(new NewElementsInShoppingList("Reload"));
    }
}
