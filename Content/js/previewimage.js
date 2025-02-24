document.addEventListener("DOMContentLoaded", function () {
    function previewImage(event) {
        var input = event.target;
        var imgElement = null;

        // Xác định đúng thẻ ảnh cần hiển thị
        if (input.id === "avatarFile") {
            imgElement = document.getElementById("avatarPreview");
        } else {
            imgElement = document.getElementById("imagePreview");
        }

        if (input.files && input.files[0] && imgElement) {
            var reader = new FileReader();
            reader.onload = function () {
                imgElement.src = reader.result;
                imgElement.style.display = "block"; // Hiển thị ảnh
            };
            reader.readAsDataURL(input.files[0]);
        }
    }

    // Lắng nghe sự kiện trên tất cả các input file
    var fileInputs = document.querySelectorAll("#imageFile, #ImageUpload, #avatarFile");
    fileInputs.forEach(function (input) {
        input.addEventListener("change", previewImage);
    });
});
