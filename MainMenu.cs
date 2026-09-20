using Godot;
using Inlanders.Simulation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public partial class Game
{
    private bool _atMainMenu, _menuEnabled;
    private string _continuePath = "saves/continue.json";
    private Control _mainMenu = null!;
    private PanelContainer _mainPanel = null!;
    private VBoxContainer _mainColumn = null!;
    private ScrollContainer _mainScroll = null!;
    private Label _menuMessage = null!;
    private readonly Dictionary<string, Button> _mainButtons = new();
    private void MakeMainMenu()
    {
        _menuEnabled = true; GetTree().AutoAcceptQuit = false;
        var layer = new CanvasLayer { Layer = 20 }; AddChild(layer);
        _mainMenu = new Control(); layer.AddChild(_mainMenu); _mainMenu.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
        MakeMenuArt();
        _mainPanel = HudPanel(_mainMenu); _mainPanel.Theme = HudTheme();
        _mainScroll = new ScrollContainer { HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled }; _mainPanel.AddChild(_mainScroll);
        _mainColumn = new VBoxContainer { SizeFlagsHorizontal = Control.SizeFlags.ExpandFill }; _mainColumn.AddThemeConstantOverride("separation", 12); _mainScroll.AddChild(_mainColumn);
        _menuFocusRing=new Panel { MouseFilter=Control.MouseFilterEnum.Ignore,Visible=false };
        var focusStyle=new StyleBoxFlat { BgColor=new(0,0,0,0),BorderColor=new("efd08c"),BorderWidthLeft=2,BorderWidthRight=2,BorderWidthTop=2,BorderWidthBottom=2,CornerRadiusTopLeft=6,CornerRadiusTopRight=6,CornerRadiusBottomLeft=6,CornerRadiusBottomRight=6 };
        _menuFocusRing.AddThemeStyleboxOverride("panel",focusStyle);_mainMenu.AddChild(_menuFocusRing);
        _mainMenu.Resized += LayoutMainMenu;
        LayoutMainMenu(); ShowMainMenu();
    }
    private void LayoutMainMenu()
    {
        _mainPanel.Position = new(40, 40); _mainPanel.Size = new(400, Math.Max(400, _mainMenu.Size.Y - 80));
        LayoutMenuArt();
        var focus=GetViewport().GuiGetFocusOwner();
        if(_atMainMenu && focus!=null && _mainColumn.IsAncestorOf(focus))RevealMenuFocusAfterLayout(focus,_menuPageRevision);
    }
    private void MenuPage(string title)
    {
        _menuControls.Clear();_menuBack=null;_menuPageTitle=title;
        int revision=++_menuPageRevision;Callable.From(()=>FocusMenuPage(revision)).CallDeferred();
        foreach (var child in _mainColumn.GetChildren()) { _mainColumn.RemoveChild(child); child.QueueFree(); }
        _mainButtons.Clear(); _mainColumn.AddChild(Text(PublicIdentity.Title, 32, true)); _mainColumn.AddChild(Text(title, 20, true));
        _mainScroll.ScrollVertical = 0;
        _menuMessage = Text("", 14, true); _mainColumn.AddChild(_menuMessage);
    }
    private Button MenuButton(string label, Action action,string? focusKey=null)
    {
        var button = Button(label, () => { TraceMenuClick("activated:"+label); action(); }); button.CustomMinimumSize = new(0, 44); _mainColumn.AddChild(button); _mainButtons[label] = button;
        RegisterMenuControl(button,focusKey??label);if(label is "Back" or "Cancel")_menuBack=action;return button;
    }
    private bool DeveloperPlaces=>_reviewRequest!=null || OS.GetCmdlineUserArgs().Any(a=>a=="--developer" || a.EndsWith("smoke-test"));
    private bool CanContinuePlace(World world,bool developer)=>developer || world.PublicPlace!=null;
    private void ShowMainMenu()
    {
        _atMainMenu = true; _paused = true; _placing = false; _pathStroke = false;
        CloseManagementUi(); RefreshGhost(); _hud.Hide(); _mainMenu.Show();
        MenuPage(PublicIdentity.Tagline);
        MenuButton("Continue", ContinueFromMenu).Disabled = !File.Exists(_continuePath);
        MenuButton("Play", TransformationMenu);
        if(DeveloperPlaces)
            MenuButton("Earlier prototypes", ComparisonMenu);
        MenuButton("Settings", MainSettings);
        MenuButton("Quit", RequestQuit);
        _mainColumn.AddChild(Text("New France, 17th century. Make a home by the river, live from the land, and build a life worth staying for.", 15, true));
    }
    private void RememberSettlement()
    {
        if (_menuEnabled) _world.SaveFile(_continuePath);
    }
    private void ReturnToMainMenu()
    {
        if (SaveSession()) ShowMainMenu();
    }
    private void EnterFromMenu(World world)
    {
        // Persist Continue before leaving the menu, so a write failure keeps the menu usable.
        var book = world.Campaign != null ? ReadCampaignBook() : null;
        world.SaveFile(_continuePath);
        if (book != null) { book.Capture(world); _campaignBook = book; }
        AdoptWorld(world);
        GetViewport().GuiGetFocusOwner()?.ReleaseFocus();
        _atMainMenu = false; _mainMenu.Hide(); _hud.Show(); _paused = true;
        if (world.Founding!=null || world.Campaign != null || world.Neighborhood!=null && !world.IsArrangementCourt) ToggleDrawer(2);
        if(world.Founding?.TransformationHamlet==true){CloseDrawer();_focus=OnGround(0,4);_camera.Size=29;UpdateCamera();Notice("A working hamlet. Follow the garden paths, inspect a place, and choose what you would change. Space starts daily life.");}
        else if(world.CourtStudy is {} study){_focus=OnGround(3,3);_camera.Size=CourtZoom(29);UpdateCamera();Notice(study.Finite?"Open a place to gather. Choose Your place [G] for the brief and starting layout. Space plays village life.":"Sixteen neighbors, a place of your own. Your place [G] shows the starting layout. Space plays village life.");}
        else if(world.IsArrangementCourt){_focus=OnGround(3,3);_camera.Size=CourtZoom(29);UpdateCamera();Notice(world.Creative?"Make a place of your own: free building, moves and removal. Real meals without hunger penalties. Welcoming is optional in Goals. Press Space to play.":"Choose a resident or home to follow daily life. Try one building elsewhere; restore it from the resident card. Welcoming is optional in Goals. Press Space to play.");}
        else Notice("Settlement ready and paused. Press Space to play.");
    }
    private void MenuAttempt(Action action)
    {
        try { action(); } catch (Exception e) { _menuMessage.Text = "Could not open settlement: " + e.Message; TraceMenuClick("open-error",error:e.ToString()); }
    }
    private void ContinueFromMenu() => MenuAttempt(() =>
    {
        var world = World.LoadFile(_continuePath);
        if(!CanContinuePlace(world,DeveloperPlaces))throw new InvalidOperationException("This save belongs to an earlier experiment. Choose Play for the current village, or launch with --developer to revisit archived places.");
        EnterFromMenu(world);
    });
    private void ComparisonMenu()
    {
        MenuPage("Developer · archived experiments");
        MenuButton("Earlier farmstead",RiverFarmsteadMenu);
        MenuButton("Free arrangement",()=>CourtStartMenu(false));
        MenuButton("Found a village · A home by the water",FoundingMenu);
        MenuButton("Short introduction · A place to gather",()=>CourtStartMenu(true));
        _mainColumn.AddChild(Text("Archived settlement experiments and earlier rule sets. Play opens the current inhabited hamlet in Normal or relaxed mode.",15,true));
        MenuButton("Earlier settlements", NeighborhoodMenu);
        MenuButton("Between wood and water · prototype",TransformationMenu);
        MenuButton("Earlier free court", CreativeCourtMenu);
        MenuButton("Campaign", CampaignMenu);
        MenuButton("Free play", FreePlayMenu);
        MenuButton("Legacy Creative",()=>FreePlayMenu(true));
        MenuButton("Original neighborhood control",()=>StartNeighborhood(World.NewNeighborhoodExperiment()));
        MenuButton("Try landing and meadow",()=>StartNeighborhood(World.NewNeighborhoodLandscapeExperiment()));
        MenuButton("Play original river level",()=>OpenMenuCampaign(6,false));
        MenuButton("Back",ShowMainMenu);
    }
    private void CampaignMenu()
    {
        MenuPage("Campaign");
        try
        {
            var book = ReadCampaignBook();
            foreach (var level in World.CampaignLevels)
            {
                int id = level.Id;
                _mainColumn.AddChild(Text($"{id}. {level.Title}" + (book.Completed.Contains(id) ? " · Complete" : ""), 17, true));
                MenuButton(book.Settlements.ContainsKey(id) ? $"Resume level {id}" : $"Start level {id}", () => OpenMenuCampaign(id, false));
                if (book.Settlements.ContainsKey(id)) MenuButton($"Replay level {id}", () => ConfirmMenu($"Replay level {id}","Start this level again? The preceding village remains recoverable from Goals.",()=>OpenMenuCampaign(id,true),CampaignMenu));
            }
            _mainColumn.AddChild(Text("Replay retains the preceding village, recoverable from Goals. All buildings remain available.", 14, true));
        }
        catch (Exception e)
        {
            _menuMessage.Text = "Could not read campaign: " + e.Message + " Start a fresh campaign to use the current levels.";
            MenuButton("Start fresh campaign", () => MenuAttempt(() =>
            {
                var book = new CampaignBook(); book.Capture(World.NewCampaign(1));
                book.SaveFile(_campaignPath); _campaignBook = book; CampaignMenu();
            }));
        }
        MenuButton("Back", ShowMainMenu);
    }
    private void OpenMenuCampaign(int level, bool replay) => MenuAttempt(() =>
    {
        var book = ReadCampaignBook();
        var world = !replay && book.Settlements.TryGetValue(level, out var saved) ? World.LoadJson(saved) : World.NewCampaign(level);
        if (replay && book.Settlements.TryGetValue(level, out var previous)) book.BeforeReplay[level] = previous;
        book.Capture(world); book.SaveFile(_campaignPath); _campaignBook = book; EnterFromMenu(world);
    });
    private void FreePlayMenu() => FreePlayMenu(false);
    private void FreePlayMenu(bool creative)
    {
        MenuPage(creative ? "Creative · arrange and watch" : "Free play");
        if(creative)_mainColumn.AddChild(Text("Separate free-building sandbox: instant construction and no hunger. This uses earlier village rules, not the local-food settlement journey. Arrange freely; there is no welcome objective or recurring commons.",15,true));
        foreach (bool large in new[] { false, true })
        {
            string name = large ? "Three clearings" : "Original clearing", path = SandboxSavePath(large, creative);
            _mainColumn.AddChild(Text(name, 18));
            if (File.Exists(path)) MenuButton("Resume " + name, () => MenuAttempt(() => EnterFromMenu(World.LoadFile(path))));
            void StartNew()=>MenuAttempt(() =>
            {
                var world = creative ? World.NewCreative(large) : large ? World.NewLargeMap() : World.NewScenario();
                if (File.Exists(path)) File.Copy(path, path + ".before-new", true);
                world.SaveFile(path); EnterFromMenu(world);
            });
            MenuButton("New " + name, () => { if(File.Exists(path)) ConfirmMenu("New "+name,"Start a new village? The preceding village will be kept under Restore previous.",StartNew,()=>FreePlayMenu(creative));else StartNew(); });
            if (File.Exists(path + ".before-new")) MenuButton("Restore previous " + name, () => MenuAttempt(() =>
            {
                var world = World.LoadFile(path + ".before-new");
                if (File.Exists(path)) File.Copy(path, path + ".before-restore", true);
                world.SaveFile(path); EnterFromMenu(world);
            }));
        }
        _mainColumn.AddChild(Text(creative ? "Instant free buildings, no hunger, all decorations. Remove completed buildings to rearrange; stored goods return to the yard. Starting anew keeps the previous village recoverable." : "The supper goal is optional. Starting anew retains the preceding village; use Restore previous to return to it.", 14, true));
        MenuButton("Back", ShowMainMenu);
    }
    private void MainSettings()
    {
        MenuPage("Sound");
        var mute = MenuButton(_soundMuted ? "Unmute sound" : "Mute sound", () => { ToggleSoundMute(); MainSettings(); },"sound-mute");
        void Slider(string title, float value, Action<float> set)
        {
            _mainColumn.AddChild(Text(title, 16));
            var slider = new HSlider { MinValue = 0, MaxValue = 100, Step = 5, Value = value, CustomMinimumSize = new(0, 36) };
            slider.ValueChanged += n => { set((float)n); AudioVolumeChanged(); };
            slider.DragEnded += changed => { if (changed) SaveAudioSettings(); }; _mainColumn.AddChild(slider);
            RegisterMenuControl(slider,title);
        }
        Slider("Effects", _effectsVolume, n => { _effectsVolume = n; _effectsSlider.SetValueNoSignal(n); });
        Slider("Nature", _ambienceVolume, n => { _ambienceVolume = n; _ambienceSlider.SetValueNoSignal(n); });
        Slider("Music", _musicVolume, n => { _musicVolume = n; _musicSlider.SetValueNoSignal(n); });
        MenuButton(_musicMuted ? "Unmute music" : "Mute music", () => { ToggleMusicMute(); MainSettings(); },"music-mute");
        MenuButton("Back", () => { SaveAudioSettings(); ShowMainMenu(); });
    }
}
