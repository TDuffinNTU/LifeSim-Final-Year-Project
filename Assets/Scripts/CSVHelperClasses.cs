using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Globalization;
using CsvHelper;

/// <summary>
/// The database that we will build our game around
/// </summary>
public class Database
{
    // databases -- immutable
    private readonly IEnumerable<TRANSLATION_MAP> Translations;
    private readonly IEnumerable<RESPONSE_MAP> Responses;
    private readonly IEnumerable<NPC_DIALOG_MAP> NPCDialog;
    private readonly IEnumerable<NPC_DATA_MAP> NPCData;

    private IEnumerable<FRIENDSHIP_MAP> Friendships;


    // paths to CSVs
    // TODO: move root inside unity
    private const string rootpath = "C:\\Users\\thoma\\OneDrive - Nottingham Trent University\\YEAR FOUR\\FYP\\Databases\\";



    private const string TranslationsPath = rootpath + "TRANSLATIONS.CSV";
    private const string NPCDialogPath = rootpath + "NPCDIALOG.CSV";
    private const string ResponsesPath = rootpath + "RESPONSES.CSV";
    private const string NPCDataPath = rootpath + "NPC_DATA.CSV";
    private const string FurniturePath = rootpath + "FURNITURE.CSV";
    private const string FriendshipsPath = rootpath + "SAVEDATA\\FRIENDSHIPS.CSV";
    private const string SettingsPath = rootpath + "SAVEDATA\\SETTINGS.CSV";
    private const string SaveDataPath = rootpath + "SAVEDATA\\SAVEDATA.CSV";

    

    public Database()
    {
        Translations =  LoadData<TRANSLATION_MAP>(TranslationsPath);
        NPCDialog =       LoadData<NPC_DIALOG_MAP>(NPCDialogPath);
        Responses =     LoadData<RESPONSE_MAP>(ResponsesPath);
        //Friendships = LoadData<FRIENDSHIP_MAP>(PATHS[3]);
        //NPCData = LoadData<NPC_DATA_MAP>(PATHS[4]);
    }

    /// <summary>
    /// Returns dialog data from the database by its PKey
    /// </summary>
    /// <param name="name">The PKey for the NPC_DIALOG table</param>
    /// <returns>NPC_DIALOG_MAP class with data</returns>
    public NPC_DIALOG_MAP DialogLookup(string name) 
    {
        var query = 
            from d in NPCDialog
            where d.Name == name
            select d;

        return (query != null && query.Any()) ? query.First() : null;
    }

    /// <summary>
    /// returns translation string from the central TRANSLATIONS table in the db
    /// </summary>
    /// <param name="name">PKey for TRANSLATIONS table</param>
    /// <param name="code">region code corresponding to table header (ie. "EN", "CY")</param>
    /// <returns>string from TRANSLATIONS table of PKey and region</returns>
    public string TranslationLookup(string name, string code)
    {
        if (name == "" || code == "")
        {
            return "";
        }

        var query =
            from t in Translations
            where t.Name == name

            select new
            {
                translation =
                    code.ToLower() == "en" ? t.EN :
                    code.ToLower() == "cy" ? t.CY :
                    $"INVALID TRANSLATION CODE: {code}"
            };


        return (query != null && query.Any()) ?
            (query.First().translation == "\"\"" ? $"NO TRANSLATION FOR: {name}\tCODE: {code}" : query.First().translation) :
            $"couldnt find";

    }

    /// <summary>
    /// Modify player relationship with NPC of name by delta points
    /// </summary>
    /// <param name="name"></param>
    /// <param name="delta"></param>
    public void ModifyFriendshipPoints (string name, int delta)
    {
        var temp = Friendships.ToList();

        // find friendship
        foreach (var f in temp) 
        {
            if (f.Name == name) 
            {
                f.Points += delta;
                break;
            }
        }

        using (var writer = new StreamWriter(FriendshipsPath))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture)) 
        {
            csv.WriteHeader<FRIENDSHIP_MAP>();
            csv.NextRecord();
            foreach(var f in temp) 
            {
                csv.WriteRecord(f);
                csv.NextRecord();
            }
        } // Using keyword means the stream is flushed (ie written to) once out of scope
    }


    /// <summary>
    /// Type-agnostic loading of data into memory from CSV files
    /// </summary>
    /// <typeparam name="T">Class_Map class to assign data to</typeparam>
    /// <param name="path">Filepath of .csv</param>
    /// <returns>Enumerable of records under table </returns>
    private IEnumerable<T> LoadData<T>(string path)
    {
        using (var reader = new StreamReader(path))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            return csv.GetRecords<T>().ToList();
        }
    }

}

/// <summary>
/// Translations for our game, Name = TKey
/// </summary>
public class TRANSLATION_MAP
{
    public string Name { get; set; }
    public string EN { get; set; }
    public string CY { get; set; }
}


/// <summary>
/// Player responses Tkeys
/// </summary>
public class RESPONSE_MAP
{
    public string Name { get; set; }
    public int Points { get; set; }
}

/// <summary>
/// NPC dialog Tkeys and responses TKeys
/// </summary>
public class NPC_DIALOG_MAP
{
    public string Name { get; set; }
    public string Emoji { get; set; }
    public string ResponseNice { get; set; }
    public string ResponseNeutral { get; set; }
    public string ResponseMean { get; set; }
}

/// <summary>
/// Friendship status with NPC
/// </summary>
public class FRIENDSHIP_MAP 
{
    public string Name { get; set; }
    public int Points { get; set; }
}

/// <summary>
/// NPC Data (name, attitude for now)
/// </summary>
public class NPC_DATA_MAP 
{
    public string Name { get; set; }
    public int Attitude { get; set; }
}

/// <summary>
/// Settings for the game
/// </summary>
public class SETTINGS_MAP
{ 
    public string LanugageCode { get; set; }
    public string AllLanguages { get; set; }
}


/// <summary>
/// Our save data
/// </summary>
public class SAVE_DATA_MAP 
{
    public string Name { get; set; }
    public string Inventory { get; set; }
    public string MapTiles { get; set; }
    public string MapItems { get; set; }
    public string Friendships { get; set; }

}






