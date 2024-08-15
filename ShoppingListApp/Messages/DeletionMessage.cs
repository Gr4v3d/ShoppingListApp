using CommunityToolkit.Mvvm.Messaging.Messages;

namespace MauiApp2.Messages;

internal class DeletionMessage : ValueChangedMessage<string>
{
    public DeletionMessage(string value) : base(value)
    {
    }
}
