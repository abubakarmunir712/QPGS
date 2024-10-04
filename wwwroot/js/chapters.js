CheckLogin("teacher")
document.addEventListener('DOMContentLoaded', () => {
    // Get the full URL of the current page
    const urlParams = new URLSearchParams(window.location.search);

    // Get the value of the 'subject' parameter
    const subjectId = urlParams.get('subject');

    if (!subjectId) {
        console.error("Subject ID not found in the URL parameters.");
        return;
    }

    const token = localStorage.getItem('token'); // Retrieve the token from localStorage
    const apiUrl = `http://localhost:5195/api/chapter/subject/${subjectId}`;

    // Fetch chapter data from the API
    fetch(apiUrl, {
        method: "GET",
        headers: {
            "Authorization": `Bearer ${token}`, // Include the token in the Authorization header
            "Content-Type": "application/json"
        }
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
                return Promise.reject("Failed to fetch chapters");
            }
            return response.json();
        })
        .then(data => {
            if (data.$values && data.$values.length > 0) {
                const chapters = data.$values;

                // Get the container where all the chapter elements will be added
                const chapterListContainer = document.querySelector('.chapter-list');

                if (!chapterListContainer) {
                    console.error("Chapter list container not found in the DOM.");
                    return;
                }

                // Create and append chapter elements to the container
                chapters.forEach((chapter, index) => {
                    const chapterHTML = `
                        <div class="col-md-6 col-lg-6 mb-3">
                            <div class="chapter-box d-flex align-items-center p-3 shadow-sm" data-id="${chapter.chapterId}">
                                <div class="chapter-icon me-2">
                                    <i class="fas fa-minus-circle"></i>
                                </div>
                                <div class="chapter-title">
                                    Chapter-${index + 1}: ${chapter.chapterName}
                                </div>
                            </div>
                        </div>
                    `;

                    chapterListContainer.insertAdjacentHTML('beforeend', chapterHTML);
                });
            } else {
                Toastify({
                    text: "No chapters found for this subject",
                    duration: 3000,
                    gravity: "top",
                    position: 'center',
                    backgroundColor: "red",
                }).showToast();
            }
        })
        .catch(error => {
            console.error("Error fetching chapter data:", error);
        });
});
