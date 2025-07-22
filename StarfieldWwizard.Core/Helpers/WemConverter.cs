using System.Diagnostics;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using Windows.Storage;
using BnkExtractor;
using FFMpegCore;
using Serilog;

namespace StarfieldWwizard.Core.Helpers;

public static class WemConverter
{
    public static string ConvertAndSaveWem2Ogg(byte[] wemBytes)
    {
        var tempWemPath = Path.ChangeExtension(Path.GetTempFileName(), "wem");
        
        Log.Information("Saving wem to {0}", tempWemPath);
        
        using (var fs = new FileStream(tempWemPath, FileMode.Create, FileAccess.Write))
        {
            fs.Write(wemBytes);
            fs.Close();
        }
        
        Extractor.ConvertWem(tempWemPath);
        return Path.ChangeExtension(tempWemPath, "ogg");
    }

    public static string Wem2Wav(byte[] wemBytes)
    {
        var wemHash = Convert.ToHexString(MD5.HashData(wemBytes));
        var tempWemPath = Path.ChangeExtension(Path.Combine(Path.GetTempPath(), wemHash), "wem");
        var tempWavPath = Path.ChangeExtension(tempWemPath, "wav");

        if (File.Exists(tempWemPath))
        {
            Log.Information("Existing wem file found at {0}", tempWemPath);
        }
        else
        {
            Log.Information("Saving wem to {0}", tempWemPath);

            using var fs = new FileStream(tempWemPath, FileMode.Create, FileAccess.Write);
            fs.Write(wemBytes);
            fs.Close();
        }

        if (File.Exists(tempWavPath))
        {
            Log.Information("Existing wav found at {0}, will use that instead of converting", tempWavPath);
        }
        else
        {
            var procInfo = new ProcessStartInfo("vgmstream-cli.exe", $"-o \"{tempWavPath}\" \"{tempWemPath}\"")
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true
            };

            var proc = new Process()
            {
                StartInfo = procInfo
            };

            proc.Start();
            proc.WaitForExit();
        }

        return tempWavPath;
    }

    public static void Ogg2Wav(string oggPath, string wavPath)
    {
        try
        {
            Log.Information("Converting ogg at path {0} to {1}", oggPath, wavPath);
            var directoryName = Path.GetDirectoryName(wavPath);
            if (directoryName != null && !Path.Exists(directoryName))
            {
                Log.Information($"Directory {directoryName} does not exist, creating it.");
                Directory.CreateDirectory(directoryName);
            }

            Log.Information($"Writing converted wem file to wav file at {wavPath.Replace("/", "\\")}");
            FFMpegArguments
                .FromFileInput(oggPath)
                .OutputToFile(wavPath)
                .ProcessSynchronously();
        }
        catch (Exception e)
        {
            Log.Error("There was an error converting the ogg file to wav", e);
            throw;
        }
    }
}