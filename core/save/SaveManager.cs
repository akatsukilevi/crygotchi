using Godot.Collections;

namespace AfterlifeAdventures;

public partial class SaveManager : Node
{
    private static string SavePath = "user://ld_save.tres";

    private SaveGame _save = new SaveGame();

    public SaveManager() : base() { }

    public override void _Ready()
    {
        base._Ready();
        this._save.Initialize(this);
        this.LoadSavegame();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        //* If press F11, load
        if (Input.IsActionJustPressed("debug_load")) this.LoadSavegame();
        if (Input.IsActionJustPressed("debug_save")) this.WriteSavegame();
    }

    public SaveGame GetSave()
    {
        return this._save;
    }

    public void LoadSavegame()
    {
        if (!FileAccess.FileExists(SaveManager.SavePath))
        {
            GD.PushWarning("No save file found");
            return;
        }

        var saveFile = FileAccess.Open(SaveManager.SavePath, FileAccess.ModeFlags.Read);
        this._save.DeserializeSave((Dictionary<string, Variant>)Json.ParseString(saveFile.GetAsText()));
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
