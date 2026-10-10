using System;
using System.IO;
public partial class Game
{
    private static string LocalSaveDirectory=>Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData),"Inlanders","saves");
    private void UseSaveDirectory(string directory)
    {
        string At(string name)=>Path.Combine(directory,name);
        _savePath=At("settlement.json");_campaignPath=At("campaign.json");_neighborhoodPath=At("neighborhood.json");
        _continuePath=At("continue.json");_largeSavePath=At("three-clearings.json");
        _creativeSavePath=At("creative.json");_creativeLargeSavePath=At("creative-three-clearings.json");
        _atmospherePath=At("atmosphere.cfg");_audioSettingsPath=At("audio.cfg");
    }
}
