using System;
using System.IO;
using System.Threading;
namespace Inlanders.Simulation;
internal static class AtomicSave
{
    public static void Write(string path,string json)
    {
        string full=Path.GetFullPath(path);Directory.CreateDirectory(Path.GetDirectoryName(full)!);
        string temporary=full+"."+Guid.NewGuid().ToString("N")+".tmp";
        try
        {
            File.WriteAllText(temporary,json);
            for(int attempt=0;;attempt++)
            {
                try { File.Move(temporary,full,true);return; }
                catch(Exception e) when(attempt<10 && (e is IOException or UnauthorizedAccessException) && ((e.HResult & 0xffff) is 5 or 32 or 33 or 1175 or 1176))
                {Thread.Sleep(40);}
            }
        }
        finally {if(File.Exists(temporary))File.Delete(temporary);}
    }
}


