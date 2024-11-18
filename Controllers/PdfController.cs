// Controllers/PdfController.cs
using Microsoft.AspNetCore.Mvc;
using iText.Kernel.Pdf;
using iText.Layout.Borders;
using iText.Layout;
using iText.Layout.Element;
using iText.Kernel.Font;
using iText.Layout.Properties;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.IO.Image; // For ImageDataFactory
using QPGS.Models;
using Microsoft.AspNetCore.Authorization;
using iText.IO.Font.Constants; // Needed for List<T>
namespace QPGS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PdfController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public int questionNumber = 1;
        public int totalMarks = 0;
        public int numberOfQuestions = 0;
        public int totalSections = 0;

        List<string> q1 = new List<string>();
        List<string> q2 = new List<string>();
        List<string> q3 = new List<string>();
        List<string> q4 = new List<string>();
        List<string> q5 = new List<string>();
        List<string> q6 = new List<string>();

        public PdfController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        [HttpPost("generate")]
        public IActionResult GeneratePdf([FromBody] PdfRequestModel request)
        {
            if (request.R1)
            {
                totalMarks += 1 * request.number;
                totalSections++;
            }
            if (request.R2)
            {
                totalMarks += 1 * request.number;
                totalSections++;
            }
            if (request.R3)
            {
                totalMarks += 1 * request.number;
                totalSections++;
            }
            numberOfQuestions = request.number;
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

                            var font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

                            // Add content to the document (text)
                            document.Add(CreateParagraph("SZABIST SCHOOL LARKANA", 20, TextAlignment.CENTER, true, 20));

                            // Get current page and create a PdfCanvas instance
                            var page = pdf.GetPage(1);
                            var canvas = new PdfCanvas(page);

                            // Get the page height for positioning the canvas correctly
                            float pageHeight = page.GetPageSize().GetHeight();


                            canvas.BeginText()
                                  .SetFontAndSize(font, 12)
                                  .MoveText(40, pageHeight - 120)
                                  .ShowText("Subject: English")
                                  .MoveText(230, 0) // Move down for the next line
                                  .ShowText("Class: 1")
                                  .MoveText(220, 0)
                                  .ShowText($"Marks: {totalMarks * 2}");

                            // End the text mode
                            canvas.EndText();

                            var imagePath = System.IO.Path.Combine(_env.WebRootPath, "Assets/szabist-logo.png");
                            var imageData = ImageDataFactory.Create(imagePath);
                            var image = new Image(imageData);


                            image.ScaleToFit(80, 80);  // Sets image to fit within 200x200 box

                            // Set the position on the page
                            image.SetFixedPosition(44, pageHeight - 87);

                            // Add the image to the document
                            document.Add(image);

                            // Draw a line (just as an example)
                            var line = new LineSeparator(new SolidLine(1f));
                            line.SetWidth(520f)
                                .SetMarginTop(40f)
                                .SetMarginBottom(20f);

                            document.Add(line); // Add the line to the document


                            document.Add(CreateParagraph($"Answer the following questions                                            {totalSections} * {numberOfQuestions * 2} = {totalMarks * 2}  ", 15, TextAlignment.LEFT, true, 10));


                            // Add rubric sections based on the request
                            // Add sections based on the request
                            if (request.R1) AddRememberAndIdentificationSection(document, request.chapterId, request.number);
                            if (request.R2) AddUseOfItemsSection(document, request.chapterId, request.number);
                            if (request.R3) AddUnderstandingSection(document, request.chapterId, request.number);

                            // Explicitly add a new page for the "Answers" section
                            document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));

                            // Add the "Answers" heading
                            // Add the "Answers" heading with proper formatting
                            document.Add(new Paragraph("Answers")
                                .SetFontSize(16)
                                .SetBold()
                                .SetTextAlignment(TextAlignment.CENTER) // Center the title
                                .SetMarginBottom(20) // Add space below the title
                            );

                            // Combine all question lists into an array
                            List<string>[] questionLists = { q1, q2, q3, q4, q5, q6 };

                            // Initialize question number for tracking
                            int questionNumber = 1;

                            // Iterate through the lists
                            foreach (var questionList in questionLists)
                            {
                                if (questionList.Count > 0) // Check if the list is not empty
                                {
                                    // Add a question heading with spacing and style
                                    document.Add(new Paragraph($"Question {questionNumber}")
                                        .SetFontSize(14)
                                        .SetBold()
                                        .SetMarginBottom(10) // Space below the question heading
                                        .SetMarginTop(20)    // Space above the question heading
                                    );

                                    // Create a table to align options (a, b, c, etc.) in a structured way
                                    var table = new Table(2)
                                        .SetWidth(UnitValue.CreatePercentValue(100)) // Full-width table
                                        .SetMarginBottom(15); // Add space below the table

                                    char option = 'a'; // Start with 'a'
                                    foreach (var answer in questionList)
                                    {
                                        // Add the option (a, b, c) to the first column
                                        table.AddCell(new Cell()
                                            .Add(new Paragraph($"{option})")
                                                .SetFontSize(12)
                                                .SetBold())
                                            .SetBorder(Border.NO_BORDER) // Remove cell border for cleaner look
                                            .SetTextAlignment(TextAlignment.RIGHT) // Right-align the option
                                        );

                                        // Add the answer text to the second column
                                        table.AddCell(new Cell()
                                            .Add(new Paragraph(answer)
                                                .SetFontSize(12))
                                            .SetBorder(Border.NO_BORDER) // Remove cell border for cleaner look
                                            .SetTextAlignment(TextAlignment.LEFT) // Left-align the answer
                                        );

                                        option++; // Increment to the next letter
                                    }

                                    // Add the table to the document
                                    document.Add(table);

                                    questionNumber++; // Increment question number for the next non-empty list
                                }
                            }

                            // Close the document after adding all content
                            document.Close();

                        }
                    }

                    // Convert stream to byte array for response
                    var pdfBytes = stream.ToArray();

                    // Set Content-Disposition header for inline display
                    var result = new FileContentResult(pdfBytes, "application/pdf")
                    {
                        FileDownloadName = "rubric_based_question_paper.pdf"
                    };
                    Response.Headers["Content-Disposition"] = "inline; filename=rubric_based_question_paper.pdf";
                    return result;


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

        private void AddRememberAndIdentificationSection(Document document, int chapterId, int number)
        {

            // Fetch questions of type "match" for "Match the picture with the correct word"
            var matchQuestions = _context.Questions
                .Where(q => q.ChapterId == chapterId && q.Type == "match")
                .ToList();

            ShuffleArray(matchQuestions);
            matchQuestions = matchQuestions.GetRange(0, number);
            for (int i = 0; i < matchQuestions.Count; i++)
            {
                q1.Add(matchQuestions[i].AnswerText);
            }
            // Prepare a list to store names and corresponding image links
            var matchingPairs = matchQuestions.Select(q => q.QuestionText.Split(",,"))
                                              .Where(parts => parts.Length == 2)
                                              .ToList();

            // Extract images and names separately
            var images = matchingPairs.Select(pair => pair[0]).ToList(); // List of image links
            var names = matchingPairs.Select(pair => pair[1]).ToList();  // List of names

            // Shuffle the names and ensure the shuffled list doesn't match the original pairing
            var random = new Random();
            List<string> shuffledNames = new List<string>(); ;
            int maxAttempts = 5;

            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                shuffledNames = names.OrderBy(x => random.Next()).ToList();
                if (!shuffledNames.Where((name, index) => names[index] == name).Any())
                    break;
            }
            // Add the "Match the picture with the correct word" section
            document.Add(CreateParagraph($"Q.No.{questionNumber} Match the picture with the correct word", 14, TextAlignment.LEFT, true, 0));

            // Create a table with 2 columns: one for images and one for names
            var table = new Table(2)
                .SetWidth(UnitValue.CreatePercentValue(100)); // Set table width to fill the page

            // Add images and shuffled names to the table row by row
            for (int i = 0; i < images.Count; i++)
            {
                // Create the image
                var image = new Image(ImageDataFactory.Create(Path.Combine(_env.WebRootPath, $"Assets/{images[i]}"))) // Create the image
                    .SetWidth(100) // Set the desired width
                    .SetHeight(100)
                    .SetMarginTop(10); // Set the desired height

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
            questionNumber++;
            // Add the "Identify the object and write its name" section
            document.Add(CreateParagraph($"\nQ.No.{questionNumber} Identify the object and write its name", 14, TextAlignment.LEFT, true, 0));

            // Fetch questions of type "identify"
            var identifyQuestions = _context.Questions
                .Where(q => q.ChapterId == chapterId && q.Type == "identify")
                .ToList();
            ShuffleArray(identifyQuestions);
            identifyQuestions = identifyQuestions.GetRange(0, number);
            foreach (var question in identifyQuestions)
            {
                q2.Add(question.AnswerText);
                // Assuming the question text contains only the image link
                var imageLink = question.QuestionText; // Get the image link

                // Create a new paragraph
                var paragraph = new Paragraph()
                    .Add(new Image(ImageDataFactory.Create(Path.Combine(_env.WebRootPath, $"Assets/{imageLink}"))) // Add the image
                        .SetWidth(100) // Set the desired width
                        .SetHeight(100))
                        .SetMarginTop(10) // Set the desired height
                    .Add("           What is this? - __________"); // Add the prompt with a blank line

                // Add the paragraph to the document
                document.Add(paragraph);
            }


        }


        private void AddUseOfItemsSection(Document document, int chapterId, int number)
        {


            // Fetch questions of type "mark" for "Mark the correct answer"
            var markQuestions = _context.Questions
                .Where(q => q.ChapterId == chapterId && q.Type == "mark")
                .ToList();
            ShuffleArray(markQuestions);
            questionNumber++;
            // Add the "Mark the correct answer" section
            document.Add(CreateParagraph($"Q.No.{questionNumber} Mark the correct answer", 14, TextAlignment.LEFT, true, 0));

            for (int i = 0; i < number; i++)
            {
                q3.Add(markQuestions[i].AnswerText);

                // Generate label (a, b, c, d, ...)
                char label = (char)('a' + i);
                document.Add(CreateParagraph($"{label}) {markQuestions[i].QuestionText}  \n     [ ] True  \n     [ ] False", 12, TextAlignment.LEFT, false, 5));
            }

            // Fetch questions of type "choose" for "Choose the correct word"
            var chooseQuestions = _context.Questions
                .Where(q => q.ChapterId == chapterId && q.Type == "choose")
                .ToList();

            ShuffleArray(chooseQuestions);
            questionNumber++;
            // Add the "Choose the correct word" section
            document.Add(CreateParagraph($"\nQ.No.{questionNumber} Choose the correct word", 14, TextAlignment.LEFT, true, 0));
            for (int i = 0; i < number; i++)
            {
                q4.Add(chooseQuestions[i].AnswerText);

                // Generate label (a, b, c, d, ...)
                char label = (char)('a' + i);
                document.Add(CreateParagraph($"{label}) {chooseQuestions[i].QuestionText}", 12, marginBottom: 10));
            }
        }


        private void AddUnderstandingSection(Document document, int chapterId, int number)
        {

            // Fetch questions of type "blanks" for "Fill in the blanks"
            var blankQuestions = _context.Questions
                .Where(q => q.ChapterId == chapterId && q.Type == "blanks")
                .ToList();
            ShuffleArray(blankQuestions);
            questionNumber++;
            document.Add(CreateParagraph($"Q.No.{questionNumber} Fill in the blanks", 14, TextAlignment.LEFT, true, 0));
            for (int i = 0; i < number; i++)
            {
                q5.Add(blankQuestions[i].AnswerText);

                char label = (char)('a' + i); // Generate label (a, b, c, d, ...)
                document.Add(CreateParagraph($"{label}) {blankQuestions[i].QuestionText}", 12, marginBottom: 10)
                    .SetMultipliedLeading(1.5f));
            }

            // Fetch questions of type "complete" for "Complete the sentence"
            var completeQuestions = _context.Questions
                .Where(q => q.ChapterId == chapterId && q.Type == "complete")
                .ToList();
            ShuffleArray(completeQuestions);
            questionNumber++;
            document.Add(CreateParagraph($"\nQ.No.{questionNumber} Complete the sentence", 14, TextAlignment.LEFT, true, 0));
            for (int i = 0; i < number; i++)
            {
                q6.Add(completeQuestions[i].AnswerText);
                char label = (char)('a' + i); // Generate label (a, b, c, d, ...)
                document.Add(CreateParagraph($"{label}) {completeQuestions[i].QuestionText}", 12)
                    .SetMultipliedLeading(1.5f));
            }
        }

        public void ShuffleArray<T>(List<T> list)
        {
            Random random = new Random();
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                // Swap list[i] and list[j]
                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }
    }

    public class PdfRequestModel
    {
        public bool R1 { get; set; }
        public bool R2 { get; set; }
        public bool R3 { get; set; }
        public int number { get; set; }
        public int chapterId { get; set; }
    }
}
