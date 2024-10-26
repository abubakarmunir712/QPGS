CheckLogin("admin")
document.addEventListener("DOMContentLoaded", () => {
    // Fetch all subjects and chapters on page load
    fetchSubjects();
    fetchChapters();

    document.getElementById("chapterForm").addEventListener("submit", function (event) {
        event.preventDefault(); // Prevent form submission to reload the page
        addChapter(); // Call the addChapter function
    });
});

// Function to handle the add chapter action
function addChapter() {
    const chapterData = {
        ChapterName: document.getElementById("chapterName").value,
        SubjectId: parseInt(document.getElementById("subjectId").value)
    };

    fetch('/api/chapter/add', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${localStorage.getItem('token')}`
        },
        body: JSON.stringify(chapterData)
    })
        .then(response => {
            if (!response.ok) {
                // Handle errors based on response status
                Toastify({
                    text: "Something went wrong. Please try again!",
                    duration: 3000,
                    gravity: "top",
                    position: 'center',
                    backgroundColor: "red",
                }).showToast();
                return Promise.reject("Failed to add chapter");
            }
            return response.json();
        })
        .then(data => {
            // Show success message
            Toastify({
                text: "Chapter added successfully!",
                duration: 3000,
                gravity: "top",
                position: 'center',
                backgroundColor: "green",
            }).showToast();

            // Optionally, update UI or table with new data
            fetchChapters();
            document.getElementById("chapterForm").reset();
        })
        .catch(error => {
            console.error("Error:", error);
        });
}

function fetchSubjects() {
    fetch('/api/subject/all-subjects', {
        method: 'GET',
        headers: {
            Authorization: `Bearer ${localStorage.getItem('token')}`
        }
    })
        .then(response => {
            if (!response.ok) {
                throw new Error("Failed to fetch subjects.");
            }
            return response.json();
        })
        .then(data => {
            const subjectSelect = document.getElementById("subjectId");
            subjectSelect.innerHTML = ""; // Clear existing options

            data.$values.forEach(subjectItem => {
                const option = document.createElement("option");
                option.value = subjectItem.subjectId;
                option.textContent = subjectItem.subjectName;
                subjectSelect.appendChild(option);
            });
        })
        .catch(error => {
            console.error("Error:", error);
        });
}

function fetchChapters() {
    fetch('/api/chapter/all-chapters', {
        method: 'GET',
        headers: {
            Authorization: `Bearer ${localStorage.getItem('token')}`
        }
    })
        .then(response => {
            if(response.status == 404){
                document.getElementById("chapterTableBody").innerHTML = ''
            }
            if (!response.ok) {
                throw new Error("Failed to fetch chapters.");
            }
            return response.json();
        })
        .then(data => {
            const chapterTableBody = document.getElementById("chapterTableBody");
            chapterTableBody.innerHTML = ""; // Clear existing rows

            data.$values.forEach(chapterItem => {
                const row = document.createElement("tr");

                // Create and append Chapter Name cell
                const chapterNameCell = document.createElement("td");
                chapterNameCell.textContent = chapterItem.chapterName;
                row.appendChild(chapterNameCell);

                // Create and append Subject ID cell
                const subjectIdCell = document.createElement("td");
                subjectIdCell.textContent = chapterItem.subjectId;
                row.appendChild(subjectIdCell);

                // Create and append Actions cell with Delete button
                const actionsCell = document.createElement("td");
                const deleteButton = document.createElement("button");
                deleteButton.textContent = "Delete";
                deleteButton.classList.add("delete-button");
                deleteButton.onclick = () => deleteChapter(chapterItem.chapterId); // Call delete function
                actionsCell.appendChild(deleteButton);
                row.appendChild(actionsCell);

                // Append the row to the table body
                chapterTableBody.appendChild(row);
            });
        })
        .catch(error => {
            console.error("Error:", error);
        });
}

function deleteChapter(chapterId) {
    fetch(`/api/chapter/delete/${chapterId}`, {
        method: "DELETE",
        headers: {
            "Authorization": `Bearer ${localStorage.getItem('token')}`,
        }
    })
        .then(response => {
            if (response.ok) {
                Toastify({
                    text: "Chapter deleted successfully!",
                    duration: 3000,
                    gravity: "top",
                    position: 'center',
                    backgroundColor: "green",
                }).showToast();
                fetchChapters(); // Refresh the table
            } else {
                Toastify({
                    text: "Something went wrong!",
                    duration: 3000,
                    gravity: "top",
                    position: 'center',
                    backgroundColor: "red",
                }).showToast();
            }
        })
        .catch(error => {
            Toastify({
                text: "Something went wrong!",
                duration: 3000,
                gravity: "top",
                position: 'center',
                backgroundColor: "red",
            }).showToast();
        });
}
