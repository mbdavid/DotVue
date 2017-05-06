using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotVue;

/// <summary>
/// Show progress indicator from NProgress component
/// </summary>
public class NProgressAttribute : ScriptAttribute
{
    public NProgressAttribute()
        : base("NProgress.start();", "NProgress.done();")
    {
    }
}

/// <summary>
/// Remove class from root element after run this server script
/// </summary>
public class ReadyAttribute : ScriptAttribute
{
    public ReadyAttribute()
        : this("hidden")
    {
    }

    public ReadyAttribute(string className)
        : base(null, "this.$el.classList.remove('" + className + "');")
    {
    }
}


/// <summary>
/// Set loading state in refButton (only for Keen UI component)
/// </summary>
public class LoadingAttribute : ScriptAttribute
{
    public LoadingAttribute(string refButton)
        : base("this.$refs." + refButton + ".disabled = true", "this.$refs." + refButton + ".disabled = false")
    {
    }
}
