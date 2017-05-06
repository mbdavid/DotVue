<%@ WebHandler Language="C#" Class="Auth" %>
using System;
using System.Web;

public class Auth : IHttpHandler
{
    public bool IsReusable { get { return false; } }

    public void ProcessRequest(HttpContext context)
    {
        var token = context.Request.Cookies["access_token"];

        if (token == null)
        {
            context.Response.StatusCode = 500;
        }
        else
        {
            context.Response.ContentType = "text/json";
            context.Response.Write("{ \"login\": \"" + token + "\" }");
        }
    }
}