using CommunityToolkit.Mvvm.Messaging.Messages;

namespace MauiApp2.Messages;

public class NewDishAdded : ValueChangedMessage<string>
{
    public NewDishAdded(string value) : base(value)
    {
    }
}
