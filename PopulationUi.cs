using Godot;
using Inlanders.Simulation;
using System.Collections.Generic;

public partial class Game
{
    private GridContainer _rosterContainer = null!;
    private VBoxContainer _idleContainer = null!;
    private Button _inviteButton = null!;
    private Label _arrivalInfo = null!;

    private void MakeArrivalControls(VBoxContainer column)
    {
        column.AddChild(Text("WELCOME NEIGHBORS", 12));
        _arrivalInfo = Text("", 14, true); column.AddChild(_arrivalInfo);
        _inviteButton = Button("Invite 2 newcomers", () =>
        {
            if (!_world.InviteNewcomers()) { Notice(_world.InvitationProblem() ?? "Invitation unavailable."); return; }
            RenderActors(0); UpdateHud();
            Notice($"{_world.People[^2].Name} and {_world.People[^1].Name} have arrived at the timber yard. Choose their jobs in People.");
        });
        column.AddChild(_inviteButton);
    }

    private void UpdatePopulationUi()
    {
        void Resize(List<Button> buttons, Node parent, bool wrap)
        {
            while (buttons.Count > _world.Population)
            {
                var button = buttons[^1]; parent.RemoveChild(button); button.QueueFree(); buttons.RemoveAt(buttons.Count - 1);
            }
            while (buttons.Count < _world.Population)
            {
                int id = buttons.Count;
                var button = Button("", () => SelectPerson(id));
                button.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
                button.Alignment = HorizontalAlignment.Left;
                if (wrap) button.AutowrapMode = TextServer.AutowrapMode.WordSmart;
                button.AddThemeFontSizeOverride("font_size", 14);
                parent.AddChild(button); buttons.Add(button);
            }
        }
        Resize(_roster, _rosterContainer, false);
        Resize(_workerLinks, _buildingDetails, false);
        Resize(_idleLinks, _idleContainer, true);
        foreach (var p in _world.People) _roster[p.Id].Text = p.Name;
        string? problem = _world.InvitationProblem();
        _inviteButton.Disabled = problem != null;
        _arrivalInfo.Text = $"{_world.Population} neighbors · {_world.Beds} beds · {_world.SpareBeds} spare\n" +
            (problem ?? $"Ready for two newcomers. {_world.ArrivalFoodRequired} stored food covers two meals after arrival.") +
            "\nOptional. Newcomers start unassigned; food stays in storage for meals.";
        _housing.GetParent<Control>().TooltipText = $"{_world.Housed} housed of {_world.Population} neighbors · {_world.Beds} beds · {_world.SpareBeds} spare";
    }
}
