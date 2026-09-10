using System;

namespace Inlanders.Simulation;

public enum VisitorState { NotArrived, Pending, Accepted, Declined }
public sealed partial class World
{
    public const int GardenerPrice = 8;
    public VisitorState Gardener { get; private set; }
    public bool SunflowersUnlocked => Creative || Gardener == VisitorState.Accepted;
    public string SunflowerLockReason => Gardener == VisitorState.Declined ? "The gardener offer was declined in this settlement. Sunflowers are unavailable here." : Gardener == VisitorState.NotArrived ? "A gardener offers sunflowers from day 3 after a forager hut is finished. Look in Goals." : "Trade 8 berries with the visiting gardener in Goals to unlock sunflowers.";
    public int FoodAfterGardenerTrade => Food.EdibleStored - GardenerPrice;
    private void AdvanceVisitor()
    {
        if (Creative || Gardener != VisitorState.NotArrived || Food.Day < 3 || !HasForagerHut || Food.Celebrating) return;
        Gardener = VisitorState.Pending; History.Add("A gardener is visiting with sunflower seeds. See Goals.");
    }
    public string? GardenerTradeProblem() => Gardener != VisitorState.Pending ? "There is no pending gardener offer." :
        Food.Celebrating ? "Trade after the village supper." :
        Food.Berries < GardenerPrice ? $"Store {GardenerPrice} berries to trade." : null;
    public bool AcceptGardener()
    {
        if (GardenerTradeProblem() != null) return false;
        Food.Berries -= GardenerPrice; Food.TradedBerries += GardenerPrice;
        Gardener = VisitorState.Accepted;
        History.Add("Traded 8 berries for sunflower seeds. Sunflower decorations are now available.");
        return true;
    }
    public bool DeclineGardener()
    {
        if (Gardener != VisitorState.Pending || Food.Celebrating) return false;
        Gardener = VisitorState.Declined; History.Add("Wished the gardener a pleasant journey.");
        return true;
    }
    private void ValidateVisitor()
    {
        if (!Enum.IsDefined(Gardener) || Food.TradedBerries != (Gardener == VisitorState.Accepted ? GardenerPrice : 0))
            throw new InvalidOperationException("Invalid gardener trade");
    }
}
