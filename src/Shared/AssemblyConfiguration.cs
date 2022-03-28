using System.Runtime.CompilerServices;
#if MAUI
using Microsoft.Maui;
using Microsoft.Maui.Controls;
#else 
using Xamarin.Forms;
#endif
[assembly: XmlnsDefinition("http://apes.ge", "APES.UI.XF")]
[assembly: XmlnsPrefix("http://apes.ge", "apes")]
[assembly: InternalsVisibleTo("APES.UI.XF.Tests")]