using CommunityToolkit.Mvvm.Messaging.Messages;

namespace MauiApp2.Messages;

class ShoppingListValueChanged : ValueChangedMessage<string>
{
    public ShoppingListValueChanged(string value) : base(value)
    {
    }
}
