using System.Runtime.InteropServices;

namespace NavHttpClientAsync
{
    [Guid("4c301f44-63e0-11ee-8c99-0242ac120002")]
    [ComVisible(true)]
    public interface INavHttpClient
    {
       string HelloWorld();
    }

    [Guid("3543cf38-63e0-11ee-8c99-0242ac120002")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComVisible(true)]
    public class NavHttpClient : INavHttpClient
    {
        public string HelloWorld()
        {
            return "Hello World!";
        }
    }
}
