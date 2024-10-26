document.getElementById('register-form').addEventListener('submit', async function (event) {
    event.preventDefault(); // Prevent the default form submission

    // Get input values
    const username = document.getElementById('reg-username').value;
    const cnic = document.getElementById('reg-cnic').value;
    const cellphone = document.getElementById('cellphone').value;
    const password = document.getElementById('reg-password').value;
    const confirmPassword = document.getElementById('reg-conf-pass').value; // Get the confirm password value
    const userType = document.querySelector('input[name="user-type"]:checked'); // Get the selected user type
    const role = userType ? userType.value : 'teacher'; // Default to 'user' if not selected

    // Check for empty fields
    if (!username || !cnic || !cellphone || !password || !confirmPassword || !userType) {
        Toastify({
            text: "Please fill in all required fields!",
            duration: 3000,
            close: true,
            gravity: "top",
            position: 'right',
            backgroundColor: "linear-gradient(to right, #FF0000, #FF0010)", // Red error color
        }).showToast();
        return; // Stop form submission
    }

    // Check if passwords match
    if (password !== confirmPassword) {
        Toastify({
            text: "Passwords do not match!",
            duration: 3000,
            close: true,
            gravity: "top",
            position: 'right',
            backgroundColor: "linear-gradient(to right, #FF0000, #FF0010)", // Red error color
        }).showToast();
        return; // Stop form submission
    }

    // Create the request body
    const requestBody = {
        username: username,
        password: password,
        CNIC: cnic,
        role: role,
        cellphone: cellphone,
    };

    try {
        const response = await fetch('/api/auth/signup', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(requestBody),
        });

        const data = await response.json(); // Parse JSON response

        if (response.ok) {
            // If the request was successful, show success toast
            Toastify({
                text: data.message,
                duration: 3000,
                close: true,
                gravity: "top",
                position: 'right',
                backgroundColor: "linear-gradient(to right, #00b09b, #96c93d)", // Green success color
            }).showToast();
            // Reload the window after a short delay
            setTimeout(() => {
                window.location.href = '/home/login';
            }, 1000); // 1 second delay to show the toast
        } else {
            // If there was an error, show the error toast
            Toastify({
                text: data.error,
                duration: 3000,
                close: true,
                gravity: "top",
                position: 'right',
                backgroundColor: "linear-gradient(to right, #FF0000, #FF0010)", // Red error color
            }).showToast();
        }
    } catch (error) {
        console.error('Error:', error);
        // Handle any network errors or unexpected exceptions
        Toastify({
            text: "An unexpected error occurred!",
            duration: 3000,
            close: true,
            gravity: "top",
            position: 'right',
            backgroundColor: "linear-gradient(to right, #FF0000, #FF0010)", // Red error color
        }).showToast();
    }
});

document.getElementById("reg-cnic").addEventListener("input", function () {
    let value = this.value.replace(/\D/g, ""); // Remove non-digit characters
    if (value.length > 13) {
        value = value.slice(0, 13);
    }
    if (value.length > 5) {
        value = value.slice(0, 5) + "-" + value.slice(5);
    }
    if (value.length > 13) {
        value = value.slice(0, 13) + "-" + value.slice(13);
    }
    this.value = value;
});
