using Godot;
public partial class Game
{
    private Button _foodFirst=null!;
    private void MakeWorkFocus(VBoxContainer column)
    {
        _foodFirst=Button("",()=>_world.SetFoodFirst(_world.Founding?.FoodFirst!=true));column.AddChild(_foodFirst);
        _foodFirst.TooltipText="Food first replenishes food before starting expansion jobs. Current jobs, deliveries, meals and breaks finish normally. Balanced work still protects urgent meals. No profession assignments.";
    }
    private void UpdateWorkFocus()
    {
        _foodFirst.Visible=_world.PublicPlace!=null;
        _foodFirst.Text=_world.Founding?.FoodFirst==true?"Work focus: food first · change":"Work focus: balanced · change";
        _foodFirst.Disabled=_world.Food.Celebrating;
    }
}
