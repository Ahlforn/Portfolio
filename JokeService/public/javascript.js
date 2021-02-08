let dropdown = document.getElementById("serviceSelect")
let currentService;

dropdown.addEventListener("change", dropdownFunction())

function dropdownFunction() {
    currentService.classList.remove("showing")
    currentService = document.getElementById(dropdown.value)
    currentService.classList.add("showing")
    if(dropdown.value == ourService._id){
        enableInputs();
    }
}

let setup = document.getElementById("setupInput")
let punchline = document.getElementById("punchlineInput")
let btn = document.getElementById("btn")

function enableInputs() {
    setup.disabled = false;
    punchline.disabled = false;
    btn.disabled = false;
}

btn.addEventListener("click")