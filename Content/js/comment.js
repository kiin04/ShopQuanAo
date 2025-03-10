function submitComment() {
    var commentText = document.getElementById("commentText").value.trim();
    var productId = document.getElementById("commentText").getAttribute("data-product-id");

    if (commentText === "") {
        alert("Vui lòng nhập nội dung bình luận.");
        return;
    }

    fetch(commentUrl, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'X-Requested-With': 'XMLHttpRequest'
        },
        body: JSON.stringify({
            productId: productId,
            commentText: commentText
        })
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                var commentSection = document.getElementById("comments");
                var newComment = document.createElement("div");
                newComment.classList.add("comment");

                // Định dạng lại ngày nếu server trả về null hoặc sai định dạng
                let createdAtFormatted = data.createdAt || new Date().toLocaleString('vi-VN', { hour12: false });

                newComment.innerHTML = `
                <p><strong>${data.customerName}:</strong> ${commentText}</p>
                <small>${createdAtFormatted}</small>
            `;
                commentSection.prepend(newComment);

                document.getElementById("commentText").value = "";

                var noCommentsMsg = document.getElementById("no-comments");
                if (noCommentsMsg) noCommentsMsg.style.display = "none";
            } else {
                alert(data.message);
            }
        })
        .catch(error => console.error('Lỗi:', error));
}
