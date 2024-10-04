// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function decodeJwt(token) {
    const parts = token.split('.');
    if (parts.length !== 3) {
        return null; // Invalid token
    }

    const payload = parts[1];
    const decodedPayload = atob(payload.replace(/-/g, '+').replace(/_/g, '/'));
    return JSON.parse(decodedPayload);
}

function checkUserRole(isLogin) {
    const token = localStorage.getItem('token');

    if (token) {
        const decoded = decodeJwt(token);

        if (decoded) {
            const role = decoded.role;

            // Redirect based on the role
            if (role === 'admin') {
                window.location.href = '/admin';
            } else if (role === 'teacher') {
                window.location.href = '/teacher';
            } else {
                // Handle unexpected roles or redirect to a default page
                if (isLogin) {
                    window.location.href = '/home/login';
                }
            }
        } else {
            // Token is invalid
            if (isLogin){
            showToastAndRedirect(isLogin);}
        }
    } else {
        if (isLogin){
            showToastAndRedirect(isLogin);}
    }
}

function showToastAndRedirect(isLogin) {
    Toastify({
        text: "You are not logged in!",
        duration: 3000,
        close: true,
        gravity: "top",
        position: 'right',
        backgroundColor: "linear-gradient(to right, #FF5C5C, #FF5C5C)", // Red for error

    }).showToast();

    // Redirect to login after a timeout
    setTimeout(() => {
        if (isLogin) {
            window.location.href = '/home/login';
        }
    }, 3000); // 2 seconds delay
}


document.addEventListener("DOMContentLoaded", function () {
    const token = localStorage.getItem("token");

    // Function to check if the token is expired
    function isTokenExpired(token) {
        if (!token) return true; // If there is no token, treat it as expired
        const decodedToken = decodeJwt(token);
        if (!decodedToken || !decodedToken.exp) return true; // No valid token or expiration not present
        
        const currentTime = Math.floor(Date.now() / 1000); // Current time in seconds
        if(decodedToken.exp < currentTime){
            localStorage.removeItem('token')
            return true
        }
        else{
            return false
        }
    }

    // Reference to the navbar container
    const navbarContainer = document.querySelector(".navbar-nav.flex-grow-1");

    // Clear existing items
    navbarContainer.innerHTML = "";

    if (token && !isTokenExpired(token)) {
        // Token exists and is not expired, show logout link
        navbarContainer.innerHTML = `
            <li class="nav-item">
                <a class="nav-link logout-button" href="#" id="logout-link">Logout</a>
            </li>
        `;

        // Handle logout
        document.getElementById("logout-link").addEventListener("click", function (e) {
            e.preventDefault();
            localStorage.removeItem("token"); // Remove the token from localStorage
            window.location.href = "/Home/Login"; // Redirect to login page
        });
    } else {
        // Token is either not present or expired, show login/register links
        navbarContainer.innerHTML = `
            <li class="nav-item">
                <a class="nav-link text-dark" href="/Home/Login">Login</a>
            </li>
            <li class="nav-item">
                <a class="nav-link text-dark" href="/Home/Register">Register</a>
            </li>
        `;
    
    }
});