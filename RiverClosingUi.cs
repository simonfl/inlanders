using Godot;
using Inlanders.Simulation;
using System.IO;

public partial class Game
{
    private void RiverClosingMenu()
    {
        MenuPage("Before the river closes");
        _mainColumn.AddChild(Text("Recover an established village, then prepare it for twelve people. Your early food plan must grow with the settlement.",17,true));
        _mainColumn.AddChild(Text("8 min: four settlers and 24 food.\n16 min: four more settlers, no food.\n24 min: the river closes.\nThen sustain all twelve for four continuous minutes: homes, fresh meals, production covering demand, and a one-meal reserve.",15,true));
        _mainColumn.AddChild(Text("About 28–40 minutes on the default schedule. Ready villages can call boats and end the supply season early from Objectives. 3× and 6× remain available. Arrivals happen even if unprepared; shortages are recoverable.",14,true));
        string path=Path.Combine(Path.GetDirectoryName(_creativeSavePath)!,"river-closing.json");
        if(File.Exists(path))MenuButton("Resume this village",()=>MenuAttempt(()=>EnterFromMenu(World.LoadFile(path))));
        void Start()=>MenuAttempt(()=>{var w=World.NewRiverClosing();w.SaveFile(path);EnterFromMenu(w);});
        MenuButton("Begin the longer scenario",()=>{if(File.Exists(path))ConfirmMenu("Begin again?","Replace this river-closing village?",Start,RiverClosingMenu);else Start();});
        MenuButton("Back",FirstHomeMenu);
    }
}
