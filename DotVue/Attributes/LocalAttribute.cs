using System;

namespace DotVue
{
	/// <summary>
	/// Define C# class field as an local parameter - defines as client only data, do not send from client to server.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
    public class LocalAttribute : Attribute
    {
    }
}
