using Aspose.Diagram;

using Comindware.VSDX2BPMN.Models;

using System.Text;
using System.Xml;
using System.Xml.Serialization;

using Converter = Comindware.VSDX2BPMN.Converter;

using Comindware.VSDX2BPMN.Сonstants;

var path = GetPath();

var diagrams = GetFiles(path);

if (!diagrams.Any())
{
    return;
}

foreach (var diagram in diagrams)
{
    ConvertFile(diagram);
}

Console.WriteLine(Messages.SuccessMessage);
Console.ReadKey();

void ConvertFile(Comindware.VSDX2BPMN.Models.File diagram)
{
    if (!diagram.Diagram.Pages.IsExist(0))
    {
        Console.WriteLine(string.Format(Messages.NoPages, diagram.FileName));
        return;
    }

    Console.WriteLine(string.Format(Messages.FileProcess, diagram.FileName));

    foreach (Page page in diagram.Diagram.Pages)
    {
        var pageName = page.Name;

        var conventer = new Converter();

        var model = conventer.Convert(page);

        if (model != null)
        {
            CreateBpmnFile(model, diagram.FileName, pageName);
        }
    }
}

void CreateBpmnFile(Definition model, string filename, string pageName)
{
    var serializer = new XmlSerializer(model.GetType());

    var settings = new XmlWriterSettings();
    settings.Encoding = Encoding.UTF8;

    var bpmnFileName = $"{Path.GetFileNameWithoutExtension(filename)}{Common.UnderliningSymbol}{pageName}{Common.BpmnExtension}";

    if (!Directory.Exists(Path.Combine(path, Common.OutputFolderName)))
    {
        Directory.CreateDirectory(Path.Combine(path, Common.OutputFolderName));
    }

    using (var fileStream = new FileStream(Path.Combine(path, Common.OutputFolderName, bpmnFileName), FileMode.Create))
    {
        using (var xw = XmlWriter.Create(fileStream, settings))
        {
            serializer.Serialize(xw, model);
            Console.WriteLine(string.Format(Messages.FileCreated, bpmnFileName));
        }
    }
}

string GetPath()
{
    Console.WriteLine(Messages.PathToFolder);
    var path = Console.ReadLine();

    if (!Directory.Exists(path))
    {
        Console.WriteLine(Messages.IncorrectPath);
        return GetPath();
    }
    else
    {
        return path;
    }
}

List<Comindware.VSDX2BPMN.Models.File> GetFiles(string path)
{
    var result = new List<Comindware.VSDX2BPMN.Models.File>();

    if (Directory.Exists(path))
    {
        var paths = Directory.GetFiles(path, Common.VSDXSearchQuery);

        foreach (var item in paths)
        {
            result.Add(new Comindware.VSDX2BPMN.Models.File { FileName = Path.GetFileName(item), Diagram = new Aspose.Diagram.Diagram(item) });
        }
    }
    else
    {
        Console.WriteLine(Messages.IncorrectPath);
        return result;
    }

    if (!result.Any())
    {
        Console.WriteLine(Messages.NoVisioFiles);
    }

    return result;
}