using SQLite;

namespace MauiApp2.Model;

public class Dish
{
    [PrimaryKey, AutoIncrement]
    public int DishId { get; set; }
    [NotNull]
    public string DishName { get; set; }
    public Dish() { }

    public Dish(string dishName)
    {
        DishName = dishName;
    }
}
public class IngredientList
{
    public int DishId { get; set;}
    public string IngredientName { get; set; }
    public double IngredientCount { get; set;}
    public string Measure { get; set; }

    public IngredientList() { }
    public IngredientList(int dishID, string ingredientName, double ingredientCount, string measure) {
        DishId = dishID;
        IngredientName = ingredientName;
        IngredientCount = ingredientCount;
        Measure = measure;
    }
}
public class ShoppingList
{
    [PrimaryKey,AutoIncrement]
    public int ShoppingListID { get; set; }
    [NotNull]
    [Indexed]
    public int DishId { get; set; }
    public double PortionSize { get; set; }
    public ShoppingList() { }

    public ShoppingList(int dishId, double portionSize)
    {
        DishId = dishId;
        PortionSize = portionSize;
    }
}
public class ShoppingListElement
{
    public int ShoppingListID { get; set; }
    public string IngredientName { get; set; }
    public double IngredientAmountOwned { get; set; }
    public ShoppingListElement() { }
    public ShoppingListElement(int Id, string Name, double owned)
    {
        ShoppingListID = Id;
        IngredientName = Name;
        IngredientAmountOwned = owned;
    }

}
internal class DataBase
{
    private SQLiteConnection _connection;

    public DataBase()
    {
        _connection = new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),"Dishes.db"));
        _connection.CreateTable<Dish>();
        _connection.CreateTable<IngredientList>();
        _connection.CreateTable<ShoppingList>();
        _connection.CreateTable<ShoppingListElement>();
    }

    public void AddDish(Dish dish)
    {
        _connection.Insert(dish);
    }

    public void AddShoppingList(ShoppingList list)
    {
        _connection.Insert(list);
        var querry = _connection.Query<ShoppingList>("SELECT ShoppingListID FROM ShoppingList");
        var lastId = querry.Max(x => x.ShoppingListID);

        List<IngredientList> ingredients = _connection.Query<IngredientList>($"SELECT * FROM IngredientList WHERE DishId = {list.DishId}").ToList();

        foreach(var  ingredient in ingredients)
        {
            ShoppingListElement requiredIngredient = new ShoppingListElement(lastId, ingredient.IngredientName,0);
            AddShoppingListElement(requiredIngredient);
        }
    }

    private void AddShoppingListElement(ShoppingListElement element) 
    {
            _connection.Insert(element);
    }

    public int GetLastDishId()
    {
        var querry = _connection.Query<Dish>("SELECT DishId FROM Dish ");
        return querry.Max(x => x.DishId);
    }

    public List<Dish> GetAllDishes()
    {
        var querry = _connection.Query<Dish>("SELECT * FROM Dish");
        return querry.ToList();
    }

    public void AddIngredientList(IngredientList ingredientList)
    {
        _connection.Insert(ingredientList);
    }

    public List<ShoppingList> GetShoppingList()
    {
        var querry = _connection.Query<ShoppingList>("SELECT * FROM ShoppingList ORDER BY ShoppingListID DESC");
        return querry.ToList();
    }

    public List<ShoppingListElement> GetShoppingListElements(int ID)
    {
        var querry = _connection.Query<ShoppingListElement>($"SELECT * FROM ShoppingListElement WHERE ShoppingListID = {ID}");
        return querry.ToList();
    }

    public Dish GetDish(int ID)
    {
        var querry = _connection.Query<Dish>($"SELECT * FROM Dish WHERE DishID = {ID}");
        return querry.Single();
    }

    public IngredientList GetIngredient(int ID, string Name)
    {
        var querry = _connection.Query<IngredientList>($"SELECT * FROM IngredientList WHERE DishID = {ID} AND IngredientName = '{Name}'");
        return querry.Single();
    }
}
