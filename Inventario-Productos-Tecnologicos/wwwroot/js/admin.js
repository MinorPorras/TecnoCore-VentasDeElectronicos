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
        console.log('Controller:' + controller)
        const values = {};

        form.querySelectorAll('input[name], select[name], textarea[name]').forEach(el => {
            // Ignorar ciertos campos
            if (el.name === 'controller' || el.name === '__RequestVerificationToken') {
                return;
            }

            // Manejar diferentes tipos de inputs
            if (el.type === 'checkbox') {
                values[el.name] = el.checked;
            } else if (el.type === 'radio') {
                if (el.checked) {
                    values[el.name] = el.value === 'true';
                }
            } else {
                // Manejar casos especiales
                switch (el.name) {
                    case 'Id':
                        values[el.name] = parseInt(el.value);
                        break;
                    case 'Activo':
                        console.log('Activo:', el.value);
                        values[el.name] = el.value === "true";
                        break;
                    default:
                        values[el.name] = el.value;
                }
            }
            console.log(`${el.name}: ${values[el.name]}`);
        });

        try {
            console.log('Valores a enviar:', values);
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
                console.log("Error al actualizar:", error);
                alert("Error al actualizar: " + (error.message || JSON.stringify(error)));
            }
        } catch (e) {
            console.log('Error:', e + "Controller: " + controller);
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
