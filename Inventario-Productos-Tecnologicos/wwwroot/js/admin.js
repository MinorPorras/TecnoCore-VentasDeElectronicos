document.addEventListener("DOMContentLoaded", function () {
    if (document.querySelector('.modifyElement')) {
        modifyElement();
        if (document.querySelector('#imgSelector')) {
            let imgForm = document.querySelector('#imgSelector');
            imgForm.addEventListener('change', () => mostrarImagen(imgForm));
        }
    }
    if (document.querySelector('.deleteDialog')) {
        deleteElement();
    }
    if (document.querySelector('.kardexForm')) {
        openSearchModal();
        kardexHandlers();
    }
});

function mostrarImagen(input) {
    if (input.files && input.files[0]) {
        let reader = new FileReader();
        reader.onload = function (e) {
            let preview = document.getElementById('preview');
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
    const btnCancel = document.querySelector('.btnCancel');
    const btnSubmit = document.querySelector('#btnSubmit');

    if (!deleteDialog || !showModalBtns.length) return;

    showModalBtns.forEach(btn => {
        btn.addEventListener('click', () => {
            const id = btn.getAttribute('data-id');
            const name = btn.getAttribute('value');
            const isActive = btn.getAttribute('data-active').toLowerCase() === 'true';
            const idInput = deleteDialog.querySelector('#idDelete');
            const activeInput = deleteDialog.querySelector('#active');
            
            // Establecer valores en el formulario
            idInput.value = id;
            activeInput.value = !isActive; // Invertimos el valor actual

            // Actualizar el título y el botón del diálogo
            const dialogTitle = deleteDialog.querySelector('h1');
            const accion = isActive ? 'desactivar' : 'activar';
            dialogTitle.textContent = `¿Desea ${accion} ${name}?`;

            if (isActive) {
                btnSubmit.classList.add('btn-danger');
                btnSubmit.classList.remove('btn-success');
                btnSubmit.textContent = 'Desactivar';
            } else {
                btnSubmit.classList.add('btn-success');
                btnSubmit.classList.remove('btn-danger');
                btnSubmit.textContent = 'Activar';
            }

            deleteDialog.showModal();
        });
    });

    // Manejar el cierre del diálogo con el botón Cancelar
    btnCancel.addEventListener('click', () => {
        deleteDialog.close();
    });

    // Cerrar el diálogo al hacer clic fuera de él
    deleteDialog.addEventListener('click', (e) => {
        const dialogDimensions = deleteDialog.getBoundingClientRect();
        if (
            e.clientX < dialogDimensions.left ||
            e.clientX > dialogDimensions.right ||
            e.clientY < dialogDimensions.top ||
            e.clientY > dialogDimensions.bottom
        ) {
            deleteDialog.close();
        }
    });
}

function openSearchModal() {
    document.getElementById('showModalBtn').addEventListener('click', function () {
        document.getElementById('searchProductoDialog').showModal();
    });
}

function selectProductoKardex(productoId, productoNombre, Entry) {
    fetch(`/Kardex/GetProductoStock/${productoId}`)
        .then(response => response.json())
        .then(data => {
            document.getElementById('ProductoId').value = productoId;
            document.getElementById('ProductoNombre').value = productoNombre;
            document.getElementById('StockAnterior').value = data.stock;

            document.getElementById('searchProductoDialog').close();
            let cant;
            if (Entry) {
                cant = parseInt(document.getElementById('Cantidad').value) || 0; // Obtener Cantidad de Entrada
            } else {
                cant = parseInt(document.getElementById('CantidadExit').value) || 0; // Obtener Cantidad de Entrada
            }

            if (isNaN(cant)) {
                cant = 0;
            }
            document.getElementById('StockActual').value = cant + parseInt(data.stock); // Inicializar Stock Actual
        })
        .catch(error => console.error('Error:', error));
}

function kardexHandlers() {
    // Verifica si estamos en la vista de entrada
    const cantidadEntry = document.getElementById('Cantidad');
    if (cantidadEntry) {
        cantidadEntry.addEventListener('change', function () {
            const stockAnterior = parseInt(document.getElementById('StockAnterior').value) || 0;
            const cantidad = parseInt(this.value) || 0;
            document.getElementById('StockActual').value = stockAnterior + cantidad;
        });
    }

    // Verifica si estamos en la vista de salida
    const cantidadExit = document.getElementById('CantidadExit');
    if (cantidadExit) {
        cantidadExit.addEventListener('change', function () {
            const stockAnterior = parseInt(document.getElementById('StockAnterior').value) || 0;
            const cantidad = parseInt(this.value) || 0;
            if (cantidad > stockAnterior) {
                alert("La cantidad a retirar no puede ser mayor al stock actual.");
                this.value = stockAnterior;
                return;
            }
            document.getElementById('StockActual').value = stockAnterior - cantidad;
        });
    }
}
