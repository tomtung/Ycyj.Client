using System.Windows;
using GalaSoft.MvvmLight.Threading;
using Ninject;
using Ycyj.Client.Model;

namespace Ycyj.Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static readonly IKernel Kernel = new StandardKernel(new DefaultNinjectModule());

        static App()
        {
            DispatcherHelper.Initialize();
        }
    }
}