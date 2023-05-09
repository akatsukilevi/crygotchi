namespace Crygotchi;

public partial class FarmingSlot : Resource
{
    public float GrowthAmount = 0f;
    public bool IsDry = true;

    public void Update(Node3D DirtDry, Node3D DirtWet)
    {
        DirtDry.Visible = this.IsDry;
        DirtWet.Visible = !this.IsDry;
    }
}
