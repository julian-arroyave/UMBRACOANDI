using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using umbraco.presentation.umbracobase;

/// <summary>
/// Descripción breve de BaseProxy
/// </summary>
[RestExtension("BaseProxy")]
public class BaseProxy : GenericBase
{
    [RestExtensionMethod(returnXml = false)]
    public static string RenderMacro()
	{
        SetLanguage();
        return WrapInJson<string>(RenderMacroUtil.RenderMacro());
	}

}