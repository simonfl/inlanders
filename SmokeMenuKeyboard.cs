using Godot;
using Inlanders.Simulation;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async Task CheckMenuKeyboard()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        async Task Frames(){for(int i=0;i<5;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        string FocusKey()=>_menuControls.FirstOrDefault(c=>c.Control==GetViewport().GuiGetFocusOwner()).Key??"";
        async Task Focus(string key)
        {
            for(int i=0;i<=_menuControls.Count && FocusKey()!=key;i++){await Press(Key.Tab);await Frames();}
            Check(FocusKey()==key,"Keyboard cannot reach "+key);
            Check(_menuFocusRing.Visible && _menuFocusRing.Size.Y>0,"Focus not visible for "+key+" ring="+_menuFocusRing.Size+" scroll="+_mainScroll.ScrollVertical+" target="+GetViewport().GuiGetFocusOwner()?.GetGlobalRect()+" view="+_mainScroll.GetGlobalRect());
        }
        async Task Activate(string key){await Focus(key);await Press(Key.Enter);await Frames();}
        async Task ShiftTab()
        {
            Input.ParseInputEvent(new InputEventKey {Keycode=Key.Tab,ShiftPressed=true,Pressed=true});
            Input.ParseInputEvent(new InputEventKey {Keycode=Key.Tab,ShiftPressed=true,Pressed=false});await Frames();
        }
        Check(_mainButtons["Continue"].Disabled && FocusKey()=="Campaign","Initial focus did not skip disabled Continue");
        string initial=_world.SaveJson();await Press(Key.Up);await Frames();Check(FocusKey()=="Quit" && _atMainMenu,"Up activated Quit or missed wrap");
        await Press(Key.Down);await Frames();Check(FocusKey()=="Campaign","Down did not skip disabled Continue");
        await ShiftTab();Check(FocusKey()=="Quit","Shift-Tab order differs");
        await Press(Key.Escape);await Frames();Check(_atMainMenu && _world.SaveJson()==initial,"Root Escape quit or changed village");
        var book=new CampaignBook();foreach(var level in World.CampaignLevels)book.Capture(World.NewCampaign(level.Id));book.SaveFile(_campaignPath);_campaignBook=book;
        foreach(int width in new[]{960,1440})
        {
            GetWindow().Size=new(width,width==960?640:900);ShowMainMenu();await Frames();
            await Activate("Settings");await Focus("Effects");
            float effects=_effectsVolume;Key change=effects<100?Key.Right:Key.Left;
            await Press(change);await Frames();Check(_effectsVolume!=effects && FocusKey()=="Effects","Slider keyboard adjustment failed or moved focus");
            var config=new ConfigFile();Check(config.Load(_audioSettingsPath)==Error.Ok && Math.Abs((double)config.GetValue("audio","effects")-_effectsVolume)<.01,"Keyboard setting not persisted");
            await Press(change==Key.Right?Key.Left:Key.Right);await Frames();Check(_effectsVolume==effects,"Slider restore failed");
            await Press(Key.Down);await Frames();Check(FocusKey()=="Nature","Down changed slider rather than navigating");
            foreach(string channel in new[]{"Nature","Music"})
            {
                await Focus(channel);var slider=(HSlider)GetViewport().GuiGetFocusOwner();double value=slider.Value;
                var step=value<100?Key.Right:Key.Left;await Press(step);await Frames();
                Check(slider.Value!=value,"Keyboard slider failed for "+channel);
                await Press(step==Key.Right?Key.Left:Key.Right);await Frames();Check(slider.Value==value,"Slider restore failed for "+channel);
            }
            await Focus("sound-mute");bool muted=_soundMuted;await Press(Key.Space);await Frames();
            Check(_soundMuted!=muted && FocusKey()=="sound-mute","Space mute or rebuilt-page focus failed");await Press(Key.Enter);await Frames();Check(_soundMuted==muted,"Mute restore failed");
            await Focus("Music");await Capture($"artifacts/menu-keyboard-settings-{width}.png");
            await Press(Key.Escape);await Frames();Check(FocusKey()=="Settings","Back did not restore parent focus");
            await Activate("Campaign");await Focus("Replay level 10");
            Check(_mainScroll.ScrollVertical>0,"Campaign focus did not scroll to later level");
            var focused=GetViewport().GuiGetFocusOwner()!;Check(focused.GetGlobalRect().Position.Y>=_mainScroll.GlobalPosition.Y && focused.GetGlobalRect().End.Y<=_mainScroll.GetGlobalRect().End.Y,"Focused late campaign clipped");
            await Capture($"artifacts/menu-keyboard-campaign-{width}.png");
            GetWindow().Size=new(width==960?1440:960,width==960?900:640);await Frames();
            Check(FocusKey()=="Replay level 10" && focused.GetGlobalRect().End.Y<=_mainScroll.GetGlobalRect().End.Y,"Resize hid focused campaign entry");
            GetWindow().Size=new(width,width==960?640:900);await Frames();
            string campaign=File.ReadAllText(_campaignPath);await Press(Key.Enter);await Frames();
            Check(FocusKey()=="Cancel" && _menuPageTitle=="Replay level 10?","Replay lacks safe initial focus");
            await Press(Key.Down);await Press(Key.Up);await Frames();Check(File.ReadAllText(_campaignPath)==campaign && _atMainMenu,"Navigation activated replay");
            await Capture($"artifacts/menu-keyboard-confirm-{width}.png");
            await Press(Key.Escape);await Frames();Check(FocusKey()=="Replay level 10" && File.ReadAllText(_campaignPath)==campaign,"Cancel lost prior focus or changed campaign");
            await Activate("Resume level 10");Check(!_atMainMenu && _world.Campaign?.Level==10 && _paused,"Keyboard campaign entry failed");
            Check(GetViewport().GuiGetFocusOwner()==null || !_mainColumn.IsAncestorOf(GetViewport().GuiGetFocusOwner()),"Menu kept focus in game");
            await Press(Key.Space);Check(!_paused,"Gameplay Space consumed by old menu");await Press(Key.Space);
            ReturnToMainMenu();await Frames();await Activate("Campaign");await Activate("Replay level 10");await Activate("Replay level 10");
            Check(!_atMainMenu && _campaignBook!.BeforeReplay.ContainsKey(10),"Confirmed keyboard replay failed");
            ReturnToMainMenu();await Frames();await Activate("Free play");
            // An existing slot gives replacement a real cancellation target.
            var original=World.NewScenario();original.SetPath(new(3,0),true);original.SaveFile(_savePath);string saved=File.ReadAllText(_savePath);
            await Activate("New Original clearing");Check(FocusKey()=="Cancel","New village confirmation initially armed replacement");
            await Press(Key.Enter);await Frames();Check(File.ReadAllText(_savePath)==saved && FocusKey()=="New Original clearing","Cancel altered slot or lost focus");
            await Activate("New Original clearing");await Activate("New Original clearing");
            Check(!_atMainMenu && !_world.Creative && File.ReadAllText(_savePath+".before-new")==saved,"Confirmed replacement lost preceding village");
            ReturnToMainMenu();await Frames();await Activate("Creative");
            await Activate("New Original clearing");if(_atMainMenu)await Activate("New Original clearing");
            Check(!_atMainMenu && _world.Creative,"Keyboard Creative start failed");
            ReturnToMainMenu();await Frames();File.WriteAllText(_continuePath,"broken");await Activate("Continue");
            Check(_atMainMenu && _menuMessage.Text.Contains("Could not open") && FocusKey()=="Continue","Error page lost usable focus");
            await Press(Key.Escape);Check(_atMainMenu,"Escape from Continue error quit");
            // Mouse can take over a keyboard-focused page and its parent focus remains useful.
            var button=_mainButtons["Settings"];await Click(button.GetGlobalRect().GetCenter());await Frames();Check(_menuPageTitle=="Sound","Mouse takeover failed");
            await Press(Key.Escape);await Frames();Check(FocusKey()=="Settings","Mouse navigation did not remember parent focus");
            string validCampaign=File.ReadAllText(_campaignPath);File.WriteAllText(_campaignPath,"broken");_campaignBook=null;
            await Activate("Campaign");Check(_menuMessage.Text.Contains("Could not read") && FocusKey()=="Start fresh campaign","Corrupt campaign lacks keyboard recovery");
            await Press(Key.Down);await Frames();Check(FocusKey()=="Back" && File.ReadAllText(_campaignPath)=="broken","Navigation overwrote corrupt campaign");
            await Press(Key.Enter);await Frames();Check(FocusKey()=="Campaign","Error-page Back lost parent focus");
            File.WriteAllText(_campaignPath,validCampaign);_campaignBook=CampaignBook.LoadFile(_campaignPath);
        }
        GD.Print("PASS: keyboard-only menu traversal, disabled/wrapped focus, sliders and persistence, mute, scrolling, Back, replay/replacement cancellation and activation, campaign/Creative entry, game shortcuts and error/mouse recovery at 960/1440.");
    }
}
