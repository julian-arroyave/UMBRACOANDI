using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using umbraco;
using System.Xml.XPath;
using System.Text.RegularExpressions;

[XsltExtension]
public class MediaHelper
{
    /// <summary>
    /// Recupera la ruta de la imagen usando library.GetMedia: primera consulta a bbdd y segunda cacheada.
    /// </summary>
    /// <param name="id">id del nodo de media</param>
    /// <returns></returns>
    public static string GetFile(string id)
    {
        string ruta = string.Empty;
        try
        {
            XPathNodeIterator xmlRet = library.GetMedia(Convert.ToInt32(id), false);
            string strRet = xmlRet.Current.InnerXml.ToString();
            var regex = new Regex(@"(?<=<umbracoFile>).*?(?=</umbracoFile>)");
            var matches = regex.Matches(strRet);
            ruta = matches[0].Value;
        }
        catch (Exception ex)
        {
            
        }

        return ruta;
    }




}