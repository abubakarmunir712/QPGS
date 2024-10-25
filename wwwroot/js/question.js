CheckLogin("teacher")
const token = localStorage.getItem('token')
document.addEventListener("DOMContentLoaded", function () {
    // Get the generate button
    const generateButton = document.querySelector(".btn-primary");

    // Add event listener for click event on the generate button
    generateButton.addEventListener("click", function () {
        // Get the rubrics data from checkboxes
        const rubric1 = document.getElementById("rubric-1").checked;
        const rubric2 = document.getElementById("rubric-2").checked;
        const rubric3 = document.getElementById("rubric-3").checked;
        const questions = document.querySelector('.questionsNo').value;

        // Get the chapterId from the query parameter
        const urlParams = new URLSearchParams(window.location.search);
        const chapterId = urlParams.get("chapter");

        if (!chapterId) {
            alert("Chapter ID is missing!");
            return;
        }

        // Prepare the data to be sent in the POST request
        const requestData = {
            R1: rubric1,
            R2: rubric2,
            R3: rubric3,
            chapterId: parseInt(chapterId),
            number:parseInt(questions)
        };

        // Make a POST request to /api/pdf/generate
        fetch("/api/pdf/generate", {
            method: "POST",
            headers: {
                "Authorization": `Bearer ${token}`,
                "Content-Type": "application/json"
            },
            body: JSON.stringify(requestData)
        })
        .then(response => {
            if (!response.ok) {
                throw new Error("Failed to generate PDF");
            }
            return response.blob(); // Get the PDF as a blob
        })
        .then(blob => {
            // Create a link element to download the PDF
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement("a");
            a.href = url;
            a.download = "generated-question-paper.pdf"; // Set the file name
            document.body.appendChild(a);
            a.click();
            a.remove();
            window.URL.revokeObjectURL(url); // Clean up the URL object
        })
        .catch(error => {
            console.error("Error:", error);
            alert("An error occurred while generating the PDF.");
        });
    });
});
