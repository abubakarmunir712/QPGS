// Call the function on page load
document.addEventListener('DOMContentLoaded', checkUserRole(false));

document.getElementById("login-cnic").addEventListener("input", function () {
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

document.getElementById('login-form').addEventListener('submit', async function (event) {
    event.preventDefault(); // Prevent the default form submission

    const cnic = document.getElementById('login-cnic').value;
    const password = document.getElementById('login-password').value;
    const remember = document.getElementById('remember').checked;

    // Create the request body
    const requestBody = {
        CNIC: cnic,
        Password: password,
        Remember: remember,
    };

    try {
        const response = await fetch('/api/auth/login', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(requestBody),
        });

        const data = await response.json(); // Parse JSON response

        if (response.ok) {
            // If the request was successful, set the token in local storage
            localStorage.setItem('token', data.token);
            // Show success toast
            Toastify({
                text: data.message || "Login Successful!",
                duration: 3000,
                close: true,
                gravity: "top",
                position: 'right',
                backgroundColor: "linear-gradient(to right, #00b09b, #96c93d)", // Green success color
            }).showToast();
            setInterval(()=>{
                window.location.reload();
            },3000)

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
            background: "linear-gradient(to right, #FF0000, #FF0010)", // Red error color
        }).showToast();
    }
});
