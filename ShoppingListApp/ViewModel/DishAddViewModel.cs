﻿using MauiApp2.Model;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
namespace MauiApp2.ViewModel;
using Microsoft.Maui.Controls;
using System.ComponentModel;

class DishAddViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnNavigatedTo(Microsoft.Maui.Controls.NavigatedToEventArgs args)
    {
        CompleteReset();
    }
    public ObservableCollection<IngredientList> Ingredients { get; set; } = new ObservableCollection<IngredientList>();

    internal DataBase db = new DataBase();
    public string DishName { get; set; }
    public string IngredientAmount { get; set; }

    public string IngredientMeasure {  get; set; }

    public string IngredientName { get; set; }
    public IngredientList SelectedIngredient { get; set; }

    public DishAddViewModel()
    {
        AddIngredientToList = new AsyncRelayCommand(AddIngredients);
        AddDish = new AsyncRelayCommand(DishAdd);
        RemoveIngredientFromList = new AsyncRelayCommand(RemoveIngredient);
    }

    public ICommand AddIngredientToList { get; }
    public ICommand AddDish { get; }
    public ICommand RemoveIngredientFromList {  get; }

    private async Task AddIngredients()
    {
        var currentDish = 0;
        try {  currentDish = db.GetLastDishId(); }
        catch {}
        var ingList = new IngredientList(currentDish + 1,IngredientName,Convert.ToDouble(IngredientAmount),IngredientMeasure);
        Ingredients.Add(ingList);
        IngredientInputReset();
    }

    private async Task RemoveIngredient()
    {
        if (Ingredients.Contains(SelectedIngredient))
        {
            Ingredients.Remove(SelectedIngredient);
        }
    }

    private async Task DishAdd()
    {
        db.AddDish(new Dish(DishName));
        var temp = Ingredients.ToList();
        foreach (IngredientList item in Ingredients)
        {
            db.AddIngredientList(item);
        }       
        CompleteReset();
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
    //Zrób osobno kontrolki do dodawania komponentów, a poniżej ich liste. I jak klikasz "dodaj składnik" do jest dodawany do listy
}
