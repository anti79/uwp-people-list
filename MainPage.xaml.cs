using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using UWPApp.ViewModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UWPApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, IView
    {
        public MainViewModel MainViewModel { get { return (MainViewModel)this.DataContext; } }

        public async Task<bool> DisplayConfirmationDialog(string text)
		{
            MessageDialog dialog = new MessageDialog(text);
            dialog.Commands.Add(new UICommand("Yes", null));
            dialog.Commands.Add(new UICommand("No", null));
            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 1;
            var cmd = await dialog.ShowAsync();
            return cmd.Label=="Yes";
		}
        public void DisplayInfoDialog(string text)
		{ 
            MessageDialog messageDialog = new MessageDialog(text);
            messageDialog.ShowAsync();
        }

        public MainPage()
        {
            this.InitializeComponent();
            DataContext = new MainViewModel(this);
        }

		
	}
}
