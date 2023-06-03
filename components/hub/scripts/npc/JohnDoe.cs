using System.Threading.Tasks;

namespace Crygotchi;

public partial class JohnDoe : NPC
{
    protected override async Task StartInteraction()
    {
        GD.Print("[ JD ] Interaction has started! Will wait 5 seconds then return");

        await Task.Delay(5000);

        GD.Print("[ JD ] Finishing interaction!");
        await this.FinishInteraction();
    }

    protected override async Task FinishInteraction()
    {
        await base.FinishInteraction();
    }
}
