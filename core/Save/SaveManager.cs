namespace Crygotchi;

public partial class SaveManager : Node
{
    private static string SavePath = "user://crygotchi_save.tres";

    private SaveGame _save = new SaveGame();

    public SaveManager() : base()
    {
        // this.LoadSavegame();
    }

    public override void _Ready()
    {
        base._Ready();
        this._save.Initialize(this);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        //* If press F11, load
        // if (Input.IsActionJustPressed("debug_load")) this.LoadSavegame();
        if (Input.IsActionJustPressed("debug_save")) this.WriteSavegame();
    }

    public SaveGame GetSave()
    {
        return this._save;
    }

    public void LoadSavegame()
    {
        var saveGame = GD.Load<SaveGame>(SaveManager.SavePath);
        if (saveGame == null)
        {
            GD.Print("No savegame found, instantiating a new one");
            this._save = new SaveGame();
            return;
        }

        GD.Print("Game loaded from " + SaveManager.SavePath);
        this._save = saveGame;
    }

    public void WriteSavegame()
    {
        this._save.UpdateSave();

        var saveFile = FileAccess.Open(SaveManager.SavePath, FileAccess.ModeFlags.Write);
        saveFile.StoreString(Json.Stringify(this._save.SerializeSave()));

        GD.Print("Wrote save");
        saveFile.Close();
    }
}
