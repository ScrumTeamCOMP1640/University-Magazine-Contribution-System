window.addEventListener("load", () => {
    const inputImg = document.getElementById("img-file");
    const imgView = document.getElementById("photo");

    inputImg.addEventListener("change", () => {
        const file = inputImg.files[0];
        if (file) {
            const reader = new FileReader();
            reader.onload = function (event) {
                imgView.src = event.target.result;
            };
            reader.readAsDataURL(file);
        } else {
            console.error("No file selected.");
        }
    });
});