using System.IO;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Newtonsoft.Json;
using TLCGen.Messaging.Messages;

namespace TLCGen.Helpers
{
    public class ObservableObjectEx : ObservableObject
    {
        public void OnPropertyChanged([CallerMemberName] string propertyName = "", bool broadcast = true)
        {
            if (broadcast) WeakReferenceMessenger.Default.Send(new BroadcastMessage(this));
            base.OnPropertyChanged(propertyName);
        }
    }

    public static class DeepCloner
    {
        public static T DeepClone<T>(T obj)
        {
            using (var ms = new MemoryStream())
            {
                var json = JsonConvert.SerializeObject(obj);
                return JsonConvert.DeserializeObject<T>(json);
            }
        }
    }
}
