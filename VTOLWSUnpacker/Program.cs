using System;
using System.IO;
using System.Linq;
using System.Reflection;


namespace VTUnpacker
{
    class Program
    {
        static readonly string  supportedPacketFiles = ".vtmb.vtsb.vtcb.pngb.jpgb";
        static void Main(string[] args)
        {
            Assembly assem = Assembly.GetExecutingAssembly();
            AssemblyName assemName = assem.GetName();
            Version ver = assemName.Version;
            Console.WriteLine("VTOL Workshop Unpacker\nversion {0}", ver.ToString());
            Console.WriteLine("----------------------\n");
            if(args.Length < 1)
            {
                Console.WriteLine("Error: You must specify a folder or file to unpack.\n");
                Console.WriteLine("Examples:\n\n./VTOLWSUnpacker .  \t\t\t\tUse current directory");
                Console.WriteLine("./VTOLWSUnpacker c:\\exampleDir\\scenario.vtsb \tUnpack single file");
                Console.WriteLine("./VTOLWSUnpacker c:\\exampleDir  \t\tUnpack all files in Directory and subdirectories");
                Console.WriteLine("./VTOLWSUnpacker \"c:\\example Dir\\\" \t\tUnpack specify directory with spaces\n");
                Console.ReadLine();

            }
            else
            {
               Console.WriteLine("Using {0} as path", args[0]);
               UnPackFilesRead(args[0]);
               Console.WriteLine("----------------------\n");

            }
           
        }

        static void UnPackFilesRead(string path, bool removeOriginal = false)
        {
            if (File.Exists(path) && supportedPacketFiles.Contains(Path.GetExtension(path).ToLower()))
            {
                UnPackFles(path, removeOriginal);
                Console.WriteLine("File {0} unpacked.", path);
            }
            else if (Directory.Exists(path))
            {
                string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
                                  .Where(s => s.EndsWith("b") && supportedPacketFiles.Contains(Path.GetExtension(s))).ToArray();
                Console.WriteLine("Found {0} files to unpack.", files.Length);
                foreach (string f in files)
                {
                    UnPackFilesRead(f.ToLower(), removeOriginal);
                }
                
            }
            else
            {
                Console.WriteLine("File or directory {0} not found", path);
            }

        }


        static void UnPackFles(string filePath, bool removeOriginal = true)
        {
            switch (Path.GetExtension(filePath))
            {
                case ".vtmb":
                case ".vtsb":
                case ".vtcb":
                    CNode unpacket = CNode.ReadWorkshopConfig(filePath, removeOriginal);
                    unpacket.SaveToFile(filePath[0..^1]);
                    break;
                case ".pngb":
                case ".jpgb":
                    CNode.WorkshopDecode(filePath, removeOriginal);
                    break;
            }
        }
    }
}
