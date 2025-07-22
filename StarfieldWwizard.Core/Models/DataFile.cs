namespace StarfieldWwizard.Core.Models;

public class DataFile
{
    public string Path
    {
        get;
        init;
    }

    // todo: just get this dynamically from path
    public string Filename
    {
        get;
        init;
    }

    public byte[] Data
    {
        get;
        init;
    }
}