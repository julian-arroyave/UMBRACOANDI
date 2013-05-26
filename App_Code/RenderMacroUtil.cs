using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using umbraco.cms.businesslogic.macro;
using System.IO;
using System.Text.RegularExpressions;
using umbraco.MacroEngines;

/// <summary>
/// Descripción breve de RenderMacro
/// </summary>
public class RenderMacroUtil
{
    public static string RenderMacro()
    {
        int pageId = 1063; // default page id to render for

        string queryStringMacroAlias = HttpContext.Current.Request["macroalias"];

        if (queryStringMacroAlias != "")
        {
            string macroAttributeString = "macroalias=\"" + queryStringMacroAlias + "\"";
            string macroTagString = "<?UMBRACO_MACRO " + macroAttributeString + "/>";

            string queryStringPageId = HttpContext.Current.Request["pageid"];
            int.TryParse(queryStringPageId, out pageId);
            Macro m = Macro.GetByAlias(queryStringMacroAlias);

            if (Path.GetExtension(m.Type) == ".ascx")
            {
                macroTagString = umbraco.library.RenderMacroContent(macroTagString, pageId).Trim();
            }
            else if (Path.GetExtension(m.ScriptingFile) == ".cshtml")
            {
                macroTagString = RenderRazorScriptFile(m.ScriptingFile, pageId).Trim();
            }



                return ReplaceUmbracoLinks(RenderMacroInString(macroTagString, pageId));

        }

        return "";
    }

    private static string RenderMacroInString(string input, int nodeId)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;
        var regex = new Regex("<\\?UMBRACO_MACRO[^>]*/>", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

        return regex.Replace(input, p =>
        {
            try
            {
                string alias = p.Value.Substring(p.Value.ToLower().IndexOf("macroalias=\"") + 12);
                alias = alias.Substring(0, alias.IndexOf("\""));

                string nodoPadre = p.Value.Substring(p.Value.ToLower().IndexOf("nodopadre=\"") + 11);
                nodoPadre = nodoPadre.Substring(0, nodoPadre.IndexOf("\""));

                HttpContext.Current.Items.Add("nodoPadre", nodoPadre);

                Macro m = Macro.GetByAlias(alias);

                if (Path.GetExtension(m.Type) == ".ascx")
                {
                    return umbraco.library.RenderMacroContent(p.Value, nodeId).Trim();
                }
                else if (Path.GetExtension(m.ScriptingFile) == ".cshtml")
                {
                    return RenderRazorScriptFile(m.ScriptingFile, nodeId).Trim();
                }

                return "";
            }
            catch (Exception ex)
            {
                return "Problems Rendering Macro";
            }
        });
    }

    private static string Render(string razorScript = "", string macroScriptFileName = "", int nodeId = 0, Dictionary<string, string> macroParameters = null)
    {
        var macroEngine = new umbraco.MacroEngines.RazorMacroEngine();
        var macro = new MacroModel();

        macro.ScriptCode = razorScript;
        macro.ScriptLanguage = "cshtml";
        macro.ScriptName = macroScriptFileName;

        if (macroParameters != null)
        {
            foreach (var param in macroParameters)
            {
                macro.Properties.Add(new MacroPropertyModel(param.Key, param.Value));
            }
        }

        return macroEngine.Execute(macro, new umbraco.NodeFactory.Node(nodeId));
    }

    private static string RenderRazorScriptString(string razorScript, int nodeId, Dictionary<string, string> macroParameters = null)
    {
        return Render(razorScript, "", nodeId, macroParameters);
    }

    private static string RenderRazorScriptFile(string macroScriptFileName, int nodeId, Dictionary<string, string> macroParameters = null)
    {
        return Render("", macroScriptFileName, nodeId, macroParameters);
    }

    private static string ReplaceUmbracoLinks(string macroTagString)
    {
        var regex = new Regex("{localLink:[0-9]+}", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
        var matches = regex.Matches(macroTagString);

        foreach (Match match in matches)
        {

            var regexNum = new Regex(@"\d+");
            var matchesNum = regexNum.Matches(match.ToString());

            if (matchesNum.Count > 0)
            {
                DynamicNode node = new DynamicNode(matchesNum[0].Value);
                string url = node.Url;
                url = url.Remove(0, 1);
                macroTagString = macroTagString.Replace(match.ToString(), url);
            }
        }

        return macroTagString;
    }
}