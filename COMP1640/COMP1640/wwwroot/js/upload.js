window.addEventListener("load", () => {
    const inputImage = document.getElementById("image-upload");
    const imageView = document.getElementById("img-view");
    const inputFile = document.getElementById("file-upload");
    const fileWrapper = document.getElementById("file-wrapper");
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

    inputImage.addEventListener("change", () => {
        let imgLink = URL.createObjectURL(inputImage.files[0]);
        imageView.style.backgroundImage = `url(${imgLink})`;
        imageView.textContent = "";
        imageView.style.height = "300px";
        imageView.style.border = "1px solid #00000033";
    })

    inputFile.addEventListener("change", (e) => {
        fileWrapper.textContent = "";
        let fileName = e.target.files[0].name;
        let fileType = e.target.value.split(".").pop();
        fileShow(fileName, fileType);
    })

    const fileShow = (fileName, fileType) => {
        const showFileBoxElement = document.createElement("div");
        showFileBoxElement.classList.add("show-file-box");
        const leftElement = document.createElement("div");
        leftElement.classList.add("left");
        const fileTypeElement = document.createElement("span");
        fileTypeElement.classList.add("file-type");
        fileTypeElement.innerHTML = fileType;
        leftElement.append(fileTypeElement);
        const fileTitleElement = document.createElement("h3");
        fileTitleElement.innerHTML = fileName;
        leftElement.append(fileTitleElement);
        showFileBoxElement.append(leftElement);
        const rightElement = document.createElement("div");
        rightElement.classList.add("right");
        showFileBoxElement.append(rightElement);
        const crossElement = document.createElement("span");
        crossElement.innerHTML = "&check;";
        rightElement.append(crossElement);
        fileWrapper.append(showFileBoxElement);
    }
});