using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async void RunSurveySmoke()
    {
        try { await CheckResourceSurveyUi(); GetTree().Quit(); }
        catch(Exception e) { GD.PrintErr("SURVEY SMOKE FAIL: "+e); GetTree().Quit(1); }
    }
    private async Task CheckResourceSurveyUi()
    {
        var previous=_world; var size=GetWindow().Size; bool labels=_showWorldLabels;
        void Check(bool ok,string why) { if(!ok) throw new Exception(why); }
        async Task Frames(int count=5) { for(int i=0;i<count;i++) await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame); }
        try
        {
            foreach(bool lake in new[]{false,true})
            {
                var w=lake?World.NewLakeMap():World.NewCreative(true);
                if(lake) w.Place(new(3,4),true,BuildingKind.FishingDock);
                else w.Place(new(-9,2),false,BuildingKind.Quarry);
                AdoptWorld(w); _paused=true; _noticeUntil=0; await Frames();
                string saved=w.SaveJson();
                BeginPlacement(BuildingKind.Cottage); await Press(Key.U); await Frames();
                Check(_surveying && !_placing && _surveyDetails.Visible,"Survey failed to replace placement");
                foreach(int width in new[]{1440,960})
                {
                    GetWindow().Size=new(width,width==960?640:900); CloseDrawer(); await Frames();
                    foreach(var source in w.ResourceSources())
                    {
                        _focus=OnGround(source.Cell.X,source.Cell.Z); _camera.Size=17; UpdateCamera(); await Frames();
                        var marker=_sourceButtons[source.Key]; Check(marker.Visible,"Source marker covered at centered camera");
                        await UiClick(marker); await Frames();
                        Check(_selectedSource==source.Key && _selectedSite<0 && _selectedPerson<0 && _surveyDetails.Visible,"Marker click leaked to village selection");
                        Check(_sourceInfo.Text.Contains("available") && _sourceInfo.Size.X<=_inspector.Size.X,"Source report missing or overflowing");
                        _inspectionScroll.ScrollVertical=0; await Frames(); await Capture($"artifacts/f21j-{source.Key.Kind}-{source.Key.Id}-{width}.png");
                        if(_sourceWorkplaceLinks.Count>0)
                        {
                            int target=_sourceWorkplaceLinks.Keys.First(); var link=_sourceWorkplaceLinks[target];
                            _inspectionScroll.EnsureControlVisible(link); await Frames(); await UiClick(link); await Frames();
                            Check(_selectedSite==target && !_surveyDetails.Visible,"Related workplace link failed");
                            _inspectionScroll.EnsureControlVisible(_surveyBack); await Frames(); await UiClick(_surveyBack); await Frames();
                            Check(_selectedSource==source.Key && _focus.X==source.Cell.X && _focus.Z==source.Cell.Z,"Return to source failed");
                        }
                        Check(saved==w.SaveJson(),"Survey navigation changed village");
                    }
                }
                if(_showWorldLabels) ToggleWorldLabels(); await Frames();
                Check(_sourceButtons.Values.Any(b=>b.Visible),"World-label preference hid survey controls");
                int nodes=GetTree().GetNodeCount(); await Frames(90);
                Check(nodes==GetTree().GetNodeCount() && saved==w.SaveJson(),"Idle survey grew nodes or changed save");
                await Press(Key.H); await Frames(); Check(_watching && !_surveying && !_sourceMarkers.Visible,"Watch retained survey");
                await Press(Key.U); await Frames(); Check(!_watching && _surveying,"Survey shortcut did not leave Watch");
                await Press(Key.Escape); await Frames(); Check(!_surveying && !_sourceMarkers.Visible,"Esc did not finish survey");
                OpenEconomy(); await Frames(); _drawerPages[4].EnsureControlVisible(_surveyToggle); await Frames(); await UiClick(_surveyToggle); await Frames();
                Check(_surveying,"Economy survey entry failed");
                BeginPlacement(BuildingKind.Cottage); await Frames(); Check(!_surveying && _placing,"Placement retained survey picking");
                await Press(Key.U); await Frames();
            }
            AdoptWorld(new World()); _paused=true; await Frames();
            Check(!_surveying && !_sourceMarkers.Visible,"World switch retained survey");
            await Press(Key.U); await Frames();
            Check(_sourceButtons.Count==0 && _sourceInfo.Text.Contains("No fish grounds"),"Empty map retains stale sources");
            GD.Print("PASS: map source picking, current details/workplace links, 960/1440 layouts, label preference, unchanged saves/nodes, placement/Watch/Esc and empty-map reset.");
        }
        finally { if(_showWorldLabels!=labels) ToggleWorldLabels(); GetWindow().Size=size; AdoptWorld(previous); _paused=true; }
    }
}
