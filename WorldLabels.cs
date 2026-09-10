using Godot;

public partial class Game
{
    private bool _showWorldLabels = true;
    private Button _worldLabelsButton = null!, _watchLabelsButton = null!;
    private bool EditingText => GetViewport().GuiGetFocusOwner() is LineEdit or TextEdit;

    private void RegisterWorldLabel(Node node)
    {
        if (node is not Label3D label || label.GetViewport() != GetViewport() || (_ghost != null && _ghost.IsAncestorOf(label))) return;
        label.AddToGroup("world_labels"); label.Visible = _showWorldLabels;
    }
    private void ToggleWorldLabels()
    {
        _showWorldLabels = !_showWorldLabels;
        foreach (Node3D label in GetTree().GetNodesInGroup("world_labels"))
            if (GodotObject.IsInstanceValid(label) && !label.IsQueuedForDeletion())
                label.Visible = _showWorldLabels && (_ghostModel == null || !_ghostModel.IsAncestorOf(label));
        UpdateLabelButtons(); SaveAtmosphere();
    }
    private void UpdateLabelButtons()
    {
        if (_worldLabelsButton != null) _worldLabelsButton.Text = _showWorldLabels ? "World labels: shown" : "World labels: hidden";
        if (_watchLabelsButton != null) _watchLabelsButton.Text = _showWorldLabels ? "Labels: on" : "Labels: off";
    }
}
