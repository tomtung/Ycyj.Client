using System.Diagnostics.CodeAnalysis;
using Ninject;
using Ycyj.Client.Model;

namespace Ycyj.Client.ViewModel
{
    public class ViewModelLocator
    {
        private static MainViewModel _main;
        private static readonly IKernel Kernel = new StandardKernel(new DefaultNinjectModule());

        public ViewModelLocator()
        {
            CreateMain();
        }

        public static MainViewModel MainStatic
        {
            get
            {
                if (_main == null)
                    CreateMain();
                return _main;
            }
        }

        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public MainViewModel Main
        {
            get { return MainStatic; }
        }

        public static void ClearMain()
        {
            _main.Cleanup();
            _main = null;
        }

        public static void CreateMain()
        {
            if (_main == null)
                _main = Kernel.Get<MainViewModel>();
        }

        public static void Cleanup()
        {
            ClearMain();
        }
    }
}