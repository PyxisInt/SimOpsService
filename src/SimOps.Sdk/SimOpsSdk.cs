using Refit;
using SimOps.Sdk.ApiDefinition;

namespace SimOps.Sdk;

public class SimOpsSdk
{
    public static SimOpsSdk? _sdk = null;
    private string _uri;
    
    private SimOpsSdk()
    {
        _uri = String.Empty;
    }
    
    private SimOpsSdk(string uri)
    {
        _uri = uri;
    }
    
    public static SimOpsSdk GetInstance(string uri)
    {
        if (_sdk == null)
        {
            _sdk = new SimOpsSdk(uri);
        }
        return _sdk;
    } 
    
    /****************** Services ******************************/

    public ILoginService LoginService = RestService.For<ILoginService>(_sdk._uri);
    
    /******************* End of Services **********************/
}