using Godot;
using Inlanders.Simulation;
using System.Collections.Generic;
using System.Linq;
public partial class Game
{
    private sealed class CanopyCutaway
    {
        public float Amount;
        public readonly List<(MeshInstance3D Mesh,StandardMaterial3D Original,StandardMaterial3D Fade)> Pieces=new();
        public CanopyCutaway(Node3D tree)
        {
            void Gather(Node node)
            {
                if(node is MeshInstance3D mesh && mesh.MaterialOverride is StandardMaterial3D material)
                {var faded=(StandardMaterial3D)material.Duplicate();faded.Transparency=BaseMaterial3D.TransparencyEnum.Alpha;Pieces.Add((mesh,material,faded));}
                foreach(var child in node.GetChildren())Gather(child);
            }
            foreach(var crown in tree.GetChildren().OfType<Node3D>().Where(n=>n.IsInGroup("foliage")))Gather(crown);
        }
        public void Update(bool reveal,float dt)
        {
            float next=Mathf.MoveToward(Amount,reveal?1:0,dt*7);if(next==Amount)return;Amount=next;
            foreach(var piece in Pieces)
            {
                if(Amount==0){piece.Mesh.MaterialOverride=piece.Original;continue;}
                var color=piece.Original.AlbedoColor;color.A*=Mathf.Lerp(1,.16f,Amount);piece.Fade.AlbedoColor=color;
                if(piece.Mesh.MaterialOverride!=piece.Fade)piece.Mesh.MaterialOverride=piece.Fade;
            }
        }
    }
    private IEnumerable<Vector3> CanopyFocus()
    {
        if(_atMainMenu)yield break;
        if(_placing && !PointerOverHud(_pointerPosition))yield return OnGround(_hover.X,_hover.Z,.15f);
        int person=_dailyPerson>=0?_dailyPerson:_selectedPerson;
        if(person>=0 && person<_people.Count)yield return PresentedPerson(person)+Vector3.Up*.65f;
        if(_world.Cottages.FirstOrDefault(c=>c.Id==_selectedSite) is {} site)
        {yield return BuildingPosition(site)+Vector3.Up*.65f;yield return OnGround(site.Entrance.X,site.Entrance.Z,.65f);}
        if(_additionSelected && _world.PendingAddition is {} plan)yield return OnGround(plan.Cell.X,plan.Cell.Z,.15f);
    }
    private bool CanopyObscures(TreeView view,Vector3 target)
    {
        var towardCamera=_camera.GlobalBasis.Z.Normalized();var center=view.Top.GlobalPosition+Vector3.Up*(1.95f*view.Top.Scale.Y);
        var offset=center-target;float depth=offset.Dot(towardCamera);float radius=1.12f*view.Top.Scale.Y;
        return depth>0 && (offset-towardCamera*depth).LengthSquared()<radius*radius;
    }
    private void RenderCanopyCutaways(float dt)
    {
        var targets=CanopyFocus().ToArray();
        foreach(var view in _trees.Values)
        {
            bool reveal=view.Top.Visible && targets.Any(t=>CanopyObscures(view,t));
            if(reveal)view.Cutaway??=new(view.Top);
            view.Cutaway?.Update(reveal,dt);
        }
    }
}
