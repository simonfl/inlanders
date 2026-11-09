using Godot;
using Inlanders.Simulation;
using System.IO;
using System.Linq;

public partial class Game
{
    private VBoxContainer _recoveryChoices=null!;
    private Button _recoveryCompare=null!;
    private Label _recoveryGapLabel=null!,_recoveryFieldLabel=null!;

    private void CrossingRecoveryMenu()
    {
        MenuPage("The broken crossing");
        _mainColumn.AddChild(Text("Both homes survived the flood. The crossing did not. Four neighbors have eight portions left, while twenty ripe portions wait in their field across the channel.",17,true));
        _mainColumn.AddChild(Text("Repair the crossing and bring the harvest home, or start a garden beside the village. Four logs remain: the garden can start now; the six-log repair needs a little timber work. No new homes are needed.",14,true));
        string path=Path.Combine(Path.GetDirectoryName(_creativeSavePath)!,"broken-crossing.json");
        if(File.Exists(path))MenuButton("Resume recovery",()=>MenuAttempt(()=>EnterFromMenu(World.LoadFile(path))));
        void Start()=>MenuAttempt(()=>{var world=World.NewCrossingRecovery();world.SaveFile(path);EnterFromMenu(world);});
        MenuButton("Begin recovery",()=>{if(File.Exists(path))ConfirmMenu("Begin again?","Replace this recovery village?",Start,CrossingRecoveryMenu);else Start();});
        MenuButton("Back",FirstHomeMenu);
    }

    private void MakeRecoveryUi()
    {
        _recoveryChoices=new();_firstHomeGoals.AddChild(_recoveryChoices);
        _recoveryChoices.AddChild(Text("Repair: reach 20 ripe portions, then keep using the field. Garden: build here now, then wait for the first crop.",14,true));
        _recoveryChoices.AddChild(Button("Repair crossing · 6 logs",()=>{
            _recoveryChoices.Hide();CloseManagementUi();
            var bridge=_world.Cottages.FirstOrDefault(c=>c.Cell==World.RecoveryCrossing && c.Kind==BuildingKind.Bridge) ?? _world.Place(World.RecoveryCrossing,0,BuildingKind.Bridge);
            if(bridge==null){Notice("The crossing site is blocked. Clear the two banks before repairing.");return;}
            _focus=OnGround(0,-3);UpdateCamera();RebuildQueue();UiCue(Cue.Place);
            Notice("Crossing repair ordered. Click the bridge if you want to inspect or cancel it.");
        }));
        _recoveryChoices.AddChild(Button("Plant nearby · 4 logs",()=>{_recoveryChoices.Hide();CloseManagementUi();BeginPlacement(BuildingKind.VegetableGarden);}));
        _recoveryCompare=Button("Compare recovery plans",OpenRecoveryChoices);_firstHomeGoals.AddChild(_recoveryCompare);
        Label Marker(){var label=Text("",13);label.MouseFilter=Control.MouseFilterEnum.Ignore;label.AddThemeColorOverride("font_outline_color",new("182d25"));label.AddThemeConstantOverride("outline_size",5);_hud.AddChild(label);return label;}
        _recoveryGapLabel=Marker();_recoveryFieldLabel=Marker();
        _recoveryChoices.Hide();_recoveryCompare.Hide();_recoveryGapLabel.Hide();_recoveryFieldLabel.Hide();
    }

    private void OpenRecoveryChoices()
    {
        _firstHomeToolsOpen=false;
        if(!_drawer.Visible || _tabs.CurrentTab!=2){ToggleDrawer(2);_recoveryChoices.Show();}
        else _recoveryChoices.Visible=!_recoveryChoices.Visible;
    }

    private void UpdateCrossingRecoveryUi()
    {
        bool recovery=_world.Founding?.FirstHome?.Recovery==true;
        _recoveryCompare.Visible=recovery && !_world.Founding!.FirstHome!.Complete && !_recoveryChoices.Visible;
        if(!recovery)_recoveryChoices.Hide();
        _recoveryGapLabel.Visible=_recoveryFieldLabel.Visible=recovery && !_atMainMenu && !_watching;
        if(!recovery)return;
        var bridge=_world.Cottages.FirstOrDefault(c=>c.Kind==BuildingKind.Bridge && c.Cell==World.RecoveryCrossing);
        _recoveryGapLabel.Text=bridge?.Complete==true?"Crossing restored":bridge!=null?"Crossing under repair":"Washed-out crossing";
        _recoveryGapLabel.Position=_camera.UnprojectPosition(OnGround(0,-3,.5f))+new Vector2(-65,-18);
        var field=_world.Cottages.FirstOrDefault(c=>c.Kind==BuildingKind.VegetableField);
        _recoveryFieldLabel.Visible=field!=null && !_atMainMenu && !_watching;
        if(field!=null){_recoveryFieldLabel.Text=_world.ReadWorkplace(field).MissingAccess?$"Cut-off field · {field.Harvest} ripe portions":"Field reachable";_recoveryFieldLabel.Position=_camera.UnprojectPosition(OnGround(field.Cell.X,field.Cell.Z-2,.8f))+new Vector2(-65,-18);}
    }
}
