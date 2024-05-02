window.addEventListener("load", () => {
    const inputImage = document.getElementById("img-file");
    const imageView = document.getElementById("photo");

    inputImage.addEventListener("change", () => {
        const file = inputImage.files[0];
        if (file) {
            const reader = new FileReader();
            reader.onload = function (event) {
                imageView.src = event.target.result;
            };
            reader.readAsDataURL(file);
        } else {
            console.error("No file selected.");
        }
    });
});