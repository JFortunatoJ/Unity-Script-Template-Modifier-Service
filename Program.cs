using System.ServiceProcess;

namespace UnityScriptTemplateModifierService
{
    static class Program
    {
        static void Main()
        {
            ServiceBase[] ServicesToRun = new ServiceBase[]
            {
                new UnityScriptTemplateModifierService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
