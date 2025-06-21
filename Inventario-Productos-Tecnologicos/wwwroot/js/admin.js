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
        kardexHandlers();
        openSearchModal();
    }
    if (document.querySelector('.cuponesForm')) {
        handleCuponesForm(true);
    }
    if (document.querySelector('.cuponesFormEdit')) {
        handleCuponesForm(false);
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

function showAlert(message, type = 'success') {
    const alertContainer = document.getElementById('alertContainer');
    const alert = document.createElement('div');
    alert.className = `alert alert-${type} alert-dismissible fade show`;
    alert.role = 'alert';
    alert.innerHTML = `
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
    `;
    alertContainer.appendChild(alert);

    // Auto-cerrar la alerta después de 5 segundos
    setTimeout(() => {
        alert.classList.remove('show');
        setTimeout(() => alert.remove(), 150);
    }, 5000);
}

function modifyElement() {
    const updateBtn = document.getElementById('updateBtn')
    updateBtn.addEventListener('click', async (e) => {
        e.preventDefault()
        const form = document.querySelector('.modifyElement');
        const controller = document.getElementById('controller').value
        const action = document.getElementById('action').value
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

            const data = await response.json();

            if (response.ok) {
                showAlert('Operación realizada con éxito', 'success');
                setTimeout(() => {
                    window.location.href = `/${controller}/Index`;
                }, 1500);
            } else {
                showAlert(data.message || 'Ha ocurrido un error', data.type || 'danger');
            }
        } catch (e) {
            showAlert('Error en la operación', 'danger');
            console.error('Error:', e);
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
            const isActive = btn.getAttribute('data-active').toLowerCase();
            const idInput = deleteDialog.querySelector('#idDelete');
            const activeInput = deleteDialog.querySelector('#active');

            // Establecer valores en el formulario
            idInput.value = id;
            activeInput.value = !isActive; // Invertimos el valor actual

            // Actualizar el título y el botón del diálogo
            const dialogTitle = deleteDialog.querySelector('h1');
            let accion;
            switch (isActive) {
                case 'true':
                    accion = 'desactivar';
                    btnSubmit.classList.add('btn-danger');
                    btnSubmit.classList.remove('btn-success');
                    btnSubmit.textContent = 'Desactivar';
                    break;
                case 'false':
                    accion = 'activar';
                    btnSubmit.classList.add('btn-success');
                    btnSubmit.classList.remove('btn-danger');
                    btnSubmit.textContent = 'Activar';
                    break;
                case 'delete':
                    accion = 'eliminar';
                    btnSubmit.classList.add('btn-danger');
                    btnSubmit.classList.remove('btn-success');
                    btnSubmit.textContent = 'Eliminar';
                    break;
                default:
                    accion = 'realizar esta acción sobre';
                    btnSubmit.classList.add('btn-success');
                    btnSubmit.classList.remove('btn-danger');
                    btnSubmit.textContent = 'Confirmar';
                    break;
            }
            dialogTitle.textContent = `¿Desea ${accion}: ${name}?`;

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

function handleCuponesForm(create = true) {
    const fechaInicio = document.querySelector('#FechaInicio');
    const fechaFin = document.querySelector('#FechaFin');
    const today = new Date();
    const tomorrow = new Date(today);
    console.log(create);

    if (create) {
        // Obtener fecha de hoy
        fechaInicio.value = today.toISOString().split('T')[0];
    }
    if (create) {
        // Obtener fecha de mañana
        tomorrow.setDate(today.getDate() + 1);
        fechaFin.value = tomorrow.toISOString().split('T')[0];
    }

    // Actualizar fecha mínima de fin cuando cambie la fecha de inicio
    fechaInicio.addEventListener('change', function () {
        if (fechaInicio.value) {
            const selectedDate = new Date(fechaInicio.value);
            const minDate = new Date(selectedDate);
            minDate.setDate(selectedDate.getDate() + 1);

            const year = minDate.getFullYear();
            const month = String(minDate.getMonth() + 1).padStart(2, '0');
            const day = String(minDate.getDate()).padStart(2, '0');
            fechaFin.min = `${year}-${month}-${day}`;
        }
    });

    // Manejar la visualización de símbolos según el tipo de descuento
    const tipoDescuento = document.querySelector('#TipoDescuento');
    const symbolColon = document.getElementById('symbolColon');
    const symbolPorc = document.getElementById('symbolPorc');

    if (tipoDescuento.value === "1") { // Porcentaje
        symbolColon.style.display = 'none';
        symbolPorc.style.display = 'inline';
    } else if (tipoDescuento.value === "2") { // Fijo
        symbolColon.style.display = 'inline';
        symbolPorc.style.display = 'none';
    }

    tipoDescuento.addEventListener('change', function () {
        if (this.value === "1") { // Porcentaje
            symbolColon.style.display = 'none';
            symbolPorc.style.display = 'inline';
        } else if (this.value === "2") { // Fijo
            symbolColon.style.display = 'inline';
            symbolPorc.style.display = 'none';
        }
    });
}
