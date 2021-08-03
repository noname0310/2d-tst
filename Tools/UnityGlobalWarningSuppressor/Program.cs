using System;
using System.IO;
using System.Text;

const string codeTitle = "//supressed warning by unity global warning supressor";
var supressList = File.ReadAllLines("SupressList.txt");
var supressionCodeBuilder = new StringBuilder();
supressionCodeBuilder.Append(codeTitle);
supressionCodeBuilder.Append('\n');
foreach (var item in supressList)
{
    supressionCodeBuilder.Append("#pragma warning disable ");
    supressionCodeBuilder.Append(item);
    supressionCodeBuilder.Append('\n');
}
supressionCodeBuilder.Append(codeTitle);
supressionCodeBuilder.Append('\n');
var supressionCode = supressionCodeBuilder.ToString();

Console.WriteLine("Adding Global Supress...");
var rootFolders = File.ReadAllLines("RootFolderList.txt");
foreach (var rootFolder in rootFolders)
{
    var files = Directory.GetFiles(rootFolder, "*.cs", SearchOption.AllDirectories);
    foreach (var file in files)
    {
        var lines = File.ReadAllLines(file);
        if (lines.Length < 0)
            continue;

        if (lines[0].Contains(codeTitle))
        {
            var i = 1;
            for (; !lines[i].Contains(codeTitle); i++) { }
            i += 1;
            var codeBuilder = new StringBuilder();
            codeBuilder.Append(supressionCode);
            for (; i < lines.Length; i++)
            {
                codeBuilder.Append(lines[i]);
                if (i != lines.Length - 1)
                    codeBuilder.Append('\n');
            }
            File.WriteAllText(file, codeBuilder.ToString());
        }
        else
            File.WriteAllText(file, supressionCode + File.ReadAllText(file));
        Console.WriteLine($"{file} is changed");
    }
}
