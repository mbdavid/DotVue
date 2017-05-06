using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotVue;

public static class JsExtensions
{
    public static JavascriptBuilder CloseModal(this JavascriptBuilder js, string refModal)
    {
        return js.Code("this.$refs.{0}.close();", refModal);
    }

    public static JavascriptBuilder OpenModal(this JavascriptBuilder js, string refModal)
    {
        return js.Code("this.$refs.{0}.open();", refModal);
    }

    public static JavascriptBuilder Reload(this JavascriptBuilder js)
    {
        return js.Code("location.reload();");
    }
}