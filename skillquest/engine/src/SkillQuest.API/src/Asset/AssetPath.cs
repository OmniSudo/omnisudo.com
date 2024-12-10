namespace SkillQuest.API.Asset;

public class AssetPath{
    public static bool IsFileBelowDirectory(string fileInfo, string directoryInfo, string separator)
    {
        fileInfo = Path.GetFullPath(fileInfo);
        var directoryPath = string.Format("{0}{1}"
            , directoryInfo
            , directoryInfo.EndsWith(separator) ? "": separator);

        return fileInfo.StartsWith(directoryPath, StringComparison.OrdinalIgnoreCase);
    }
    
    public static bool IsSafePath(string path){
        var cwd = Environment.CurrentDirectory;
        return IsFileBelowDirectory(path, cwd, "/");
    }

    public static string Sanitize(string path){
        return IsSafePath(path) ? path : throw new ArgumentException($"'{path}' escapes sandbox");
    }
}
