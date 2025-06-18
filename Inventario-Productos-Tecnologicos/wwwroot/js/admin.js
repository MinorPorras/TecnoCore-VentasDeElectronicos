document.addEventListener("DOMContentLoaded", function () {
    if (document.querySelector('.modifyElement')) {
        modifyElement();
        if(document.querySelector('#imgSelector')) {
            let imgForm = document.querySelector('#imgSelector');
            imgForm.addEventListener('change', mostrarImagen(imgForm));
        }
    }
    if (document.querySelector('.deleteDialog')) {
        deleteElement()
    }
})

function mostrarImagen(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function(e) {
            var preview = document.getElementById('preview');
            preview.src = e.target.result;
            preview.style.display = 'block';
        }

        reader.readAsDataURL(input.files[0]);
    }
}

function modifyElement() {
    const updateBtn = document.getElementById('updateBtn')
    updateBtn.addEventListener('click', async (e) => {
        e.preventDefault()
        const form = document.querySelector('.modifyElement');
        const controller = document.getElementById('controller').value
        const action = document.getElementById('action').value
        console.log('Controller:' + controller)
        const values = {};

        form.querySelectorAll('input[name], select[name], textarea[name]').forEach(el => {
            // Ignorar ciertos campos
            if (el.name === 'controller' || el.name === '__RequestVerificationToken' || el.name === 'action') {
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
                    case 'CategoriaId':
                    case 'MarcaId':
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
            const response = await fetch(`/${controller}/${action}`, {
                method: "PUT",
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': token
                },
                body: bodyRequest
            })
            if (response.ok) {
                // Redirigir según la acción realizada
                if (action === 'EditSubcategoria') {
                    // Redirigir a la página de edición de la categoría padre
                    window.location.href = `/${controller}/Edit/${values.CategoriaId}`;
                } else {
                    window.location.href = `/${controller}/Index`;
                }
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
    const showModalBtns = document.querySelectorAll('.showModal');
    const deleteDialog = document.querySelector('.deleteDialog');
    const pageTitle = document.querySelector('.pageTitle');
    const btnCancel = document.querySelector('.btnCancel');

    if (!deleteDialog || !showModalBtns.length) return;

    showModalBtns.forEach(btn => {
        btn.addEventListener('click', () => {
            const id = btn.getAttribute('data-id');
            const name = btn.getAttribute('value');
            const idInput = deleteDialog.querySelector('#idDelete');
            // Establecer el ID en el formulario
            idInput.value = id;

            // Actualizar el título del diálogo
            const dialogTitle = deleteDialog.querySelector('h1');
            if (dialogTitle && pageTitle) {
                dialogTitle.textContent = `¿Desea eliminar ${pageTitle.textContent}: ${name}?`;
            }

            deleteDialog.showModal();
        });
    });

    // Manejar el cierre del diálogo
    if (btnCancel) {
        btnCancel.addEventListener('click', () => deleteDialog.close());
    }

    // Cerrar al hacer clic fuera del diálogo
    deleteDialog.addEventListener('click', (event) => {
        const rect = deleteDialog.getBoundingClientRect();
        const isInDialog = (rect.top <= event.clientY && event.clientY <= rect.top + rect.height &&
            rect.left <= event.clientX && event.clientX <= rect.left + rect.width);
        if (!isInDialog) {
            deleteDialog.close();
        }
    });
}
