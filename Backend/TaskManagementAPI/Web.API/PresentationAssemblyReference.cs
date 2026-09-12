using System.Reflection;

namespace Web.API;

public class PresentationAssemblyReference
{
    internal static readonly Assembly assembly = typeof(PresentationAssemblyReference).Assembly;
}
