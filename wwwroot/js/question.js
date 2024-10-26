CheckLogin("teacher");
const token = localStorage.getItem('token');

document.addEventListener("DOMContentLoaded", function () {
    const generateButton = document.querySelector(".btn-primary");

    generateButton.addEventListener("click", function () {
        const rubric1 = document.getElementById("rubric-1").checked;
        const rubric2 = document.getElementById("rubric-2").checked;
        const rubric3 = document.getElementById("rubric-3").checked;
        const questions = document.querySelector('.questionsNo').value;

        const urlParams = new URLSearchParams(window.location.search);
        const chapterId = urlParams.get("chapter");

        if (!chapterId) {
            alert("Chapter ID is missing!");
            return;
        }

        const requestData = {
            R1: rubric1,
            R2: rubric2,
            R3: rubric3,
            chapterId: parseInt(chapterId),
            number: parseInt(questions)
        };

        fetch("/api/pdf/generate", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${token}`
            },
            body: JSON.stringify(requestData)
        })
        .then(response => {
            if (!response.ok) {
                throw new Error("Network response was not ok");
            }
            return response.blob();
        })
        .then(blob => {
            const blobUrl = window.URL.createObjectURL(blob);
            window.open(blobUrl); // Opens PDF in a new tab
        })
        .catch(error => console.error("Error generating PDF:", error));
        
    });
});
