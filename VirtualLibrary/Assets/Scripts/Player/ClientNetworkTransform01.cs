using Unity.Netcode.Components;


public class ClientNetworkTransform01 : NetworkTransform
{
    protected override bool OnIsServerAuthoritative()
    {
        return false;

    }
}
