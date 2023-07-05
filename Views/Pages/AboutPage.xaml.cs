using Wpf.Ui.Common.Interfaces;

namespace Roundtable.Views.Pages
{
    /// <summary>
    /// Interaction logic for AboutView.xaml
    /// </summary>
    public partial class AboutPage : INavigableView<ViewModels.AboutViewModel>
    {
        public ViewModels.AboutViewModel ViewModel
        {
            get;
        }


        public AboutPage(ViewModels.AboutViewModel viewModel)
        {
            ViewModel = viewModel;

            InitializeComponent();
        }
    }
}
