namespace TestQuestionParser.Models;

public class HomeIndexUploadModel
{
    public IFormFile? ExcelFile { get; set; }
    
    public int SkipWorksheetCount { get; set; }
    
    public int CorrectColIndex { get; set; }
    public int LabelColIndex { get; set; }
    public int ValueColIndex { get; set; }
    public int ImageColIndex { get; set; }
}