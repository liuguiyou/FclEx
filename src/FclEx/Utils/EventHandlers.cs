using System.Threading.Tasks;

namespace FclEx.Utils
{
    public delegate Task AsyncEventHandler<in TSender, in TEventArgs>(TSender sender, TEventArgs e);

    public delegate void EventHandler<in TSender, in TEventArgs>(TSender sender, TEventArgs e);
}
