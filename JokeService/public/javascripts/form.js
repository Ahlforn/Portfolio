const headers = { 'Content-Type': 'application/json; charset=utf-8' };
let id = document.getElementById("id");
let setup = document.getElementById("setup");
let punchline = document.getElementById("punchline");
let btnSave = document.getElementById("save");
let btnDelete = document.getElementById("delete");
let btnCreate = document.getElementById("create");

if(id) {
    btnSave.addEventListener('click', (event) => {
        event.preventDefault();

        let data = buildData();
        data.id = id.value;

        fetch('/api/jokes', {
            method: 'PUT',
            headers: headers,
            body: JSON.stringify(data)
        }).then((res) => {
            document.location.href = '/';
        });
    });

    btnDelete.addEventListener('click', (event) => {
        event.preventDefault();

        fetch('/api/jokes/' + id.value, {
            method: 'DELETE'
        }).then((res) => {
            document.location.href = '/';
        });
    });
} else {
    btnCreate.addEventListener('click', (event) => {
        event.preventDefault();

        let data = buildData();

        fetch('/api/jokes', {
            method: 'POST',
            headers: headers,
            body: JSON.stringify(data)
        }).then((res) => {
            document.location.href = '/';
        })
    });
}

function buildData() {
    return {
        setup: setup.value,
        punchline: punchline.value
    }
}