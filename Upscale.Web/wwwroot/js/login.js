let currentLimit = 8;

function switchDocument(type, limit, element) {
    currentLimit = limit;
    const input = document.getElementById('userInput');
    input.maxLength = limit;
    input.value = "";

    document.querySelectorAll('.nav-link').forEach(btn => btn.classList.remove('active'));
    element.classList.add('active');

    document.getElementById('userError').classList.add('d-none');
}

function isNumber(evt) {
    const charCode = (evt.which) ? evt.which : evt.keyCode;
    return !(charCode > 31 && (charCode < 48 || charCode > 57));
}

function validateSubmit(e) {
    const user = document.getElementById('userInput').value;
    const pass = document.getElementById('passInput').value;
    const userError = document.getElementById('userError');
    const passError = document.getElementById('passError');
    let isValid = true;

    if (user.trim() === "" || user.length < currentLimit) {
        userError.classList.remove('d-none');
        isValid = false;
    } else {
        userError.classList.add('d-none');
    }

    if (pass.trim() === "") {
        passError.classList.remove('d-none');
        isValid = false;
    } else {
        passError.classList.add('d-none');
    }

    if (!isValid) e.preventDefault();
    return isValid;
}

function togglePass() {
    const passInput = document.getElementById('passInput');
    const eyeIcon = document.getElementById('eyeIcon');
    const isPass = passInput.type === "password";
    passInput.type = isPass ? "text" : "password";
    eyeIcon.classList.toggle('fa-eye');
    eyeIcon.classList.toggle('fa-eye-slash');
}