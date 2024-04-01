const showPassword = document.getElementById("show-password");
const passwordField = document.getElementById("password");

showPassword.addEventListener("click", function () {
    this.classList.toggle("fa-eye");
    const type = passwordField.getAttribute("type") === "password" ? "text" : "password";
    passwordField.setAttribute("type", type);
})