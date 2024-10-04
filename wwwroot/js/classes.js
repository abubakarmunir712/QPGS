document.addEventListener('DOMContentLoaded', function () {
    const token = localStorage.getItem('token'); // Retrieve the token from localStorage
    const apiUrl = "/api/class/my-classes"; // Your API endpoint

    // Fetch class data from the API
    fetch(apiUrl, {
        method: "GET",
        headers: {
            "Authorization": `Bearer ${token}`, // Include the token in the Authorization header
            "Content-Type": "application/json"
        }
    })
        .then(response => {
            if (response.status === 401) {
                // Handle unauthorized access
                showToastAndRedirect("Unauthorized access. Please log in again.");
                localStorage.removeItem('token')
                return;
            }

            if (!response.ok) {
                throw new Error('Network response was not ok ' + response.statusText);
            }
            return response.json();
        })
        .then(data => {
            if (data.$values) {
                const classData = data.$values;

                // Get the parent element for class boxes
                const classContainer = document.querySelector('.classes-list');
                classContainer.innerHTML = ''; // Clear previous content

                // Iterate over the class data and create class boxes
                classData.forEach(classItem => {
                    // Create a new div for each class
                    const classBoxDiv = document.createElement('div');
                    classBoxDiv.classList.add('col-md-4', 'col-sm-6', 'mb-4'); // Bootstrap classes for responsiveness

                    classBoxDiv.innerHTML = `
                    <div class="class-box text-center" data-id="${classItem.classId}" onclick="gotoClass(${classItem.classId})">
                    <img src="/Assets/class-icon.png" alt="${classItem.className}" class="class-icon">
                    <div class="class-title">${classItem.className}</div>
                    <div class="class-description">${classItem.classDescription}</div> <!-- Optional: Add description if needed -->
                    </div>
                    `;

                    // Append the new class box to the class container
                    classContainer.appendChild(classBoxDiv);
                });
            } else {
                console.error("No classes found in the response.");
            }
        })
        .catch(error => {
            console.error("Error fetching class data:", error);
        });

    function showToastAndRedirect(message) {
        // Show the Toastify alert
        Toastify({
            text: message,
            duration: 3000, // Duration of the toast in milliseconds
            gravity: "top", // `top` or `bottom`
            position: 'center', // `left`, `center` or `right`
            backgroundColor: "red", // Set a background color
            stopOnFocus: true, // Prevents dismissing of toast on hover
        }).showToast();

        // Redirect to the login page after 3 seconds
        setTimeout(() => {
            window.location.href = "/"; // Adjust the path to your login page
        }, 3000);
    }
});

function gotoClass(classId) {
    window.location.href = `/Teacher/Subjects?class=${classId}`
}
