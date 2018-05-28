using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FclEx
{
    public delegate Task AsyncEventHandler<in TSender, in TEventArgs>(TSender sender, TEventArgs e);

    public delegate void EventHandler<in TSender, in TEventArgs>(TSender sender, TEventArgs e);
}
