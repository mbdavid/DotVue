using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using DotVue;

public interface IEmail
{
    Guid Send(string to, string subject, string body);
}

public class Email : IEmail
{
    public Guid Send(string to, string subject, string body)
    {
        var id = Guid.NewGuid();

        Debug.Print($"Send Fake Email to: {to} ({id})");

        return id;
    }
}
