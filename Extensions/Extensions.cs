
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Roundtable
{
    public static class Extensions
    {
        public static async Task DownloadFile(this HttpClient client, string address, string fileName)
        {
            using (var response = await client.GetAsync(address))
            using (var stream = await response.Content.ReadAsStreamAsync())
            using (var file = File.OpenWrite(fileName))
            {
                stream.CopyTo(file);
            }
        }
    }
    public static class MyWpfExtensions
    {
        public static System.Windows.Forms.IWin32Window GetIWin32Window(this System.Windows.Media.Visual visual)
        {
            var source = System.Windows.PresentationSource.FromVisual(visual) as System.Windows.Interop.HwndSource;
            System.Windows.Forms.IWin32Window win = new OldWindow(source.Handle);
            return win;
        }

        private class OldWindow : System.Windows.Forms.IWin32Window
        {
            private readonly System.IntPtr _handle;
            public OldWindow(System.IntPtr handle)
            {
                _handle = handle;
            }

            #region IWin32Window Members
            System.IntPtr System.Windows.Forms.IWin32Window.Handle
            {
                get { return _handle; }
            }
            #endregion
        }
    }
}
