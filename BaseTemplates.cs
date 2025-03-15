namespace UnityScriptTemplateModifierService
{
    public static class BaseTemplates
    {
        public static readonly string BaseMonoBehaviourContent = "using UnityEngine;\r\n\r\n    #ROOTNAMESPACEBEGIN#\r\npublic class #SCRIPTNAME# : MonoBehaviour\r\n{\r\n    private void Start()\r\n    {\r\n        #NOTRIM#\r\n    }\r\n}\r\n#ROOTNAMESPACEEND#\r\n";
    }
}
