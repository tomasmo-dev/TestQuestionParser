using System.Data;
using System.Reflection.Emit;
using System.Text;
using ExcelDataReader;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TestQuestionParser.Data;
using TestQuestionParser.Models;

namespace TestQuestionParser.Services;

public class FileService
{
    private readonly SystemContext _context;
    private readonly ILogger<FileService> _logger;
    
    private static readonly string[] ALLOWED_TYPES = new[] { "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" };
    
    public FileService(SystemContext context, ILogger<FileService> logger)
    {
        _context = context;
        _logger = logger;
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
    }

    private bool ValidateExcelFile(IFormFile file)
    {
        if (file == null)
        {
            return false;
        }
        
        var mimeType = file!.ContentType;
        
        if (!ALLOWED_TYPES.Contains(mimeType))
        {
            return false;
        }
        
        return true;
    }

    private IList<Question> ParseWorksheet(DataTable workbook, HomeIndexUploadModel model)
    {
        var numRows = workbook.Rows.Count;
        
        int correctColIndex = model.CorrectColIndex;
        int labelColIndex = model.LabelColIndex;
        int valueColIndex = model.ValueColIndex;
        int imageColIndex = model.ImageColIndex;
        
        var questions = new List<Question>();
        
        bool QuestionsStarted = false;
        Question? question = null;
        int answerIndex = 0;
        
        // one so it skips header row
        for (int i = 1; i < numRows; i++)
        {
            var row = workbook.Rows[i];
            
            var label = row[labelColIndex].ToString() ?? "";
            var value = row[valueColIndex].ToString() ?? "";
            var correct = row[correctColIndex].ToString() ?? "";
            var image = row[imageColIndex].ToString() ?? "";
            
            LabelType labelType;

            if (QuestionMappings.LabelMappings.ContainsKey(label.ToLower()))
            {
                labelType = QuestionMappings.LabelMappings[label.ToLower()];
            }
            else
            {
                labelType = LabelType.Invalid;
            }
            
            switch (labelType)
            {
                case LabelType.Question:
                    if (QuestionsStarted)
                    {
                        question.Question1 = value;
                    }
                    break;
                
                case LabelType.Answer:
                    if (QuestionsStarted)
                    {
                        if (answerIndex == 0)
                        {
                            question.Answer1 = value;
                        }
                        else if (answerIndex == 1)
                        {
                            question.Answer2 = value;
                        }
                        else if (answerIndex == 2)
                        {
                            question.Answer3 = value;
                        }
                        else if (answerIndex == 3)
                        {
                            question.Answer4 = value;
                        }
                        else
                        {
                            throw new Exception($"Too many answers on question : {question.InternalId} | answer count : {answerIndex + 1}");
                        }
                    
                        answerIndex++;
                    
                        if (correct != "")
                        {
                            question.CorrectAnswer = answerIndex;
                        }
                    }
                    break;
                
                case LabelType.Explanation:
                    if (QuestionsStarted)
                    {
                        question.Explanation = value;

                        question.Chapter = workbook.TableName;
                    
                        questions.Add(question);
                    
                        QuestionsStarted = false;
                        question = null;
                        answerIndex = 0;
                    }
                    break;
                
                case LabelType.Number:
                    if (question != null)
                    {
                        question.Chapter = workbook.TableName;
                    
                        questions.Add(question);
                    
                        QuestionsStarted = false;
                        question = null;
                        answerIndex = 0;
                    }

                    if (value.Contains("84 / 174") || value.Contains("64 / 537"))
                    {
                        continue;
                    }
                    
                    QuestionsStarted = true;
                    question = new Question();
                    question.InternalId = value;
                    break;
                
                default:
                case LabelType.Invalid:
                    QuestionsStarted = false;
                    continue;
            }
            
            if (image != "" && QuestionsStarted)
            {
                question.ImageLink = $"{question.ImageLink};{image}";
            }
        }

        return questions;
    }

    public async Task<bool> ParseExcelFile(HomeIndexUploadModel model)
    {
        var valid = ValidateExcelFile(model.ExcelFile);
        if (!valid) return false;
        
        var allQuestions = new List<Question>();

        using (Stream file = model.ExcelFile.OpenReadStream())
        {
            IExcelDataReader reader = ExcelReaderFactory.CreateReader(file);
            var dataset = reader.AsDataSet();

            IList<DataTable> workbooks = dataset.Tables
                .Cast<DataTable>()
                .ToList()
                .Skip(model.SkipWorksheetCount)
                .ToList();

            foreach (var workbook in workbooks)
            {
                try
                {
                    var questions = ParseWorksheet(workbook, model);
                    allQuestions.AddRange(questions);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Error parsing worksheet : {workbook.TableName}");
                    _logger.LogError(e, $"Error message : {e.Message}");
                    _logger.LogError(e, $"Error stack trace : {e.StackTrace}");
                    return false;
                }
            }

            var questionChunks = allQuestions.Chunk(500);

            try
            {
                var tasks = questionChunks.Select(async chunk =>
                {
                    await using var threadContext = new SystemContext(_context.Options);
                    await threadContext.Questions.AddRangeAsync(chunk);
                    await threadContext.SaveChangesAsync();
                });
            
                await Task.WhenAll(tasks);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error saving questions to database");
                _logger.LogError(e, $"Error message : {e.Message}");
                _logger.LogError(e, $"Error stack trace : {e.StackTrace}");
                _logger.LogError(e, "Deleting all insterted questions from database");
                
                _context.Questions.RemoveRange(allQuestions);
                
                return false;
            }
            
        }

        return true;
    }
}

static class QuestionMappings
{
    public static readonly Dictionary<string, LabelType> LabelMappings = new Dictionary<string, LabelType> // mappings for the labels in the excel file
    {
        { "question", LabelType.Question},
        { "answer", LabelType.Answer},
        { "explanation", LabelType.Explanation},
        { "number", LabelType.Number},
        { "", LabelType.Invalid}
    };
}

public enum LabelType
{
    Question,
    Answer,
    Explanation,
    Number,
    Invalid
}