document.addEventListener("DOMContentLoaded", function () {
    if (document.querySelector('.modifyElement')) {
        modifyElement();
    }
    if (document.querySelector('.deleteDialog')) {
        deleteElement()
    }
})

function modifyElement() {
    const updateBtn = document.getElementById('updateBtn')
    updateBtn.addEventListener('click', async (e) => {
        e.preventDefault()
        const form = document.querySelector('.modifyElement');
        const controller = document.getElementById('controller').value
        console.log(controller)
        const values = {};

        form.querySelectorAll('input[name], select[name], textarea[name]').forEach(el => {
            switch (el.type) {
                case 'checkbox':
                    values[el.name] = el.checked;
                    break;
                case 'radio':
                    if (el.checked) values[el.name] = el.value;
                    break;
            }
            switch (el.name) {
                case 'controller':
                    return;
                case '__RequestVerificationToken':
                    return;
                case 'Id':
                    values[el.name] = parseInt(el.value);
                    break;
                case 'Activo':
                    values[el.name] = el.value === "true";
                    break;
                default:
                    values[el.name] = el.value;
            }
            console.log(el.name);
        });
        try {
            console.log(values);
            const bodyRequest = JSON.stringify(values);
            const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
            const response = await fetch(`/${controller}/Edit`, {
                method: "PUT",
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': token
                },
                body: bodyRequest
            })
            if (response.ok) {
                window.location.href = `/${controller}/Index`
            } else {
                const text = await response.text();
                let error;
                try {
                    error = text ? JSON.parse(text) : {message: "Error desconocido"};
                } catch {
                    error = {message: text || "Error desconocido"};
                }
                console.error("Error al actualizar" + error);
                alert("Error al actualizar: " + (error.message || JSON.stringify(error)));
            }
        } catch (e) {
            console.error('Error:', e);
            alert('Error al procesar la solicitud');
        }
    })
}

function deleteElement() {
    console.log("Entra aquí")
    let showModalBtn = document.querySelectorAll('.showModal');
    let deleteDialog = document.querySelector('.deleteDialog');
    let pageTitle = document.querySelector('.pageTitle').innerText;
    const btnCancel = document.querySelector('.btnCancel');

    showModalBtn.forEach(btn => {
        btn.addEventListener('click', () => {
            console.log("Entra aquí");
            const id = btn.getAttribute('data-id');
            const name = btn.getAttribute('value');
            document.getElementById('Id').value = id;
            deleteDialog.firstElementChild.innerHTML = `Desea eliminar el ${pageTitle.toLowerCase()} : ${name}?`;
            deleteDialog.showModal()
        })
    })

    function closeModal() {
        deleteDialog.close();
    }

    console.log("Asigna el evento")
    btnCancel.addEventListener('click', closeModal);
    deleteDialog.addEventListener('click', (event) => {
        if (event.target === deleteDialog) {
            deleteDialog.close();
        }
    });
}


