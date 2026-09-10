using Godot;
using Inlanders.Simulation;
using System;
using System.Threading.Tasks;

public partial class Game
{
    private async Task CheckVisitorUi()
    {
        var previous=_world; var size=GetWindow().Size;
        async Task Frames() { for(int i=0;i<4;i++) await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame); }
        try
        {
            AdoptWorld(World.NewCampaign(2)); _paused=true;
            for(int i=0;i<1201;i++) _world.Tick(.1f);
            await Frames();
            if(!_visitorStand.Visible || !_menuButtons[2].Text.Contains("Visitor")) throw new Exception("Visitor undiscoverable");
            foreach(var width in new[]{1440,960})
            {
                GetWindow().Size=new(width,width==960?640:900); ToggleDrawer(2);
                await Frames(); _drawerPages[2].EnsureControlVisible(_visitorAccept); await Frames();
                if(!_visitorPanel.Visible || !_visitorText.Text.Contains("food after trade")) throw new Exception("Visitor offer unclear");
                await Capture($"artifacts/f15-offer-{width}.png"); CloseDrawer();
            }
            _world.Campaign!.Complete=true; _completionAnnounced=true; await Frames();
            if(!_menuButtons[2].Text.Contains("Visitor")) throw new Exception("Completed campaign hid visitor");
            _world.Campaign.Complete=false;
            string pending=_world.SaveJson();
            ToggleWatch(); await Frames();
            if(!_watching || _drawer.Visible) throw new Exception("Visitor interrupted watch mode");
            ExitWatch(); await UiClick(_visitorAccept); await Frames();
            if(!_world.SunflowersUnlocked || _visitorStand.Visible || !_visitorPlant.Visible) throw new Exception("Visitor acceptance failed");
            await UiClick(_visitorPlant); await Frames();
            if(!_decorating || _decorationKind!=DecorationKind.Sunflowers) throw new Exception("Reward action failed");
            PlaceCottage(new(5,3));
            if(_world.Decorations.Count!=1) throw new Exception("Sunflower placement failed");
            _placing=false; RefreshGhost(); CloseDrawer(); _focus=new(5,0,3); _camera.Size=12; UpdateCamera();
            await Frames(); await Capture("artifacts/f15-sunflowers.png");
            AdoptWorld(World.LoadJson(pending)); _paused=true; await UiClick(_visitorDecline); await Frames();
            if(_world.Gardener!=VisitorState.Declined || _visitorStand.Visible) throw new Exception("Visitor decline failed");
            _decorationKind=DecorationKind.Sunflowers; _removeDecoration=false;
            if(!DecorationDescription.Contains("declined")) throw new Exception("Declined reward still promised");
            GD.Print("PASS: visitor marker, quiet badge, projected food, 1440/960 offer, Watch isolation, accept/reward planting and decline.");
        }
        finally { GetWindow().Size=size; AdoptWorld(previous); _paused=true; }
    }
}
