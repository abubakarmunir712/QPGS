document.addEventListener("DOMContentLoaded", () => {
    // Add event listener to the form\
    fetchTeachers();
    fetchClasses();
    document.getElementById("classForm").addEventListener("submit", function(event) {
        event.preventDefault();  // Prevent form submission to reload the page
        addClass();  // Call the addClass function
    });
});

// Function to handle the add class action
function addClass() {
    const classData = {
        className: document.getElementById("className").value,
        classDescription: document.getElementById("classDescription").value,
        adminId: parseInt(document.getElementById("adminId").value)
    };

    fetch('/api/class/add', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization':`Bearer ${localStorage.getItem('token')}`
        },
        body: JSON.stringify(classData)
    })
    .then(response => {
        if (!response.ok) {
            Toastify({
                text: "Something went wrong. Please try again!",
                duration: 3000,
                gravity: "top",
                position: 'center',
                backgroundColor: "red",
            }).showToast();
            return Promise.reject("Failed to add class");
        }
        return response.json();
    })
    .then(data => {
        // Show success message
        Toastify({
            text: "Class added successfully!",
            duration: 3000,
            gravity: "top",
            position: 'center',
            backgroundColor: "green",
        }).showToast();

        // Optionally, update UI or table with new data
        fetchClasses();
        document.getElementById("classForm").reset();
    })
    .catch(error => {
        console.error("Error:", error);
    });
}

function fetchTeachers() {
    fetch('/api/auth/teachers')
        .then(response => {
            if (!response.ok) {
                throw new Error("Failed to fetch teachers.");
            }
            return response.json();
        })
        .then(data => {
            const adminSelect = document.getElementById("adminId");
            data.$values.forEach(teacher => {
                const option = document.createElement("option");
                option.value = teacher.userId;
                option.textContent = `${teacher.username}`;
                adminSelect.appendChild(option);
            });
        })
        .catch(error => {
            console.error("Error:", error);
        });
}

function fetchClasses() {
    fetch('/api/class/all-classes')
        .then(response => {
            if (!response.ok) {
                throw new Error("Failed to fetch classes.");
            }
            return response.json();
        })
        .then(data => {
            const classTableBody = document.getElementById("classTableBody");
            classTableBody.innerHTML = ""; // Clear existing rows

            data.$values.forEach(classItem => {
                const row = document.createElement("tr");

                // Create and append Class Name cell
                const classNameCell = document.createElement("td");
                classNameCell.textContent = classItem.className;
                row.appendChild(classNameCell);

                // Create and append Class Description cell
                const classDescriptionCell = document.createElement("td");
                classDescriptionCell.textContent = classItem.classDescription;
                row.appendChild(classDescriptionCell);

                // Create and append Admin ID cell
                const adminIdCell = document.createElement("td");
                adminIdCell.textContent = classItem.adminId;
                row.appendChild(adminIdCell);

                // Create and append Actions cell with Delete button
                const actionsCell = document.createElement("td");
                const deleteButton = document.createElement("button");
                deleteButton.textContent = "Delete";
                deleteButton.classList.add("delete-button");
                deleteButton.onclick = () => deleteClass(classItem.classId); // Call delete function
                actionsCell.appendChild(deleteButton);
                row.appendChild(actionsCell);

                // Append the row to the table body
                classTableBody.appendChild(row);
            });
        })
        .catch(error => {
            console.error("Error:", error);
        });
}

function deleteClass(classId) {
    // Example delete function
    fetch(`/api/class/delete/${classId}`, {
        method: "DELETE",
        headers: {
            "Authorization": `Bearer ${localStorage.getItem('token')}`,
        }
    })
        .then(response => {
            if (response.ok) {
                Toastify({
                    text: "Class deleted successfully!",
                    duration: 3000,
                    gravity: "top",
                    position: 'center',
                    backgroundColor: "green",
                }).showToast();
                fetchClasses(); // Refresh the table
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