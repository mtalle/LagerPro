namespace LagerPro.Api.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class RequireTilgangAttribute : Attribute
{
    public int RessursId { get; }

    public RequireTilgangAttribute(int ressursId)
    {
        RessursId = ressursId;
    }
}
