document.addEventListener("DOMContentLoaded", function () {
    function fadeOutMessage(id) {
        var message = document.getElementById(id);
        if (message) {
            setTimeout(function () {
                message.classList.add("fade-out");
                setTimeout(() => message.style.display = "none", 500); // Ẩn sau khi fade out
            }, 3000); // Sau 3 giây
        }
    }

    fadeOutMessage("success-message");
    fadeOutMessage("error-message");
});
