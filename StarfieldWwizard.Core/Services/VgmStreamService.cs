using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Text;
using Noggog;
using Serilog;
using StarfieldWwizard.Core.Contracts.Services;

namespace StarfieldWwizard.Core.Services;

public class VgmStreamService : IVgmStreamService
{
    private readonly string vgmDownloadUrl =
        "https://github.com/vgmstream/vgmstream/releases/download/r2023/vgmstream-win64.zip";
    
    [DllImport("shlwapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
    static extern bool PathFindOnPath([In, Out] StringBuilder pszFile, [In] string[] ppszOtherDirs);

    public bool VgmStreamOnPath()
    {
        var sb = new StringBuilder("vgmstream-cli.exe", 260);
        if (PathFindOnPath(sb, null))
        {
            var pathToVgm = sb.ToString();
            Log.Information("Path to vgmstream found at {0}", pathToVgm);

            return !pathToVgm.IsNullOrEmpty();
        }

        return false;
    }

    public bool VgmStreamInBaseDirectory() => File.Exists(Path.Join(AppContext.BaseDirectory, "vgmstream-cli.exe"));

    public async Task DownloadVgmStreamIfNotExists()
    {
        Log.Information("Looking for vgmstream in PATH or base directory");
        if (!VgmStreamOnPath() && !VgmStreamInBaseDirectory())
        {
            Log.Information("vgmstream not found in PATH or base directory, downloading from {0}", vgmDownloadUrl);
            using var httpClient = new HttpClient();
            var dlPath = AppContext.BaseDirectory;
            await using var stream = await httpClient.GetStreamAsync(vgmDownloadUrl);
            
            var vgmStreamZipPath = Path.Combine(dlPath, "vgmstream-win64.zip");
            await using var fileWriter = File.OpenWrite(vgmStreamZipPath);
            await stream.CopyToAsync(fileWriter);
            fileWriter.Close();
            Log.Information("vgmstream completed download, at path {0}", vgmStreamZipPath);

            Log.Information("Unzipping vgmstream to {0}", dlPath);
            ZipFile.ExtractToDirectory(vgmStreamZipPath, dlPath, true);
            Log.Information("vgmstream-win64.exe should be found at {0}", Path.Combine(dlPath, "vgmstream-cli.exe"));
        }
        else
        {
            Log.Information("vgmstream-cli.exe already exists, skipping download");
        }
    }
}