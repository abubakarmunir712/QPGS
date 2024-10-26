CheckLogin("admin")
document.addEventListener("DOMContentLoaded", function () {
    fetchChapters();
    fetchQuestions(); // Fetch questions when the document is loaded
});

// Fetch all chapters
function fetchChapters() {
    fetch('/api/chapter/all-chapters', {
        headers: {
            Authorization: `Bearer ${localStorage.getItem('token')}`
        }
    })
    .then(response => response.json())
    .then(data => {
        const chapterSelect = document.getElementById("chapterId");
        const chapters = data.$values; // Assuming $values is always present

        // Clear existing options before populating new ones
        chapterSelect.innerHTML = '<option value="">Select Chapter</option>';

        chapters.forEach(chapter => {
            const option = document.createElement("option");
            option.value = chapter.chapterId; // Use chapterId from your structure
            option.textContent = chapter.chapterName; // Use chapterName from your structure
            chapterSelect.appendChild(option);
        });
    })
    .catch(error => console.error('Error fetching chapters:', error));
}

// Fetch all questions
function fetchQuestions() {
    fetch('/api/question/all', {
        headers: {
            Authorization: `Bearer ${localStorage.getItem('token')}`
        }
    })
    .then(response => response.json())
    .then(data => {
        const questionsTableBody = document.getElementById("questionTableBody");
        questionsTableBody.innerHTML = ''; // Clear existing rows

        const questions = data.$values; // Assuming $values contains the questions

        questions.forEach(question => {
            const row = document.createElement("tr");
            row.innerHTML = `
                <td>${question.questionText}</td>
                <td>${question.chapterId}</td>
                <td>${question.type}</td>
                <td>
                    <button onclick="deleteQuestion(${question.questionId})">Delete</button>
                </td>
            `;
            questionsTableBody.appendChild(row);
        });
    })
    .catch(error => {
        console.error('Error fetching questions:', error);
        Toastify({
            text: "Failed to fetch questions. Please try again!",
            duration: 3000,
            gravity: "top",
            position: 'center',
            backgroundColor: "red",
        }).showToast();
    });
}

// Add a new question
function addQuestion() {
    const questionText = document.getElementById("questionText").value;
    const chapterId = document.getElementById("chapterId").value;
    const questionType = document.getElementById("questionType").value;
    let data;

    // Concatenate image URL and question text if type is 'identify'
    if (questionType === "match") {
        const imageUrl = document.getElementById("imageUrl").value;
        data = {
            questionText: `${imageUrl},,${questionText}`,
            chapterId: parseInt(chapterId),
            type: questionType // Change 'Type' to 'type'
        };
    } else {
        data = {
            questionText: questionText,
            chapterId: parseInt(chapterId),
            type: questionType // Change 'Type' to 'type'
        };
    }

    fetch('/api/question/add', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            Authorization:`Bearer ${localStorage.getItem('token')}`
        },
        body: JSON.stringify(data)
    })
    .then(response => {
        if (!response.ok) {
            throw new Error('Network response was not ok');
        }
        return response.json();
    })
    .then(data => {
        console.log('Question added:', data);
        // Optionally, reset the form or provide feedback
        document.getElementById("questionForm").reset();
        toggleImageInput(); // Reset the image input visibility
        fetchQuestions(); // Refresh the questions table after adding a question
    })
    .catch(error => {
        console.error('Error adding question:', error);
        Toastify({
            text: "Error adding question. Please try again!",
            duration: 3000,
            gravity: "top",
            position: 'center',
            backgroundColor: "red",
        }).showToast();
    });
}

// Delete a question by ID
function deleteQuestion(id) {
    fetch(`/api/question/delete/${id}`, {
        method: 'DELETE',
        headers: {
            Authorization: `Bearer ${localStorage.getItem('token')}`
        }
    })
    .then(response => {
        if (!response.ok) {
            throw new Error('Network response was not ok');
        }
        return response.json();
    })
    .then(data => {
        console.log(data.message);
        fetchQuestions(); // Refresh the questions table after deletion
        Toastify({
            text: "Question deleted successfully!",
            duration: 3000,
            gravity: "top",
            position: 'center',
            backgroundColor: "green",
        }).showToast();
    })
    .catch(error => {
        console.error('Error deleting question:', error);
        Toastify({
            text: "Error deleting question. Please try again!",
            duration: 3000,
            gravity: "top",
            position: 'center',
            backgroundColor: "red",
        }).showToast();
    });
}

// Toggle image input visibility based on question type
function toggleImageInput() {
    const questionType = document.getElementById("questionType").value;
    const imageInputContainer = document.getElementById("imageInputContainer");

    // Show image input only for "Match" type
    if (questionType === "match") {
        imageInputContainer.style.display = "block";
    } else {
        imageInputContainer.style.display = "none";
    }
    if (questionType === "identify"){
        const inputBox = document.querySelector('#questionText')
        inputBox.placeholder = "Enter image url"
    }
}
