using System;
using System.Collections.Generic;

namespace TestQuestionParser.Data;

public partial class Question
{
    public int Id { get; set; }

    public int? QuestionId { get; set; }

    public string? InternalId { get; set; }

    public string? Question1 { get; set; }

    public string? Answer1 { get; set; }

    public string? Answer2 { get; set; }

    public string? Answer3 { get; set; }

    public string? Answer4 { get; set; }

    public int? CorrectAnswer { get; set; }

    public sbyte? PplA { get; set; }

    public sbyte? PplH { get; set; }

    public sbyte? CplA { get; set; }

    public sbyte? CplH { get; set; }

    public sbyte? AtplA { get; set; }

    public sbyte? AtplH { get; set; }

    public sbyte? IrA { get; set; }

    public sbyte? IrH { get; set; }

    public string? Explanation { get; set; }

    public bool? B11 { get; set; }

    public bool? B12 { get; set; }

    public bool? B2 { get; set; }

    public string? ImageName { get; set; }

    public string? ImageLink { get; set; }

    public string? Chapter { get; set; }
}
