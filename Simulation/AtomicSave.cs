using System;
using System.IO;
using System.Threading;
namespace Inlanders.Simulation;
internal static class AtomicSave
{
    public static void Write(string path,string json)
    {
        string full=Path.GetFullPath(path),temporary=full+"."+Guid.NewGuid().ToString("N")+".tmp";
        string operation="create directory";int replacements=0;Exception? failure=null;
        void Diagnose(Exception e,string stage)
        {
            e.Data["SavePath"]=full;e.Data["SaveOperation"]=stage;e.Data["ReplacementAttempts"]=replacements;
            Console.Error.WriteLine($"Atomic save failed: operation={stage}; target={full}; temporary={temporary}; replacementAttempts={replacements}; HResult=0x{e.HResult:X8}; {e}");
        }
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(full)!);
            operation="write temporary";File.WriteAllText(temporary,json);
            operation="replace target";
            for(int attempt=0;;attempt++)
            {
                replacements=attempt+1;
                try { File.Move(temporary,full,true);return; }
                catch(Exception e) when(attempt<10 && (e is IOException or UnauthorizedAccessException) && ((e.HResult & 0xffff) is 5 or 32 or 33 or 1175 or 1176))
                {Thread.Sleep(40);}
            }
        }
        catch(Exception e){failure=e;Diagnose(e,operation);throw;}
        finally
        {
            try{if(File.Exists(temporary))File.Delete(temporary);}
            catch(Exception e){Diagnose(e,"cleanup temporary");if(failure==null)throw;}
        }
    }
}
