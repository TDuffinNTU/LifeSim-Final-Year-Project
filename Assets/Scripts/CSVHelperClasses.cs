using CsvHelper;
using System;
using System.Globalization;
using System.IO;
using System.Linq;


// region code wasn't found
public class RegionCodeNotFoundException : Exception
{
    public RegionCodeNotFoundException()
    {
    }

    public RegionCodeNotFoundException(string message) 
        : base(message)
    {        
    }
}

// primary key was not found
public class PrimaryKeyNotFoundException : Exception
{
    public PrimaryKeyNotFoundException()
    {
    }

    public PrimaryKeyNotFoundException(string message)
        : base(message)
    {
    }
}

abstract class CSVData 
{
    // find location of csv data
    public virtual string GetPath() { return "Assets/Databases/"; }

    // find primary key
    public abstract string GetPrimaryKey();

    // query csv by primary key, type T must match signature of class being entered
    public T GetDataByPrimaryKey<T>(string pkey) 
    {      
        // get path of child class and open in csv helper
        using (var reader = new StreamReader(this.GetPath()))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            // get each record and search sequential
            var records = csv.GetRecords<T>();
            
            foreach(var record in records)
            {
                // casting to CSVData parent class
                CSVData data = (CSVData)(object) record;
                if (data.GetPrimaryKey() == pkey) 
                {
                    return record;
                }
            }
        }

        throw new PrimaryKeyNotFoundException(pkey);
    }

}

class TRANSLATIONS : CSVData
{
    // CSV HELPER CLASS
    public string Name { get; set; }
    public string EN { get; set; }
    public string CY { get; set; }

    public override string GetPath()
    {
        var p = base.GetPath();
        return p + "TRANSLATIONS.CSV";
    }

    public override string GetPrimaryKey()
    {
        return this.Name;
    }

    // avoid hard-coding translation strings
    public string GetTranslationWithCode(string code) 
    {
        return code.ToUpper() switch
        {
            "EN"    => EN,
            "CY"    => CY,
            _       => throw new RegionCodeNotFoundException(code),
        };
    }
}


class NPCDIALOG : CSVData
{
    // CSV HELPER CLASS
    public string Name { get; set; }
    public string Emoji { get; set; }
    public string ResponseNice { get; set; }
    public string ResponseNeutral { get; set; }
    public string ResponseMean { get; set; }    

    public override string GetPath()
    {
        var p = base.GetPath();
        return p + "NPCDIALOG.CSV";
    }

    public override string GetPrimaryKey()
    {
        return this.Name;
    }

    public string[] GetEmoji(int count)
    {
        string[] arr = Emoji.Split(',');
        return arr.Take(count).ToArray();
    }
}

class PLAYERRESPONSES : TRANSLATIONS
{    
    // pretty much inherits everything but the path
    public override string GetPath()
    {
        var p = "Assets/Databases/";
        return p + "PLAYERRESPONSES.CSV";
    }       
}

