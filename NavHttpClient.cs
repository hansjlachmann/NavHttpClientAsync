using System.Runtime.InteropServices;

namespace NavHttpClientAsync
{
    [Guid("2fb45a77-9e85-431d-9ac9-d030107deebd")]
    [ComVisible(true)]
    public interface INavHttpClient
    {
       string HelloWorld();
    }

    [Guid("79b05537-33c7-4f24-be8b-581373154e71")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComVisible(true)]
    [ProgId("NavHttpClientAsync.NavHttpClient")]
    public class NavHttpClient : INavHttpClient
    {
        public string HelloWorld()
        {
            return "Hello World!";
        }
    }
}
