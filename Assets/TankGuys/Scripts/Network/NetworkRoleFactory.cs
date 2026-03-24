public static class NetworkRoleFactory
{
    public static INetworkRole CreateHost()
    {
        return new HostNetwork();
    }

    public static INetworkRole CreateClient()
    {
        return new ClientNetwork();
    }
}