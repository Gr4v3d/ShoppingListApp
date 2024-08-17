using MauiApp2.Model;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Messaging;
using MauiApp2.Messages;
using MauiApp2.View;
using System.ComponentModel;

namespace MauiApp2.ViewModel;

class DishAddViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    public ObservableCollection<IngredientList> Ingredients { get; set; } = new ObservableCollection<IngredientList>();

    internal DataBase db = new DataBase();
    
    public string DishName { get; set; }
    
    public string IngredientAmount { get; set; }

    public string IngredientMeasure {  get; set; }

    public string IngredientName { get; set; }
    
    public IngredientList SelectedIngredient { get; set; }

    public ICommand AddIngredientToList { get; }
    public ICommand AddDish { get; }
    public ICommand RemoveIngredientFromList { get; }

    public DishAddViewModel()
    {
        AddIngredientToList = new AsyncRelayCommand(AddIngredients);
        AddDish = new AsyncRelayCommand(DishAdd);
        RemoveIngredientFromList = new AsyncRelayCommand(RemoveIngredient);
    }

    //Dodaj danie do listy znanych dań
    private async Task AddIngredients()
    {
        //Sprawdzenie wszystkich danych wejściowych dla składników
        try
        {
            if (IngredientName == null || IngredientName == "") 
                throw new Exception("Nie podano nazwy składniku");
            
            if (IngredientAmount == "" || IngredientAmount == null) 
                throw new Exception("Nie podano wymaganej ilości składniku");
            
            if (!Double.TryParse(IngredientAmount,out var d)) 
                throw new Exception("Podana ilość składniku nie jest liczbą");
        }
        catch(Exception ex)
        {
            var popup = new AlertPopUp(ex.Message);
            Shell.Current.ShowPopup(popup);
            return;
        }
<<<<<<< Updated upstream
        
        var currentDish = 0;
        //Jeśli w bazie danych nie ma żadnych dań wyrzuciłby błąd, dlatego jest w try/catch
        try {  currentDish = db.GetLastDishId(); }
        catch {}
        
        //Stworzenie nowego obiektu składnika i dodania go do listy
        var ingList = new IngredientList(currentDish + 1,IngredientName,Convert.ToDouble(IngredientAmount),IngredientMeasure);
=======
        var ingList = new IngredientList(0,IngredientName,Convert.ToDouble(IngredientAmount),IngredientMeasure);
>>>>>>> Stashed changes
        Ingredients.Add(ingList);
        
        IngredientInputReset();
    }

    //Usuwanie składników z listy
    private async Task RemoveIngredient()
    {
        if (Ingredients.Contains(SelectedIngredient))
            Ingredients.Remove(SelectedIngredient);
    }

    //Dodanie dania do bazy danych
    private async Task DishAdd()
    {
        try
        {
            //Wyłapywanie braku nazwy lub braku składników
            if (DishName == null)
                throw new Exception("Nie podano nazwy dania");
            
            if (Ingredients.Count == 0)
                throw new Exception("Powietrze na talerzu nie liczy się jako danie\nProszę dodać składniki");
            
            //Dodanie do bazy danych
            db.AddDish(new Dish(DishName));
<<<<<<< Updated upstream
            
            var temp = Ingredients.ToList();
            
            //Dodawanie składników do bazy danych
=======
            var dishID = db.GetLastDishId();
>>>>>>> Stashed changes
            foreach (IngredientList item in Ingredients)
            {
                item.DishId = dishID;
                db.AddIngredientList(item);
            }
            
            //Messenger powiadamiający stronę DishView aby załadowała swoją zawartość na nowo
            WeakReferenceMessenger.Default.Send(new NewDishAdded("Reload"));
            CompleteReset();
        }
        catch(Exception ex)
        {
            var popup = new AlertPopUp(ex.Message);
            Shell.Current.ShowPopup(popup);
        }
    }
    private void CompleteReset()
    {
        Ingredients = new ObservableCollection<IngredientList>();
        DishName = "";

        PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs("Ingredients"));
        PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs("DishName"));

        IngredientInputReset();
    }
    private void IngredientInputReset()
    {
        IngredientAmount = "";
        IngredientName = "";
        IngredientMeasure = "";

        PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs("IngredientAmount"));
        PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs("IngredientName"));
        PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs("IngredientMeasure"));
    }
}
