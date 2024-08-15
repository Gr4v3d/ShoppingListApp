using CommunityToolkit.Mvvm.Messaging.Messages;

namespace MauiApp2.Messages;

internal class NewElementsInShoppingList : ValueChangedMessage<string>
{
    public NewElementsInShoppingList(string value) : base(value)
    {
    }
}
