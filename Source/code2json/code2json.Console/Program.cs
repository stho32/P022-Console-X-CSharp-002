using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

// try this next time:
// https://stackoverflow.com/questions/28234052/finding-all-class-declarations-than-inherit-from-another-with-roslyn


var directory = Directory.GetCurrentDirectory();
directory = directory.Remove(directory.IndexOf("Source"));

var files = Directory.GetFiles(directory, "*.cs", SearchOption.AllDirectories);

Console.WriteLine($"Starting analysis... {directory}...");

var classes = new ClassDeclarationCollector();

foreach (var file in files)
{
    var code = File.ReadAllText(file);
    var tree = CSharpSyntaxTree.ParseText(code);
    
    classes.Visit(tree.GetCompilationUnitRoot());
}

foreach (var someClass in classes.classDeclarations)
{
    Console.WriteLine($"- class {someClass.Identifier.Text}");
    if (someClass?.BaseList?.Types != null)
    {
        foreach (var derivedFromType in someClass.BaseList.Types)
        {
            Console.WriteLine($"    <- {derivedFromType.GetText().ToString().Trim()}");
        }
    }
}

Console.WriteLine("Finished.");


class ClassDeclarationCollector : CSharpSyntaxWalker
{
    public ICollection<ClassDeclarationSyntax> classDeclarations { get; } = new List<ClassDeclarationSyntax>();

    public override void VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        classDeclarations.Add(node);
    }
}

public interface ISomeThing
{
    void Hey();
}

public abstract class AnotherClass : ISomeThing
{
    public abstract void Hey();
}

public class MoreOfAnotherClass : AnotherClass
{
    public override void Hey()
    {
        Console.WriteLine("Dudelidu");
    }
}