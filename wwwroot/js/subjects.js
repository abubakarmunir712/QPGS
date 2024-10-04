document.addEventListener('DOMContentLoaded', () => {
    // Get the full URL of the current page
    const urlParams = new URLSearchParams(window.location.search);

    // Get the value of the 'class' parameter
    const classId = urlParams.get('class');

    const token = localStorage.getItem('token'); // Retrieve the token from localStorage
    const apiUrl = `http://localhost:5195/api/subject/${classId}`;

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
                localStorage.removeItem('token')
                window.location.href='/'
                return;
            }
            if (!response.ok && response.status !=404) {
                Toastify({
                    text: "Something went wrong. Please try again!",
                    duration: 3000, // Duration in milliseconds
                    gravity: "top", // `top` or `bottom`
                    position: 'center', // `left`, `center`, `right`
                    backgroundColor: "red", // Customize the background color
                }).showToast();
            }
            return response.json();
        })
        .then(data => {
            if (data.$values && data.$values.length > 0) {
                const subjects = data.$values;

                // Check if subjects are present
                if (!subjects || subjects.length === 0) {
                    Toastify({
                        text: "No subjects found for this class",
                        duration: 3000, // Duration in milliseconds
                        gravity: "top", // `top` or `bottom`
                        position: 'center', // `left`, `center`, `right`
                        backgroundColor: "red", // Customize the background color
                    }).showToast();
                } else {
                    // Get the container where all the content will be added
                    const container = document.querySelector('#subjectsContainer');

                    // Add the header section (only once)
                    const headerHTML = `
                        <div class="row">
                            <div class="col-12 text-start">
                                <div class="d-flex align-items-center position-relative top-left-corner">
                                    <img src="/Assets/subjects.png" alt="Subjects Icon" class="subjects-left-icon me-2">
                                    <h2>Subjects</h2>
                                </div>
                            </div>
                        </div>
                    `;
                    container.insertAdjacentHTML('beforeend', headerHTML);

                    // Add the subject boxes section
                    let subjectsHTML = '<div class="row justify-content-start mt-4">';

                    subjects.forEach(subject => {
                        subjectsHTML += `
                            <div class="col-md-3 col-sm-6 mb-4">
                                <div class="subject-box text-center">
                                    <img src="/Assets/subject-icon.png" alt="${subject.subjectName}" class="subject-icon">
                                    <div class="subject-title">${subject.subjectName}</div>
                                </div>
                            </div>
                        `;
                    });

                    subjectsHTML += '</div>';

                    // Append the subjects HTML to the container
                    container.insertAdjacentHTML('beforeend', subjectsHTML);
                }
            } else {
                Toastify({
                    text: "No subjects found for this class",
                    duration: 3000, // Duration in milliseconds
                    gravity: "top", // `top` or `bottom`
                    position: 'center', // `left`, `center`, `right`
                    backgroundColor: "red", // Customize the background color
                }).showToast();
            }
        })
        .catch(error => {
            console.error("Error fetching class data:", error);
        });
});
