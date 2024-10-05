// Controllers/PdfController.cs
using Microsoft.AspNetCore.Mvc;
using iText.Kernel.Pdf;
using iText.Layout.Borders;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Colors;
using iText.IO.Image; // For ImageDataFactory
using System.Linq; // For LINQ queries
using QPGS.Models; // Your model namespace
using System.Collections.Generic; // Needed for List<T>
namespace QPGS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PdfController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public PdfController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpPost("generate")]
        public IActionResult GeneratePdf([FromBody] PdfRequestModel request)
        {
            try
            {
                // Create a PDF document in memory
                using (var stream = new MemoryStream())
                {
                    using (var writer = new PdfWriter(stream))
                    {
                        using (var pdf = new PdfDocument(writer))
                        {
                            var document = new Document(pdf);

                            // Add the title
                            document.Add(CreateParagraph("Rubric-Based Question Paper", 20, TextAlignment.CENTER, true, 20));

                            // Add Class and Subject
                            document.Add(CreateParagraph("Class: 1", 16, TextAlignment.LEFT, true));
                            document.Add(CreateParagraph("Subject: English", 16, TextAlignment.LEFT, true, 20));

                            // Add Rubrics Section
                            document.Add(CreateParagraph("Rubrics:", 14, TextAlignment.LEFT, true, 10));
                            if (request.R1)
                            {

                                document.Add(new Paragraph("1. Remember and Identification")
                                .SetFontSize(12));
                            }
                            if (request.R2)
                            {

                                document.Add(new Paragraph("2. Use of items")
                                    .SetFontSize(12));
                            }
                            if (request.R3)
                            {
                                document.Add(new Paragraph("3. Understanding")
                                    .SetFontSize(12)
                                    .SetMarginBottom(20));
                            }
                            // Conditional Rubric Sections
                            if (request.R1)
                            {
                                AddRememberAndIdentificationSection(document, request.chapterId);
                            }

                            if (request.R2)
                            {
                                AddUseOfItemsSection(document, request.chapterId);
                            }

                            if (request.R3)
                            {
                                AddUnderstandingSection(document, request.chapterId);
                            }

                            document.Close();
                        }
                    }

                    // Return the PDF as a file response
                    var pdfBytes = stream.ToArray();
                    return File(pdfBytes, "application/pdf", "rubric_based_question_paper.pdf");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private Paragraph CreateParagraph(string text, float fontSize, TextAlignment alignment = TextAlignment.LEFT, bool bold = false, float marginBottom = 0)
        {
            var paragraph = new Paragraph(text)
                .SetFontSize(fontSize)
                .SetTextAlignment(alignment)
                .SetMarginBottom(marginBottom);
            if (bold)
            {
                paragraph.SetBold();
            }
            return paragraph;
        }

        private void AddRememberAndIdentificationSection(Document document, int chapterId)
        {
            document.Add(new Paragraph("Remember and Identification")
                .SetFontSize(14)
                .SetFontColor(new iText.Kernel.Colors.DeviceRgb(55, 96, 146))  // Set the color here
                .SetMarginBottom(10));

            // Fetch questions of type "match" for "Match the picture with the correct word"
            var matchQuestions = _context.Questions
                .Where(q => q.ChapterId == chapterId && q.Type == "match")
                .ToList();

            // Prepare a list to store names and corresponding image links
            var matchingPairs = matchQuestions.Select(q => q.QuestionText.Split(",,"))
                                              .Where(parts => parts.Length == 2)
                                              .ToList();

            // Extract images and names separately
            var images = matchingPairs.Select(pair => pair[0]).ToList(); // List of image links
            var names = matchingPairs.Select(pair => pair[1]).ToList();  // List of names

            // Shuffle the names and ensure the shuffled list doesn't match the original pairing
            var random = new Random();
            List<string> shuffledNames;

            do
            {
                shuffledNames = names.OrderBy(x => random.Next()).ToList();
            } while (shuffledNames.Where((name, index) => names[index] == name).Any());

            // Add the "Match the picture with the correct word" section
            document.Add(CreateParagraph("1. Match the picture with the correct word:", 12));

            // Create a table with 2 columns: one for images and one for names
            var table = new Table(2)
                .SetWidth(UnitValue.CreatePercentValue(100)); // Set table width to fill the page

            // Add images and shuffled names to the table row by row
            for (int i = 0; i < images.Count; i++)
            {
                // Create the image
                var image = new Image(ImageDataFactory.Create(images[i])) // Create the image
                    .SetWidth(100) // Set the desired width
                    .SetHeight(100); // Set the desired height

                // Add the image to the first cell with no border
                table.AddCell(new Cell().Add(image).SetBorder(Border.NO_BORDER));

                // Add the shuffled name with a checkbox to the second cell with vertical alignment and no border
                table.AddCell(new Cell()
                    .Add(new Paragraph($"{shuffledNames[i]} ") // Add text with checkbox
                    .SetFontSize(12))
                    .SetVerticalAlignment(VerticalAlignment.MIDDLE) // Center the text vertically
                    .SetPaddingLeft(15) // Add left padding for space between image and text
                    .SetBorder(Border.NO_BORDER) // Remove cell border
                );
            }

            // Add the table to the document
            document.Add(table);









            // Add the "Identify the object and write its name" section
            document.Add(CreateParagraph("\n2. Identify the object and write its name:", 12));

            // Fetch questions of type "identify"
            var identifyQuestions = _context.Questions
                .Where(q => q.ChapterId == chapterId && q.Type == "identify")
                .ToList();

            foreach (var question in identifyQuestions)
            {
                // Assuming the question text contains only the image link
                var imageLink = question.QuestionText; // Get the image link

                // Create a new paragraph
                var paragraph = new Paragraph()
                    .Add(new Image(ImageDataFactory.Create(imageLink)) // Add the image
                        .SetWidth(100) // Set the desired width
                        .SetHeight(100)) // Set the desired height
                    .Add(" What is this? - __________"); // Add the prompt with a blank line

                // Add the paragraph to the document
                document.Add(paragraph);
            }


        }


        private void AddUseOfItemsSection(Document document, int chapterId)
        {
            // Add section header
            document.Add(new Paragraph("Use of terms")
                .SetFontSize(14)
                .SetFontColor(new iText.Kernel.Colors.DeviceRgb(55, 96, 146)) // Set the color here
                .SetMarginBottom(10));

            // Fetch questions of type "mark" for "Mark the correct answer"
            var markQuestions = _context.Questions
                .Where(q => q.ChapterId == chapterId && q.Type == "mark")
                .ToList();

            // Add the "Mark the correct answer" section
            document.Add(CreateParagraph("3. Mark the correct answer:", 12));
            for (int i = 0; i < markQuestions.Count; i++)
            {
                // Generate label (a, b, c, d, ...)
                char label = (char)('a' + i);
                document.Add(CreateParagraph($"{label}) {markQuestions[i].QuestionText}  \n [ ] True  \n [ ] False", 12));
            }

            // Fetch questions of type "choose" for "Choose the correct word"
            var chooseQuestions = _context.Questions
                .Where(q => q.ChapterId == chapterId && q.Type == "choose")
                .ToList();

            // Add the "Choose the correct word" section
            document.Add(CreateParagraph("\n4. Choose the correct word:", 12));
            for (int i = 0; i < chooseQuestions.Count; i++)
            {
                // Generate label (a, b, c, d, ...)
                char label = (char)('a' + i);
                document.Add(CreateParagraph($"{label}) {chooseQuestions[i].QuestionText}", 12));
            }
        }


        private void AddUnderstandingSection(Document document, int chapterId)
        {
            document.Add(new Paragraph("Understanding")
                .SetFontSize(14)
                .SetFontColor(new iText.Kernel.Colors.DeviceRgb(55, 96, 146))  // Set the color here
                .SetMarginBottom(10));

            // Fetch questions of type "blanks" for "Fill in the blanks"
            var blankQuestions = _context.Questions
                .Where(q => q.ChapterId == chapterId && q.Type == "blanks")
                .ToList();

            document.Add(CreateParagraph("5. Fill in the blanks:", 12));
            for (int i = 0; i < blankQuestions.Count; i++)
            {
                char label = (char)('a' + i); // Generate label (a, b, c, d, ...)
                document.Add(CreateParagraph($"{label}) {blankQuestions[i].QuestionText}", 12, marginBottom: 10)
                    .SetMultipliedLeading(1.5f));
            }

            // Fetch questions of type "complete" for "Complete the sentence"
            var completeQuestions = _context.Questions
                .Where(q => q.ChapterId == chapterId && q.Type == "complete")
                .ToList();

            document.Add(CreateParagraph("\n6. Complete the sentence:", 12));
            for (int i = 0; i < completeQuestions.Count; i++)
            {
                char label = (char)('a' + i); // Generate label (a, b, c, d, ...)
                document.Add(CreateParagraph($"{label}) {completeQuestions[i].QuestionText}", 12)
                    .SetMultipliedLeading(1.5f));
            }
        }

    }

    public class PdfRequestModel
    {
        public bool R1 { get; set; }
        public bool R2 { get; set; }
        public bool R3 { get; set; }

        public int chapterId { get; set; }
    }
}
