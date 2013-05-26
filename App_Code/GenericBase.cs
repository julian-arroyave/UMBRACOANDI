using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Threading;
using System.Globalization;
using umbraco.MacroEngines;

/// <summary>
/// Descripción breve de GenericBase
/// </summary>
public class GenericBase
{
    /// <summary>
    /// Establece el lenguaje en el que se deben devolver los datos obtenidos mediante llamados ajax, basandose en los parámetros suministrados en los llamados o en el idioma que tenga asociada la página que hizo el llamado ajax, si no es posible establecer el lenguaje usando esas dos condiciones, se establece la cultura es-ES por defecto
    /// </summary>
    protected static void SetLanguage()
    {
        string language = "es-ES";
        string culture = HttpContext.Current.Request["idioma"];

        //Si la cultura viene por parametro, se asigna
        if (!string.IsNullOrEmpty(culture))
        {
            language = culture;
        }
        else
        {
            //Si la cultura no viene indicada por parámetro, se obtiene la cultura asociada de la home a la que pertenece la página que hizo el llamado
            //Si el llamado no es ajax no viene la url referer, por lo que quedará con es-ES en caso de que no se especifiqué la cultura por parámetro
            if (HttpContext.Current.Request.UrlReferrer != null)
            {
                string callerUrl = HttpContext.Current.Request.UrlReferrer.LocalPath;

                if (callerUrl.StartsWith("/"))
                {
                    //Se obtiene el nombre de la home, el cual siempre está al principio de la url que hizo el llamado por ejemplo "/es/somos-vueling" o "/en/we-are-vueling"
                    string homeName = callerUrl.Split('/')[1].ToLower();

                    DynamicNode node = new DynamicNode(-1);

                    //Obtiene el nodo correspondiente a la home a la que pertenece la página
                    node = node.GetChildrenAsList.Items[0].GetChildrenAsList.Items.Where(x => x.Name.ToLower() == homeName).First();
                    //Obtiene la cultura asociada al home de la página
                    language = umbraco.library.GetCurrentDomains(node.Id)[0].Language.CultureAlias;
                }
            }
        }

        Thread.CurrentThread.CurrentUICulture = new CultureInfo(language);
    }

    /// <summary>
    /// Envuelve el dato pasado por parametro en una respuesta json determinando si se trata de un llamado jsonp o si se ha enviado como parámetro responseInJson=true
    /// </summary>
    /// <typeparam name="T">Tipo de dato que se va a almacenar en el objeto json</typeparam>
    /// <param name="resp">Dato que se quiere envolver en la respuesta json</param>
    /// <returns>Cadena en formato json generada o simplemente la cadena de entrada si no se trata de un llamado jsonp y en caso de que no se pase el parámetro responseInJson</returns>
    protected static string WrapInJson<T>(T resp)
    {
        JavaScriptSerializer ser = new JavaScriptSerializer();
        var response = new { data = resp };

        string isJson = HttpContext.Current.Request["responseInJson"];
        string callback = HttpContext.Current.Request["callback"];

        if (!string.IsNullOrEmpty(callback))
        {
            return callback + "(" + ser.Serialize(response) + ")";
        }

        if (!string.IsNullOrEmpty(isJson))
        {
            return ser.Serialize(response);
        }

        return resp.ToString();
    }
}