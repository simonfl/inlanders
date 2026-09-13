using Inlanders.Simulation;
static class AtomicSaveChecks
{
    public static void Run()
    {
        string dir=Path.Combine(Path.GetTempPath(),"inlanders-atomic-"+Guid.NewGuid());Directory.CreateDirectory(dir);string path=Path.Combine(dir,"village.json");
        try
        {
            var world=World.NewScenario();world.SaveFile(path);string first=File.ReadAllText(path);world.Tick(.1f);
            using(var held=new FileStream(path,FileMode.Open,FileAccess.Read,FileShare.Read))
            {
                var release=Task.Run(()=>{Thread.Sleep(120);held.Dispose();});world.SaveFile(path);release.Wait();
            }
            if(World.LoadFile(path).SaveJson()!=world.SaveJson())throw new Exception("Temporary lock lost save");
            string before=File.ReadAllText(path);world.Tick(.1f);bool failed=false;
            using(var held=new FileStream(path,FileMode.Open,FileAccess.Read,FileShare.Read))
                try{world.SaveFile(path);}catch(Exception e) when(e is IOException or UnauthorizedAccessException){failed=true;}
            if(!failed || File.ReadAllText(path)!=before || Directory.GetFiles(dir,"*.tmp").Length!=0)throw new Exception("Persistent lock changed committed save or leaked temporary file");
            for(int i=0;i<40;i++){world.Tick(.1f);world.SaveFile(path);if(World.LoadFile(path).SaveJson()!=world.SaveJson())throw new Exception("Repeated replacement mismatch");}
            Console.WriteLine("PASS: temporary lock recovery, bounded permanent lock failure, intact previous save and 40 exact replacements");
        }
        finally{foreach(string file in Directory.GetFiles(dir))File.Delete(file);Directory.Delete(dir);}
    }
}

