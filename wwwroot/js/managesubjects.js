CheckLogin("admin");
document.addEventListener("DOMContentLoaded", () => {
    // Fetch all classes and subjects on page load
    fetchClasses();
    fetchSubjects();

    document.getElementById("subjectForm").addEventListener("submit", function (event) {
        event.preventDefault();  // Prevent form submission to reload the page
        addSubject();  // Call the addSubject function
    });
});

// Function to handle the add subject action
function addSubject() {
    const subjectData = {
        SubjectName: document.getElementById("subjectName").value,
        ClassId: parseInt(document.getElementById("classId").value)
    };

    fetch('/api/subject/add', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${localStorage.getItem('token')}`
        },
        body: JSON.stringify(subjectData)
    })
        .then(response => {
            if (!response.ok) {
                if (response.status === 404) {
                    Toastify({
                        text: "Class not found!",
                        duration: 3000,
                        gravity: "top",
                        position: 'center',
                        backgroundColor: "orange",
                    }).showToast();
                } else {
                    Toastify({
                        text: "Something went wrong. Please try again!",
                        duration: 3000,
                        gravity: "top",
                        position: 'center',
                        backgroundColor: "red",
                    }).showToast();
                }
                return Promise.reject("Failed to add subject");
            }
            return response.json();
        })
        .then(data => {
            // Show success message
            Toastify({
                text: "Subject added successfully!",
                duration: 3000,
                gravity: "top",
                position: 'center',
                backgroundColor: "green",
            }).showToast();

            // Optionally, update UI or table with new data
            fetchSubjects();
            document.getElementById("subjectForm").reset();
        })
        .catch(error => {
            console.error("Error:", error);
        });
}

function fetchClasses() {
    fetch('/api/class/all-classes', {
        method: 'GET',
        headers: {
            Authorization: `Bearer ${localStorage.getItem('token')}`
        }
    })
        .then(response => {
            if (!response.ok) {
                throw new Error("Failed to fetch classes.");
            }
            return response.json();
        })
        .then(data => {
            const classSelect = document.getElementById("classId");
            classSelect.innerHTML = ""; // Clear existing options

            data.$values.forEach(classItem => {
                const option = document.createElement("option");
                option.value = classItem.classId;
                option.textContent = classItem.className;
                classSelect.appendChild(option);
            });
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
            if (response.status == 404){
                document.getElementById("subjectTableBody").innerHTML = "";
            }
            if (!response.ok) {
                throw new Error("Failed to fetch subjects.");
            }
            return response.json();
        })
        .then(data => {
            const subjectTableBody = document.getElementById("subjectTableBody");
            subjectTableBody.innerHTML = ""; // Clear existing rows

            data.$values.forEach(subjectItem => {
                const row = document.createElement("tr");

                // Create and append Subject ID cell
                const subjectIdCell = document.createElement("td");
                subjectIdCell.textContent = subjectItem.subjectId;
                row.appendChild(subjectIdCell);

                // Create and append Subject Name cell
                const subjectNameCell = document.createElement("td");
                subjectNameCell.textContent = subjectItem.subjectName;
                row.appendChild(subjectNameCell);

                // Create and append Class ID cell
                const classIdCell = document.createElement("td");
                classIdCell.textContent = subjectItem.classId;
                row.appendChild(classIdCell);

                // Create and append Actions cell with Delete button
                const actionsCell = document.createElement("td");
                const deleteButton = document.createElement("button");
                deleteButton.textContent = "Delete";
                deleteButton.classList.add("delete-button");
                deleteButton.onclick = () => deleteSubject(subjectItem.subjectId); // Call delete function
                actionsCell.appendChild(deleteButton);
                row.appendChild(actionsCell);

                // Append the row to the table body
                subjectTableBody.appendChild(row);
            });
        })
        .catch(error => {
            console.error("Error:", error);
        });
}

function deleteSubject(subjectId) {
    fetch(`/api/subject/delete/${subjectId}`, {
        method: "DELETE",
        headers: {
            "Authorization": `Bearer ${localStorage.getItem('token')}`,
        }
    })
        .then(response => {
            if (response.ok) {
                Toastify({
                    text: "Subject deleted successfully!",
                    duration: 3000,
                    gravity: "top",
                    position: 'center',
                    backgroundColor: "green",
                }).showToast();
                fetchSubjects(); // Refresh the table
            } else {
                if (response.status === 404) {
                    Toastify({
                        text: "Subject not found!",
                        duration: 3000,
                        gravity: "top",
                        position: 'center',
                        backgroundColor: "orange",
                    }).showToast();
                } else {
                    Toastify({
                        text: "Something went wrong!",
                        duration: 3000,
                        gravity: "top",
                        position: 'center',
                        backgroundColor: "red",
                    }).showToast();
                }
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
